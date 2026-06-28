using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ODOL6.Common;
using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class EmbeddedMaterial : IDeserializable
{
	private enum EFogMode
	{
		FM_None,
		FM_Fog,
		FM_Alpha,
		FM_FogAlpha,
		FM_FogSky
	}

	private enum EMainLight
	{
		ML_None,
		ML_Sun,
		ML_Sky,
		ML_Horizon,
		ML_Stars,
		ML_SunObject,
		ML_SunHaloObject,
		ML_MoonObject,
		ML_MoonHaloObject
	}

	public enum PixelShaderID : uint
	{
		PSNormal = 0u,
		PSNormalDXTA = 1u,
		PSNormalMap = 2u,
		PSNormalMapThrough = 3u,
		PSNormalMapGrass = 4u,
		PSNormalMapDiffuse = 5u,
		PSDetail = 6u,
		PSInterpolation = 7u,
		PSWater = 8u,
		PSWaterSimple = 9u,
		PSWhite = 10u,
		PSWhiteAlpha = 11u,
		PSAlphaShadow = 12u,
		PSAlphaNoShadow = 13u,
		PSDummy0 = 14u,
		PSDetailMacroAS = 15u,
		PSNormalMapMacroAS = 16u,
		PSNormalMapDiffuseMacroAS = 17u,
		PSNormalMapSpecularMap = 18u,
		PSNormalMapDetailSpecularMap = 19u,
		PSNormalMapMacroASSpecularMap = 20u,
		PSNormalMapDetailMacroASSpecularMap = 21u,
		PSNormalMapSpecularDIMap = 22u,
		PSNormalMapDetailSpecularDIMap = 23u,
		PSNormalMapMacroASSpecularDIMap = 24u,
		PSNormalMapDetailMacroASSpecularDIMap = 25u,
		PSTerrain1 = 26u,
		PSTerrain2 = 27u,
		PSTerrain3 = 28u,
		PSTerrain4 = 29u,
		PSTerrain5 = 30u,
		PSTerrain6 = 31u,
		PSTerrain7 = 32u,
		PSTerrain8 = 33u,
		PSTerrain9 = 34u,
		PSTerrain10 = 35u,
		PSTerrain11 = 36u,
		PSTerrain12 = 37u,
		PSTerrain13 = 38u,
		PSTerrain14 = 39u,
		PSTerrain15 = 40u,
		PSTerrainSimple1 = 41u,
		PSTerrainSimple2 = 42u,
		PSTerrainSimple3 = 43u,
		PSTerrainSimple4 = 44u,
		PSTerrainSimple5 = 45u,
		PSTerrainSimple6 = 46u,
		PSTerrainSimple7 = 47u,
		PSTerrainSimple8 = 48u,
		PSTerrainSimple9 = 49u,
		PSTerrainSimple10 = 50u,
		PSTerrainSimple11 = 51u,
		PSTerrainSimple12 = 52u,
		PSTerrainSimple13 = 53u,
		PSTerrainSimple14 = 54u,
		PSTerrainSimple15 = 55u,
		PSGlass = 56u,
		PSNonTL = 57u,
		PSNormalMapSpecularThrough = 58u,
		PSGrass = 59u,
		PSNormalMapThroughSimple = 60u,
		PSNormalMapSpecularThroughSimple = 61u,
		PSRoad = 62u,
		PSShore = 63u,
		PSShoreWet = 64u,
		PSRoad2Pass = 65u,
		PSShoreFoam = 66u,
		PSNonTLFlare = 67u,
		PSNormalMapThroughLowEnd = 68u,
		PSTerrainGrass1 = 69u,
		PSTerrainGrass2 = 70u,
		PSTerrainGrass3 = 71u,
		PSTerrainGrass4 = 72u,
		PSTerrainGrass5 = 73u,
		PSTerrainGrass6 = 74u,
		PSTerrainGrass7 = 75u,
		PSTerrainGrass8 = 76u,
		PSTerrainGrass9 = 77u,
		PSTerrainGrass10 = 78u,
		PSTerrainGrass11 = 79u,
		PSTerrainGrass12 = 80u,
		PSTerrainGrass13 = 81u,
		PSTerrainGrass14 = 82u,
		PSTerrainGrass15 = 83u,
		PSCrater1 = 84u,
		PSCrater2 = 85u,
		PSCrater3 = 86u,
		PSCrater4 = 87u,
		PSCrater5 = 88u,
		PSCrater6 = 89u,
		PSCrater7 = 90u,
		PSCrater8 = 91u,
		PSCrater9 = 92u,
		PSCrater10 = 93u,
		PSCrater11 = 94u,
		PSCrater12 = 95u,
		PSCrater13 = 96u,
		PSCrater14 = 97u,
		PSSprite = 98u,
		PSSpriteSimple = 99u,
		PSCloud = 100u,
		PSHorizon = 101u,
		PSSuper = 102u,
		PSMulti = 103u,
		PSTerrainX = 104u,
		PSTerrainSimpleX = 105u,
		PSTerrainGrassX = 106u,
		PSTree = 107u,
		PSTreePRT = 108u,
		PSTreeSimple = 109u,
		PSSkin = 110u,
		PSCalmWater = 111u,
		PSTreeAToC = 112u,
		PSGrassAToC = 113u,
		PSTreeAdv = 114u,
		PSTreeAdvSimple = 115u,
		PSTreeAdvTrunk = 116u,
		PSTreeAdvTrunkSimple = 117u,
		PSTreeAdvAToC = 118u,
		PSTreeAdvSimpleAToC = 119u,
		PSTreeSN = 120u,
		PSSpriteExtTi = 121u,
		PSTerrainSNX = 122u,
		PSInterpolationAlpha = 123u,
		PSVolCloud = 124u,
		PSVolCloudSimple = 125u,
		PSUnderwaterOcclusion = 126u,
		PSSimulWeatherClouds = 127u,
		PSSimulWeatherCloudsWithLightning = 128u,
		PSSimulWeatherCloudsCPU = 129u,
		PSSimulWeatherCloudsWithLightningCPU = 130u,
		PSSuperExt = 131u,
		PSSuperHair = 132u,
		PSSuperHairAtoC = 133u,
		PSCaustics = 134u,
		PSRefract = 135u,
		PSSpriteRefract = 136u,
		PSSpriteRefractSimple = 137u,
		PSSuperAToC = 138u,
		PSNonTLFlareNew = 139u,
		PSNonTLFlareLight = 140u,
		PSTerrainNoDetailX = 141u,
		PSTerrainNoDetailSNX = 142u,
		PSTerrainSimpleSNX = 143u,
		PSNormalPiP = 144u,
		PSNonTLFlareNewNoOcclusion = 145u,
		PSEmpty = 146u,
		PSPoint = 147u,
		PSTreeAdvTrans = 148u,
		PSTreeAdvTransAToC = 149u,
		PSCollimator = 150u,
		PSLODDiag = 151u,
		PSDepthOnly = 152u,
		NPixelShaderID = 153u,
		PSUninitialized = uint.MaxValue
	}

	public enum VertexShaderID
	{
		Basic,
		NormalMap,
		NormalMapDiffuse,
		Grass,
		Dummy1,
		Dummy2,
		ShadowVolume,
		Water,
		WaterSimple,
		Sprite,
		Point,
		NormalMapThrough,
		Dummy3,
		Terrain,
		BasicAS,
		NormalMapAS,
		NormalMapDiffuseAS,
		Glass,
		NormalMapSpecularThrough,
		NormalMapThroughNoFade,
		NormalMapSpecularThroughNoFade,
		Shore,
		TerrainGrass,
		Super,
		Multi,
		Tree,
		TreeNoFade,
		TreePRT,
		TreePRTNoFade,
		Skin,
		CalmWater,
		TreeAdv,
		TreeAdvTrunk,
		VolCloud,
		Road,
		UnderwaterOcclusion,
		SimulWeatherClouds,
		SimulWeatherCloudsCPU,
		SpriteOnSurface,
		TreeAdvModNormals,
		Refract,
		SimulWeatherCloudsGS,
		BasicFade,
		Star,
		TreeAdvNoFade,
		NVertexShaderID
	}

	public string materialName;

	private uint version;

	private ColorP emissive;

	private ColorP ambient;

	private ColorP diffuse;

	private ColorP forcedDiffuse;

	private ColorP specular;

	private ColorP specularCopy;

	public float specularPower;

	public PixelShaderID pixelShader;

	public VertexShaderID vertexShader;

	private EMainLight mainLight;

	private EFogMode fogMode;

	public string surfaceFile;

	private uint nRenderFlags;

	private uint renderFlags;

	private uint nStages;

	private uint nTexGens;

	public StageTexture[] stageTextures;

	public StageTransform[] stageTransforms;

	private StageTexture stageTI = new StageTexture();

	public void WriteToFile(string fileName)
	{
		List<string> list = new List<string>();
		ColorP colorP = ambient;
		string item = "Ambient[] = " + colorP.ToString() + ";";
		colorP = emissive;
		string item2 = "Emissive[] = " + colorP.ToString() + ";";
		colorP = diffuse;
		string item3 = "Diffuse[] = " + colorP.ToString() + ";";
		colorP = forcedDiffuse;
		string item4 = "forcedDiffuse[] = " + colorP.ToString() + ";";
		colorP = specular;
		string item5 = "Specular[] = " + colorP.ToString() + ";";
		string item6 = "specularPower = " + specularPower.ToString(new CultureInfo("en-GB").NumberFormat) + ";";
		string text = Enum.GetName(pixelShader.GetType(), pixelShader);
		string text2 = Enum.GetName(vertexShader.GetType(), vertexShader);
		if (text == "")
		{
			text = "Unknown PixelShaderID (" + pixelShader.ToString() + ")";
		}
		if (text2 == "")
		{
			text2 = "Unknown VertexShaderID (" + vertexShader.ToString() + ")";
		}
		string item7 = "PixelShader = " + text + ";";
		string item8 = "VertexShader = " + text2 + ";";
		list.Add(item);
		list.Add(item2);
		list.Add(item3);
		list.Add(item4);
		list.Add(item5);
		list.Add(item6);
		list.Add(item7);
		list.Add(item8);
		if (surfaceFile != "")
		{
			list.Add("surfaceInfo = " + surfaceFile + ";");
		}
		if (stageTextures != null)
		{
			for (int i = 0; i < stageTextures.Length; i++)
			{
				list.Add("class Stage" + i + 1);
				list.Add("{");
				list.Add("\tfilter = " + Enum.GetName(stageTextures[i].textureFilter.GetType(), stageTextures[i].textureFilter) + ";");
				list.Add("\ttexture = " + stageTextures[i].texture + ";");
				list.Add("\tuvSource = " + stageTransforms[i].uvSource.ToString() + ";");
				list.Add("\tclass uvTransform");
				list.Add("\t{");
				list.Add("\t\taside[] = " + stageTransforms[i].transformation.Orientation.Aside?.ToString() + ";");
				list.Add("\t\tup[] = " + stageTransforms[i].transformation.Orientation.Up?.ToString() + ";");
				list.Add("\t\tdir[] = " + stageTransforms[i].transformation.Orientation.Dir?.ToString() + ";");
				list.Add("\t\tpos[] = " + stageTransforms[i].transformation.Position?.ToString() + ";");
				list.Add("\t};");
				list.Add("};");
			}
		}
		File.WriteAllLines(fileName, list.ToArray());
	}

	public void ReadObject2(BinaryReaderEx input)
	{
		materialName = input.ReadAsciiz();
		Logging_Functions.Echo(input, materialName, "materialName");
		version = input.ReadUInt32();
		Logging_Functions.Echo(input, version, "version");
		Console.WriteLine("[Material ver. " + version + "] " + materialName);
		emissive.read(input);
		Logging_Functions.Echo(input, emissive, "emissive");
		ambient.read(input);
		Logging_Functions.Echo(input, ambient, "ambient");
		diffuse.read(input);
		Logging_Functions.Echo(input, diffuse, "diffuse");
		forcedDiffuse.read(input);
		Logging_Functions.Echo(input, forcedDiffuse, "forcedDiffuse");
		specular.read(input);
		Logging_Functions.Echo(input, specular, "specular");
		specularCopy.read(input);
		Logging_Functions.Echo(input, specularCopy, "specularCopy");
		if (version >= 12)
		{
			for (int i = 0; i < 4; i++)
			{
				input.ReadSingle();
			}
		}
		if (version >= 13)
		{
			for (int j = 0; j < 4; j++)
			{
				input.ReadSingle();
			}
		}
		specularPower = input.ReadSingle();
		Logging_Functions.Echo(input, specularPower, "specularPower");
		if (version >= 14)
		{
			input.ReadSingle();
		}
		if (version >= 15)
		{
			input.ReadSingle();
		}
		if (version >= 16)
		{
			input.ReadBytes(16);
		}
		pixelShader = (PixelShaderID)input.ReadUInt32();
		Logging_Functions.Echo(input, pixelShader, "pixelShader");
		vertexShader = (VertexShaderID)input.ReadUInt32();
		Logging_Functions.Echo(input, vertexShader, "vertexShader");
		if (version >= 17)
		{
			input.ReadBytes(44);
		}
		if (version >= 20)
		{
			input.ReadBytes(4);
		}
		uint variable = input.ReadUInt32();
		Logging_Functions.Echo(input, variable, "subtraction");
		uint num = input.ReadUInt32();
		mainLight = (EMainLight)num;
		Logging_Functions.Echo(input, mainLight, "mainLight");
		fogMode = (EFogMode)input.ReadUInt32();
		Logging_Functions.Echo(input, fogMode, "fogMode");
		if (version >= 4)
		{
			Logging_Functions.Echo("RVMat version >= 4");
			nRenderFlags = input.ReadUInt32();
			Logging_Functions.Echo(input, nRenderFlags, "nRenderFlags");
		}
		if (version > 6)
		{
			Logging_Functions.Echo("RVMat version > 6");
			nStages = input.ReadUInt32();
			Logging_Functions.Echo(input, nStages - 1, "nStages", hex: true);
		}
		if (version > 8)
		{
			Logging_Functions.Echo("RVMat version > 8");
			nTexGens = input.ReadUInt32();
			Logging_Functions.Echo(input, nTexGens - 1, "nTexGens");
		}
		if (version == 3)
		{
			input.Position++;
			Logging_Functions.Echo("RVMat version == 3");
		}
		if (version >= 6)
		{
			input.Position += 10L;
			Logging_Functions.Echo("RVMat version >= 6");
			surfaceFile = input.ReadAsciiz();
			Logging_Functions.Echo(input, surfaceFile, "surfaceFile");
		}
		stageTextures = new StageTexture[nStages];
		Logging_Functions.Echo(input, stageTextures, "stageTextures");
		stageTransforms = new StageTransform[nTexGens];
		Logging_Functions.Echo(input, stageTransforms, "stageTransforms");
		if (version < 8)
		{
			Logging_Functions.Echo("RVMat version < 8");
			for (int k = 1; k < nStages; k++)
			{
				stageTransforms[k] = new StageTransform(input);
				stageTextures[k].read(input, version);
				Logging_Functions.Echo(input, stageTextures[k], "stageTextures");
			}
		}
		else
		{
			Logging_Functions.Echo("RVMat version 8+");
			for (int l = 1; l < nStages; l++)
			{
				stageTextures[l] = new StageTexture();
				stageTextures[l].read(input, version);
				Logging_Functions.Echo(input, stageTextures[l], "stageTextures");
			}
			for (int m = 1; m < nTexGens; m++)
			{
				stageTransforms[m] = new StageTransform(input);
				Logging_Functions.Echo(input, stageTransforms[m], "stageTransforms");
			}
		}
		if (version >= 10)
		{
			Logging_Functions.Echo("RVMat version >= 10");
			stageTI.read(input, version);
		}
	}

	public void ReadObject(BinaryReaderEx input)
	{
		materialName = input.ReadAsciiz();
		Logging_Functions.Echo(input, materialName, "materialName");
		version = input.ReadUInt32();
		Logging_Functions.Echo(input, version, "version");
		emissive.read(input);
		Logging_Functions.Echo(input, emissive.ToString(), "emissive");
		ambient.read(input);
		Logging_Functions.Echo(input, ambient.ToString(), "ambient");
		diffuse.read(input);
		Logging_Functions.Echo(input, diffuse.ToString(), "diffuse");
		forcedDiffuse.read(input);
		Logging_Functions.Echo(input, forcedDiffuse.ToString(), "forcedDiffuse");
		specular.read(input);
		Logging_Functions.Echo(input, specular.ToString(), "specular");
		specularCopy.read(input);
		Logging_Functions.Echo(input, specularCopy.ToString(), "specularCopy");
		if (version >= 12)
		{
			input.Position += 16L;
		}
		if (version >= 13)
		{
			input.Position += 16L;
		}
		specularPower = input.ReadSingle();
		Logging_Functions.Echo(input, specularPower, "specularPower");
		if (version >= 14)
		{
			input.Position += 4L;
		}
		if (version >= 15)
		{
			input.Position += 4L;
		}
		if (version >= 17)
		{
			input.Position += 4L;
		}
		if (version >= 18)
		{
			input.Position += 40L;
		}
		if (version >= 20)
		{
			input.Position += 20L;
		}
		if (version == 19)
		{
			input.Position += 16L;
		}
		pixelShader = (PixelShaderID)input.ReadUInt32();
		Logging_Functions.Echo(input, pixelShader, "pixelShader");
		vertexShader = (VertexShaderID)input.ReadUInt32();
		Logging_Functions.Echo(input, vertexShader, "vertexShader");
		uint num = 0u;
		if (version >= 18)
		{
			num = input.ReadUInt32();
			Logging_Functions.Echo(input, num, "probably junk");
		}
		mainLight = (EMainLight)input.ReadUInt32();
		Logging_Functions.Echo(input, mainLight, "mainLight");
		int num2 = 0;
		if (version >= 18)
		{
			num2 = input.ReadByte();
			Logging_Functions.Echo(input, num2 == 100, "HasSurface");
		}
		if (version >= 18 && num2 == 0)
		{
			fogMode = (EFogMode)input.ReadUInt32();
			Logging_Functions.Echo(input, fogMode, "fogMode");
			input.Position--;
		}
		else
		{
			input.Position += 2L;
		}
		surfaceFile = input.ReadAsciiz();
		Logging_Functions.Echo(input, surfaceFile, "surfaceFile");
		nRenderFlags = input.ReadUInt32();
		Logging_Functions.Echo(input, nRenderFlags, "nRenderFlags");
		if (num2 != 0)
		{
			renderFlags = input.ReadUInt32();
			Logging_Functions.Echo(input, renderFlags, "renderFlags");
		}
		nStages = input.ReadUInt32();
		Logging_Functions.Echo(input, nStages, "nStages");
		nTexGens = input.ReadUInt32();
		Logging_Functions.Echo(input, nTexGens, "nTexGens");
		stageTextures = new StageTexture[nStages];
		Logging_Functions.Echo(input, stageTextures.Length, "stageTextures");
		stageTransforms = new StageTransform[nTexGens];
		Logging_Functions.Echo(input, stageTransforms.Length, "stageTransforms");
		if (version < 8)
		{
			Logging_Functions.Echo("RVMat version < 8");
			for (int i = 1; i < nStages; i++)
			{
				stageTransforms[i] = new StageTransform(input);
				stageTextures[i].read(input, version);
				Logging_Functions.Echo(input, stageTextures[i], "stageTextures");
			}
		}
		else
		{
			Logging_Functions.Echo("RVMat version 8+");
			for (int j = 1; j < nStages; j++)
			{
				stageTextures[j] = new StageTexture();
				stageTextures[j].read(input, version);
				Logging_Functions.Echo(input, stageTextures[j], "stageTextures");
			}
			for (int k = 1; k < nTexGens; k++)
			{
				stageTransforms[k] = new StageTransform(input);
				Logging_Functions.Echo(input, stageTransforms[k], "stageTransforms");
			}
		}
		if (version >= 10)
		{
			new StageTexture().read(input, version);
			if (version >= 11)
			{
				Logging_Functions.Echo("RVMat version >= 10");
				stageTI.read(input, version);
			}
		}
		input.Position += 52L;
	}
}
