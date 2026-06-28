using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ODOL6.Model;
using ODOL6.Model.MLOD;
using ODOL6.Model.ODOL;

namespace ODOL6;

public static class Methods
{
	public static void Swap<T>(ref T v1, ref T v2)
	{
		T val = v2;
		T val2 = v1;
		v1 = val;
		v2 = val2;
	}

	public static bool EqualsFloat(float f1, float f2, float tolerance = 0.0001f)
	{
		return Math.Abs(f1 - f2) <= tolerance;
	}

	public static IEnumerable<T> Yield<T>(this T src)
	{
		yield return src;
	}

	public static IEnumerable<T> Yield<T>(params T[] elems)
	{
		return elems;
	}

	public static string CharsToString(this IEnumerable<char> chars)
	{
		return new string(chars.ToArray());
	}

	internal static void Convert6P3D(string filePath, bool extractmodelcfg, bool extractmaterial)
	{
		string text = filePath.Replace(".p3d", "_mlod.p3d");
		if (File.Exists(text))
		{
			File.Delete(text);
		}
		P3D instance = P3D.GetInstance(filePath);
		if (instance is MLOD)
		{
			Console.WriteLine("'" + filePath + "' is already in editable MLOD format");
		}
		else if (instance is ODOL oDOL)
		{
			Console.WriteLine("'" + filePath + "': ODOL was loaded successfully.");
			MLOD mLOD = Conversion.ODOL2MLOD(oDOL);
			Console.WriteLine("'" + filePath + "': Conversion successful.");
			Console.WriteLine($"'{filePath}': Saving as: '{Path.GetFileName(text)}'");
			mLOD.WriteToFile(text, allowOverwriting: true);
			if (extractmodelcfg)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
				Console.WriteLine("'" + fileNameWithoutExtension + "': Extracting model.cfg...");
				FileInfo fileInfo = new FileInfo(text);
				if (fileInfo.Directory != null)
				{
					FileInfo fileInfo2 = new FileInfo(Path.Combine(fileInfo.Directory.FullName, "model.cfg"));
					try
					{
						string contents = ((!fileInfo2.Exists) ? oDOL.GetModelCfg() : oDOL.CombineModelCfg(File.ReadAllLines(fileInfo2.FullName)));
						File.WriteAllText(fileInfo2.FullName, contents);
					}
					catch (Exception ex)
					{
						Console.WriteLine("'" + fileNameWithoutExtension + $"': model.cfg extraction skipped ({ex.GetType().Name}).");
					}
				}
			}
			if (!extractmaterial)
			{
				return;
			}
			string text2 = filePath.Remove(filePath.Length - 4, 4) + "_Rvmats";
			P3D_LOD[] lODs = oDOL.LODs;
			foreach (P3D_LOD p3D_LOD in lODs)
			{
				try
				{
					LOD lOD = (LOD)p3D_LOD;
					if (lOD.Materials == null)
					{
						continue;
					}
					EmbeddedMaterial[] materials = lOD.Materials;
					foreach (EmbeddedMaterial obj in materials)
					{
						string fileName = Path.GetFileName(obj.materialName);
						if (!Directory.Exists(text2))
						{
							Directory.CreateDirectory(text2);
						}
						Console.WriteLine("Extracted: " + fileName);
						obj.WriteToFile(Path.Combine(text2, fileName));
					}
				}
				catch
				{
				}
			}
		}
		else
		{
			Console.WriteLine("'" + filePath + "' could not be loaded.");
		}
	}
}
