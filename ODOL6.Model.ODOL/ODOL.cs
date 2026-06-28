using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ODOL6.Common;
using ODOL6.Common.Math;
using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class ODOL : P3D
{
	private static readonly bool TraceEnabled = Environment.GetEnvironmentVariable("DEODOL_TRACE") == "1";

	public string fileName;

	public const int LATEST_VERSION = 73;

	public const int MINIMAL_VERSION = 28;

	private string muzzleFlash;

	private uint appID;

	private int nLods;

	private float[] resolutions;

	public ODOL_ModelInfo modelInfo;

	internal bool hasAnims;

	private Animations animations = new Animations();

	private uint[] lodStartAdresses;

	private uint[] lodEndAdresses;

	private bool[] permanent;

	private List<LoadableLodInfo> LoadableLodInfos;

	private LOD[] lods;

	private int buoyancyType;

	public Skeleton Skeleton => modelInfo.skeleton;

	public override float Mass => modelInfo.mass;

	public override P3D_LOD[] LODs => lods;

	public ODOL(string fileName)
	{
		this.fileName = fileName;
		System.IO.Stream stream = File.OpenRead(fileName);
		Read(new BinaryReaderEx(stream));
	}

	public bool IsSnappable()
	{
		LOD lOD = lods.FirstOrDefault((LOD l) => l.Resolution.getLODType() == LodName.Memory);
		if (lOD != null && lOD.NamedSelections.Where((NamedSelection ns) => ns.Name.Equals("lb", StringComparison.InvariantCultureIgnoreCase) || ns.Name.Equals("le", StringComparison.InvariantCultureIgnoreCase) || ns.Name.Equals("pb", StringComparison.InvariantCultureIgnoreCase) || ns.Name.Equals("pe", StringComparison.InvariantCultureIgnoreCase)).Count() >= 4)
		{
			return true;
		}
		return false;
	}

	private void Read(BinaryReaderEx input)
	{
		void Trace(string message)
		{
			if (TraceEnabled)
			{
				Console.WriteLine("[trace] " + message + " @ " + input.Position);
			}
		}

		string text = input.ReadAscii(4);
		if ("ODOL" != text)
		{
			throw new FormatException("ODOL signature is missing");
		}
		base.Version = input.ReadUInt32();
		Console.WriteLine("Version is " + base.Version + ", Hex: " + base.Version.ToString("X4"));
		if (base.Version > 73)
		{
			throw new FormatException("Unknown ODOL version");
		}
		if (base.Version < 28)
		{
			throw new FormatException("Old ODOL version is currently not supported");
		}
		input.Version = (int)base.Version;
		Trace("header-read");
		if (base.Version >= 44)
		{
			input.UseLZOCompression = true;
			Logging_Functions.Echo("LZO Compression Enabled");
		}
		if (base.Version >= 64)
		{
			input.UseCompressionFlag = true;
			Logging_Functions.Echo("Compression Flag Used");
		}
		if (base.Version >= 59)
		{
			appID = input.ReadUInt32();
			Logging_Functions.Echo(input, appID, "appID");
		}
		if (base.Version >= 58)
		{
			muzzleFlash = input.ReadAsciiz();
			Logging_Functions.Echo(input, muzzleFlash, "muzzleFlash");
		}
		nLods = input.ReadInt32();
		Trace("lod-count-read");
		Logging_Functions.Echo(input, nLods, "nLods");
		resolutions = new float[nLods];
		for (int i = 0; i < nLods; i++)
		{
			resolutions[i] = input.ReadSingle();
			Logging_Functions.Echo("Found resolution with index " + i + " and data " + resolutions[i]);
		}
		Trace("lod-resolutions-read");
		modelInfo = new ODOL_ModelInfo(input, nLods);
		Trace("model-info-read");
		if (base.Version > 29)
		{
			hasAnims = input.ReadBoolean();
			Trace("has-anims-read");
			if (hasAnims)
			{
				animations.Read(input);
				Trace("animations-read");
				Logging_Functions.Echo("Animations present and read.");
			}
		}
		lodStartAdresses = new uint[nLods];
		lodEndAdresses = new uint[nLods];
		permanent = new bool[nLods];
		for (int j = 0; j < nLods; j++)
		{
			lodStartAdresses[j] = input.ReadUInt32();
			Logging_Functions.Echo("LOD start address of LOD " + j + " found at: " + lodStartAdresses[j] + ", Hex: " + lodStartAdresses[j].ToString("X4"));
		}
		Trace("lod-starts-read");
		for (int k = 0; k < nLods; k++)
		{
			lodEndAdresses[k] = input.ReadUInt32();
			Logging_Functions.Echo("LOD end address of LOD " + k + " found at: " + lodEndAdresses[k] + ", Hex: " + lodEndAdresses[k].ToString("X4"));
		}
		Trace("lod-ends-read");
		for (int l = 0; l < nLods; l++)
		{
			permanent[l] = input.ReadBoolean();
			Logging_Functions.Echo("LOD " + l + " is permanent: " + permanent[l]);
		}
		Trace("lod-permanent-flags-read");
		LoadableLodInfos = new List<LoadableLodInfo>(nLods);
		lods = new LOD[nLods];
		long position = input.Position;
		for (int m = 0; m < nLods; m++)
		{
			if (!permanent[m])
			{
				LoadableLodInfo loadableLodInfo = new LoadableLodInfo();
				loadableLodInfo.ReadObject(input);
				LoadableLodInfos.Add(loadableLodInfo);
				position = input.Position;
			}
			input.Position = lodStartAdresses[m];
			lods[m] = new LOD();
			Trace("lod-" + m + "-read-start");
			Logging_Functions.Echo("Processing LOD: " + m);
			lods[m].Read(input, resolutions[m]);
			Trace("lod-" + m + "-read-end");
			input.Position = position;
		}
		if (base.Version > 54)
		{
			input.Position = lodEndAdresses.Max();
			buoyancyType = input.ReadInt32();
		}
		input.Close();
	}

	public string[] GetHiddenSelectionNames()
	{
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 0; i < nLods; i++)
		{
			Section[] sections = lods[i].Sections;
			for (int j = 0; j < sections.Length; j++)
			{
				_ = sections[j];
				hashSet.Add("");
			}
			for (int k = 0; k < lods[i].NamedSelections.Count(); k++)
			{
				if (lods[i].NamedSelections[k].IsSectional)
				{
					hashSet.Add(lods[i].NamedSelections[k].Name);
				}
			}
		}
		List<string> list = new List<string>();
		foreach (string item in hashSet)
		{
			if (item != "")
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public string[] GetAxisSelectionNames()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < nLods; i++)
		{
			if (!(lods[i].Name == "Memory"))
			{
				continue;
			}
			for (int j = 0; j < animations.axisData[i].Count(); j++)
			{
				if (animations.axisData[i][j] == null)
				{
					list.Add("");
					continue;
				}
				NamedSelection[] namedSelections = lods[i].NamedSelections;
				foreach (NamedSelection namedSelection in namedSelections)
				{
					if (namedSelection.SelectedVertices.Count() != 2)
					{
						continue;
					}
					switch (animations.animationClasses[j].animType)
					{
					case Animations.AnimationClass.AnimType.Rotation:
					case Animations.AnimationClass.AnimType.RotationX:
					case Animations.AnimationClass.AnimType.RotationY:
					case Animations.AnimationClass.AnimType.RotationZ:
					{
						Vector3P a2 = lods[i].Points[namedSelection.SelectedVertices[0].value];
						if (AreVectorsAboutEqual(a2, animations.axisData[i][j][0]))
						{
							list.Add(namedSelection.Name);
						}
						break;
					}
					case Animations.AnimationClass.AnimType.Translation:
					case Animations.AnimationClass.AnimType.TranslationX:
					case Animations.AnimationClass.AnimType.TranslationY:
					case Animations.AnimationClass.AnimType.TranslationZ:
					{
						Vector3P vector3P = lods[i].Points[namedSelection.SelectedVertices[0].value];
						Vector3P a = lods[i].Points[namedSelection.SelectedVertices[1].value] - vector3P;
						if (AreVectorsAboutEqual(a, animations.axisData[i][j][1]))
						{
							list.Add(namedSelection.Name);
						}
						break;
					}
					}
				}
			}
			break;
		}
		return list.ToArray();
	}

	public bool AreVectorsAboutEqual(Vector3P a, Vector3P b)
	{
		if ((double)Math.Abs(a.X - b.X) > 1E-06)
		{
			return false;
		}
		if ((double)Math.Abs(a.Y - b.Y) > 1E-06)
		{
			return false;
		}
		if ((double)Math.Abs(a.Z - b.Z) > 1E-06)
		{
			return false;
		}
		return true;
	}

	public string CombineModelCfg(string[] existing)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		foreach (string text in existing)
		{
			if (text.ToLower().Contains("class cfgskeletons"))
			{
				if (modelInfo.skeleton.Name != "")
				{
					flag = true;
				}
				flag2 = false;
			}
			else if (text.ToLower().Contains("class cfgmodels"))
			{
				flag = false;
				flag2 = true;
			}
			else if (flag)
			{
				if (text.Contains("\tclass " + modelInfo.skeleton.Name))
				{
					flag = false;
				}
				else if (text.StartsWith("};"))
				{
					stringBuilder.AppendLine("\tclass " + modelInfo.skeleton.Name);
					stringBuilder.AppendLine("\t{");
					stringBuilder.AppendLine("\t\tIsDiscrete = " + modelInfo.skeleton.isDiscrete.GetHashCode() + ";");
					stringBuilder.AppendLine("\t\tSkeletonInherit = \"" + modelInfo.skeleton.pivotsNameObsolete + "\";");
					stringBuilder.AppendLine("\t\tSkeletonBones[] =");
					stringBuilder.AppendLine("\t\t{");
					for (int j = 0; j < modelInfo.skeleton.bones.Count(); j += 2)
					{
						if (j + 1 < modelInfo.skeleton.bones.Count())
						{
							stringBuilder.Append("\t\t\t\"" + modelInfo.skeleton.bones[j] + "\", \"" + modelInfo.skeleton.bones[j + 1] + "\"");
							if (j + 1 != modelInfo.skeleton.bones.Count() - 1)
							{
								stringBuilder.Append(",");
							}
							stringBuilder.AppendLine();
						}
					}
					stringBuilder.AppendLine("\t\t};");
					stringBuilder.AppendLine("\t};");
				}
			}
			else if (flag2)
			{
				if (text.Contains("\tclass " + Path.GetFileNameWithoutExtension(fileName)))
				{
					flag2 = false;
				}
				else if (text.Contains("class Default"))
				{
					flag3 = true;
				}
				else if (text.StartsWith("};"))
				{
					flag2 = false;
					if (!flag3)
					{
						stringBuilder.AppendLine("\tclass Default");
						stringBuilder.AppendLine("\t{");
						stringBuilder.AppendLine("\t\tSections[] = {};");
						stringBuilder.AppendLine("\t\tSectionsInherit = \"\";");
						stringBuilder.AppendLine("\t\tSkeletonName = \"\";");
						stringBuilder.AppendLine("\t};");
					}
					stringBuilder.AppendLine("\tclass " + Path.GetFileNameWithoutExtension(fileName) + ": Default");
					stringBuilder.AppendLine("\t{");
					string[] hiddenSelectionNames = GetHiddenSelectionNames();
					if (hiddenSelectionNames.Count() != 0)
					{
						stringBuilder.AppendLine("\t\tSections[] =");
						stringBuilder.AppendLine("\t\t{");
						for (int k = 0; k < hiddenSelectionNames.Count(); k++)
						{
							stringBuilder.Append("\t\t\t\"" + hiddenSelectionNames[k] + "\"");
							if (k + 1 != hiddenSelectionNames.Count())
							{
								stringBuilder.Append(",");
							}
							stringBuilder.AppendLine();
						}
						stringBuilder.AppendLine("\t\t};");
					}
					if (modelInfo.skeleton.Name != "")
					{
						stringBuilder.AppendLine("\t\tSkeletonName = \"" + modelInfo.skeleton.Name + "\";");
					}
					if (animations.animationClasses != null)
					{
						string[] axisSelectionNames = GetAxisSelectionNames();
						stringBuilder.AppendLine("\t\tclass Animations");
						stringBuilder.AppendLine("\t\t{");
						for (int l = 0; l < animations.animationClasses.Count(); l++)
						{
							if (l < axisSelectionNames.Length)
							{
								stringBuilder.AppendLine("\t\t\tclass " + animations.animationClasses[l].animName);
								stringBuilder.AppendLine("\t\t\t{");
								stringBuilder.AppendLine("\t\t\t\ttype = \"" + animations.animationClasses[l].animType.ToString() + "\";");
								stringBuilder.AppendLine("\t\t\t\tsource = \"" + animations.animationClasses[l].animSource + "\";");
								stringBuilder.AppendLine("\t\t\t\tselection = \"" + modelInfo.skeleton.bones[animations.Anims2Bones[0][l] * 2] + "\";");
								if (!string.IsNullOrEmpty(axisSelectionNames[l]))
								{
									stringBuilder.AppendLine("\t\t\t\taxis = \"" + axisSelectionNames[l] + "\";");
								}
								if (animations.animationClasses[l].sourceAddress != Animations.AnimationClass.AnimAddress.AnimClamp)
								{
									stringBuilder.AppendLine("\t\t\t\tsourceAddress = \"" + animations.animationClasses[l].sourceAddress.ToString() + "\";");
								}
								if (animations.animationClasses[l].minPhase != 0f || animations.animationClasses[l].maxPhase != 1f)
								{
									stringBuilder.AppendLine("\t\t\t\tminPhase = \"" + animations.animationClasses[l].minPhase + "\";");
								}
								if (animations.animationClasses[l].maxPhase != 1f)
								{
									stringBuilder.AppendLine("\t\t\t\tmaxPhase = \"" + animations.animationClasses[l].maxPhase + "\";");
								}
								stringBuilder.AppendLine("\t\t\t\tminValue = \"" + animations.animationClasses[l].minValue + "\";");
								stringBuilder.AppendLine("\t\t\t\tmaxValue = \"" + animations.animationClasses[l].maxValue + "\";");
								switch (animations.animationClasses[l].animType)
								{
								case Animations.AnimationClass.AnimType.Rotation:
								case Animations.AnimationClass.AnimType.RotationX:
								case Animations.AnimationClass.AnimType.RotationY:
								case Animations.AnimationClass.AnimType.RotationZ:
									stringBuilder.AppendLine("\t\t\t\tangle0 = \"" + animations.animationClasses[l].angle0 + "\";");
									stringBuilder.AppendLine("\t\t\t\tangle1 = \"" + animations.animationClasses[l].angle1 + "\";");
									break;
								case Animations.AnimationClass.AnimType.Translation:
								case Animations.AnimationClass.AnimType.TranslationX:
								case Animations.AnimationClass.AnimType.TranslationY:
								case Animations.AnimationClass.AnimType.TranslationZ:
									stringBuilder.AppendLine("\t\t\t\toffset0 = \"" + animations.animationClasses[l].offset0 + "\";");
									stringBuilder.AppendLine("\t\t\t\toffset1 = \"" + animations.animationClasses[l].offset1 + "\";");
									break;
								case Animations.AnimationClass.AnimType.Direct:
									stringBuilder.AppendLine("\t\t\t\taxisPos = \"" + animations.animationClasses[l].axisPos?.ToString() + "\";");
									stringBuilder.AppendLine("\t\t\t\taxisDir = \"" + animations.animationClasses[l].axisDir?.ToString() + "\";");
									stringBuilder.AppendLine("\t\t\t\tangle = \"" + animations.animationClasses[l].angle + "\";");
									stringBuilder.AppendLine("\t\t\t\taxisOffset = \"" + animations.animationClasses[l].axisOffset + "\";");
									break;
								case Animations.AnimationClass.AnimType.Hide:
									stringBuilder.AppendLine("\t\t\t\thideValue = \"" + animations.animationClasses[l].hideValue + "\";");
									break;
								}
								stringBuilder.AppendLine("\t\t\t};");
							}
							else
							{
								Console.WriteLine($"Index out of range: idxAnimClass = {l}, axes.Length = {axisSelectionNames.Length}");
							}
						}
						stringBuilder.AppendLine("\t\t};");
					}
					stringBuilder.AppendLine("\t};");
				}
			}
			stringBuilder.AppendLine(text);
		}
		return stringBuilder.ToString();
	}

	public string GetModelCfg()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (modelInfo.skeleton.bones != null)
		{
			stringBuilder.AppendLine("class CfgSkeletons");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("\tclass " + modelInfo.skeleton.Name);
			stringBuilder.AppendLine("\t{");
			stringBuilder.AppendLine("\t\tIsDiscrete = " + modelInfo.skeleton.isDiscrete.GetHashCode() + ";");
			stringBuilder.AppendLine("\t\tSkeletonInherit = \"" + modelInfo.skeleton.pivotsNameObsolete + "\";");
			stringBuilder.AppendLine("\t\tSkeletonBones[] =");
			stringBuilder.AppendLine("\t\t{");
			for (int i = 0; i < modelInfo.skeleton.bones.Count(); i += 2)
			{
				if (i + 1 < modelInfo.skeleton.bones.Count())
				{
					stringBuilder.Append("\t\t\t\"" + modelInfo.skeleton.bones[i] + "\", \"" + modelInfo.skeleton.bones[i + 1] + "\"");
					if (i + 1 != modelInfo.skeleton.bones.Count() - 1)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.AppendLine();
				}
				else
				{
					Console.WriteLine($"Index out of range: idxBone = {i}, modelInfo.skeleton.bones.Count() = {modelInfo.skeleton.bones.Count()}");
				}
			}
			stringBuilder.AppendLine("\t\t};");
			stringBuilder.AppendLine("\t};");
			stringBuilder.AppendLine("};\n");
		}
		stringBuilder.AppendLine("class CfgModels");
		stringBuilder.AppendLine("{");
		stringBuilder.AppendLine("\tclass Default");
		stringBuilder.AppendLine("\t{");
		stringBuilder.AppendLine("\t\tSections[] = {};");
		stringBuilder.AppendLine("\t\tSectionsInherit = \"\";");
		stringBuilder.AppendLine("\t\tSkeletonName = \"\";");
		stringBuilder.AppendLine("\t};");
		stringBuilder.AppendLine("\tclass " + Path.GetFileNameWithoutExtension(fileName) + ": Default");
		stringBuilder.AppendLine("\t{");
		if (modelInfo.skeleton.Name != "")
		{
			stringBuilder.AppendLine("\t\tSkeletonName = \"" + modelInfo.skeleton.Name + "\";");
		}
		string[] hiddenSelectionNames = GetHiddenSelectionNames();
		if (hiddenSelectionNames != null && hiddenSelectionNames.Count() != 0)
		{
			stringBuilder.AppendLine("\t\tSections[] =");
			stringBuilder.AppendLine("\t\t{");
			for (int j = 0; j < hiddenSelectionNames.Count(); j++)
			{
				stringBuilder.Append("\t\t\t\"" + hiddenSelectionNames[j] + "\"");
				if (j + 1 != hiddenSelectionNames.Count())
				{
					stringBuilder.Append(",");
				}
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine("\t\t};");
		}
		if (animations.animationClasses != null && animations.Anims2Bones.Count() > 0)
		{
			string[] axisSelectionNames = GetAxisSelectionNames();
			stringBuilder.AppendLine("\t\tclass Animations");
			stringBuilder.AppendLine("\t\t{");
			for (int k = 0; k < animations.animationClasses.Count(); k++)
			{
				if (animations.Anims2Bones[0].Count() > k && axisSelectionNames.Length > k)
				{
					stringBuilder.AppendLine("\t\t\tclass " + animations.animationClasses[k].animName);
					stringBuilder.AppendLine("\t\t\t{");
					stringBuilder.AppendLine("\t\t\t\ttype = \"" + animations.animationClasses[k].animType.ToString() + "\";");
					stringBuilder.AppendLine("\t\t\t\tsource = \"" + animations.animationClasses[k].animSource + "\";");
					stringBuilder.AppendLine("\t\t\t\tselection = \"" + modelInfo.skeleton.bones[animations.Anims2Bones[0][k] * 2] + "\";");
					if (axisSelectionNames[k] != "")
					{
						stringBuilder.AppendLine("\t\t\t\taxis = \"" + axisSelectionNames[k] + "\";");
					}
					if (animations.animationClasses[k].sourceAddress != Animations.AnimationClass.AnimAddress.AnimClamp)
					{
						stringBuilder.AppendLine("\t\t\t\tsourceAddress = \"" + animations.animationClasses[k].sourceAddress.ToString() + "\";");
					}
					if (animations.animationClasses[k].minPhase != 0f || animations.animationClasses[k].maxPhase != 1f)
					{
						stringBuilder.AppendLine("\t\t\t\tminPhase = \"" + animations.animationClasses[k].minPhase + "\";");
					}
					if (animations.animationClasses[k].maxPhase != 1f)
					{
						stringBuilder.AppendLine("\t\t\t\tmaxPhase = \"" + animations.animationClasses[k].maxPhase + "\";");
					}
					stringBuilder.AppendLine("\t\t\t\tminValue = \"" + animations.animationClasses[k].minValue + "\";");
					stringBuilder.AppendLine("\t\t\t\tmaxValue = \"" + animations.animationClasses[k].maxValue + "\";");
					switch (animations.animationClasses[k].animType)
					{
					case Animations.AnimationClass.AnimType.Rotation:
					case Animations.AnimationClass.AnimType.RotationX:
					case Animations.AnimationClass.AnimType.RotationY:
					case Animations.AnimationClass.AnimType.RotationZ:
						stringBuilder.AppendLine("\t\t\t\tangle0 = \"" + animations.animationClasses[k].angle0 + "\";");
						stringBuilder.AppendLine("\t\t\t\tangle1 = \"" + animations.animationClasses[k].angle1 + "\";");
						break;
					case Animations.AnimationClass.AnimType.Translation:
					case Animations.AnimationClass.AnimType.TranslationX:
					case Animations.AnimationClass.AnimType.TranslationY:
					case Animations.AnimationClass.AnimType.TranslationZ:
						stringBuilder.AppendLine("\t\t\t\toffset0 = \"" + animations.animationClasses[k].offset0 + "\";");
						stringBuilder.AppendLine("\t\t\t\toffset1 = \"" + animations.animationClasses[k].offset1 + "\";");
						break;
					case Animations.AnimationClass.AnimType.Direct:
						stringBuilder.AppendLine("\t\t\t\taxisPos = \"" + animations.animationClasses[k].axisPos?.ToString() + "\";");
						stringBuilder.AppendLine("\t\t\t\taxisDir = \"" + animations.animationClasses[k].axisDir?.ToString() + "\";");
						stringBuilder.AppendLine("\t\t\t\tangle = \"" + animations.animationClasses[k].angle + "\";");
						stringBuilder.AppendLine("\t\t\t\taxisOffset = \"" + animations.animationClasses[k].axisOffset + "\";");
						break;
					case Animations.AnimationClass.AnimType.Hide:
						stringBuilder.AppendLine("\t\t\t\thideValue = \"" + animations.animationClasses[k].hideValue + "\";");
						break;
					}
					stringBuilder.AppendLine("\t\t\t};");
				}
			}
			stringBuilder.AppendLine("\t\t};");
		}
		stringBuilder.AppendLine("\t};");
		stringBuilder.AppendLine("};");
		return stringBuilder.ToString();
	}
}
