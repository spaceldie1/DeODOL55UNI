using ODOL5.Common;
using ODOL5.Common.Math;
using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class ODOL_ModelInfo
{
	public int special { get; private set; }

	public float BoundingSphere { get; private set; }

	public float GeometrySphere { get; private set; }

	public int remarks { get; private set; }

	public int andHints { get; private set; }

	public int orHints { get; private set; }

	public Vector3P AimingCenter { get; private set; }

	public PackedColor color { get; private set; }

	public PackedColor colorType { get; private set; }

	public float viewDensity { get; private set; }

	public Vector3P bboxMin { get; private set; }

	public Vector3P bboxMax { get; private set; }

	public float propertyLodDensityCoef { get; private set; }

	public float propertyDrawImportance { get; private set; }

	public Vector3P bboxMinVisual { get; private set; }

	public Vector3P bboxMaxVisual { get; private set; }

	public Vector3P boundingCenter { get; private set; }

	public Vector3P geometryCenter { get; private set; }

	public Vector3P centerOfMass { get; private set; }

	public Matrix3P invInertia { get; private set; }

	public bool autoCenter { get; private set; }

	public bool lockAutoCenter { get; private set; }

	public bool canOcclude { get; private set; }

	public bool canBeOccluded { get; private set; }

	public bool AICovers { get; private set; }

	public float htMin { get; private set; }

	public float htMax { get; private set; }

	public float afMax { get; private set; }

	public float mfMax { get; private set; }

	public float mFact { get; private set; }

	public float tBody { get; private set; }

	public bool forceNotAlphaModel { get; private set; }

	public SBSource sbSource { get; private set; }

	public bool prefershadowvolume { get; private set; }

	public float shadowOffset { get; private set; }

	public bool animated { get; private set; }

	public Skeleton skeleton { get; private set; }

	public MapType mapType { get; private set; }

	public float[] massArray { get; private set; }

	public float mass { get; private set; }

	public float invMass { get; private set; }

	public float armor { get; private set; }

	public float invArmor { get; private set; }

	public float propertyExplosionShielding { get; private set; }

	public byte memory { get; private set; }

	public byte geometry { get; private set; }

	public byte geometrySimple { get; private set; }

	public byte geometryPhys { get; private set; }

	public byte geometryFire { get; private set; }

	public byte geometryView { get; private set; }

	public byte geometryViewPilot { get; private set; }

	public byte geometryViewGunner { get; private set; }

	public byte geometryViewCargo { get; private set; }

	public byte landContact { get; private set; }

	public byte roadway { get; private set; }

	public byte paths { get; private set; }

	public byte hitpoints { get; private set; }

	public byte minShadow { get; private set; }

	public bool canBlend { get; private set; }

	public string propertyClass { get; private set; }

	public string propertyDamage { get; private set; }

	public bool propertyFrequent { get; private set; }

	public int[] preferredShadowVolumeLod { get; private set; }

	public int[] preferredShadowBufferLod { get; private set; }

	public int[] preferredShadowBufferLodVis { get; private set; }

	internal ODOL_ModelInfo(BinaryReaderEx input, int nLods)
	{
		Read(input, nLods);
	}

	public void Read(BinaryReaderEx input, int nLods)
	{
		int version = input.Version;
		special = input.ReadInt32();
		Logging_Functions.Echo(input, special, "special", hex: true);
		BoundingSphere = input.ReadSingle();
		Logging_Functions.Echo(input, BoundingSphere, "BoundingSphere");
		GeometrySphere = input.ReadSingle();
		Logging_Functions.Echo(input, GeometrySphere, "GeometrySphere");
		remarks = input.ReadInt32();
		Logging_Functions.Echo(input, remarks, "remarks", hex: true);
		andHints = input.ReadInt32();
		Logging_Functions.Echo(input, andHints, "andHints", hex: true);
		orHints = input.ReadInt32();
		Logging_Functions.Echo(input, orHints, "orHints", hex: true);
		AimingCenter = new Vector3P(input);
		Logging_Functions.Echo(input, AimingCenter, "AimingCenter");
		color = new PackedColor(input.ReadUInt32());
		Logging_Functions.Echo(input, color, "color");
		colorType = new PackedColor(input.ReadUInt32());
		Logging_Functions.Echo(input, colorType, "colorType");
		viewDensity = input.ReadSingle();
		Logging_Functions.Echo(input, viewDensity, "viewDensity");
		bboxMin = new Vector3P(input);
		Logging_Functions.Echo(input, bboxMin, "bboxMin");
		bboxMax = new Vector3P(input);
		Logging_Functions.Echo(input, bboxMax, "bboxMax");
		if (version >= 70)
		{
			propertyLodDensityCoef = input.ReadSingle();
			Logging_Functions.Echo(input, propertyLodDensityCoef, "propertyLodDensityCoef");
		}
		if (version >= 71)
		{
			propertyDrawImportance = input.ReadSingle();
			Logging_Functions.Echo(input, propertyDrawImportance, "propertyDrawImportance");
		}
		if (version >= 52)
		{
			bboxMinVisual = new Vector3P(input);
			Logging_Functions.Echo(input, bboxMinVisual, "bboxMinVisual");
			bboxMaxVisual = new Vector3P(input);
			Logging_Functions.Echo(input, bboxMaxVisual, "bboxMaxVisual");
		}
		boundingCenter = new Vector3P(input);
		Logging_Functions.Echo(input, boundingCenter, "boundingCenter");
		geometryCenter = new Vector3P(input);
		Logging_Functions.Echo(input, geometryCenter, "geometryCenter");
		centerOfMass = new Vector3P(input);
		Logging_Functions.Echo(input, centerOfMass, "centerOfMass");
		invInertia = new Matrix3P(input);
		Logging_Functions.Echo(input, invInertia, "invInertia");
		autoCenter = input.ReadBoolean();
		Logging_Functions.Echo(input, autoCenter, "autoCenter");
		lockAutoCenter = input.ReadBoolean();
		Logging_Functions.Echo(input, lockAutoCenter, "lockAutoCenter");
		canOcclude = input.ReadBoolean();
		Logging_Functions.Echo(input, canOcclude, "canOcclude");
		canBeOccluded = input.ReadBoolean();
		Logging_Functions.Echo(input, canBeOccluded, "canBeOccluded");
		if (version >= 73)
		{
			AICovers = input.ReadBoolean();
			Logging_Functions.Echo(input, AICovers, "AICovers");
		}
		if (version >= 53)
		{
			input.ReadBytes(5);
		}
		if ((version >= 42 && version < 10000) || version >= 10042)
		{
			htMin = input.ReadSingle();
			Logging_Functions.Echo(input, htMin, "htMin");
			htMax = input.ReadSingle();
			Logging_Functions.Echo(input, htMax, "htMax");
			afMax = input.ReadSingle();
			Logging_Functions.Echo(input, afMax, "afMax");
			mfMax = input.ReadSingle();
			Logging_Functions.Echo(input, mfMax, "mfMax");
		}
		if ((version >= 43 && version < 10000) || version >= 10043)
		{
			mFact = input.ReadSingle();
			Logging_Functions.Echo(input, mFact, "mFact");
			tBody = input.ReadSingle();
			Logging_Functions.Echo(input, tBody, "tBody");
		}
		if (version >= 33)
		{
			forceNotAlphaModel = input.ReadBoolean();
			Logging_Functions.Echo(input, forceNotAlphaModel, "forceNotAlphaModel");
		}
		if (version >= 37)
		{
			int variable = input.ReadInt32();
			Logging_Functions.Echo(input, variable, "sbSource", hex: true);
			sbSource = (SBSource)variable;
			prefershadowvolume = input.ReadBoolean();
			Logging_Functions.Echo(input, prefershadowvolume, "prefershadowvolume");
		}
		if (version >= 48)
		{
			shadowOffset = input.ReadSingle();
			Logging_Functions.Echo(input, shadowOffset, "shadowOffset");
		}
		animated = input.ReadBoolean();
		Logging_Functions.Echo(input, animated, "animated");
		skeleton = new Skeleton(input);
		Logging_Functions.Echo(input, skeleton, "skeleton");
		mapType = (MapType)input.ReadByte();
		Logging_Functions.Echo(input, mapType, "mapType");
		massArray = input.ReadCompressedFloatArray();
		Logging_Functions.Echo(input, massArray, "massArray");
		if (version >= 54)
		{
			input.ReadByte();
			Logging_Functions.Echo(input, 1, "skip byte for 54");
		}
		mass = input.ReadSingle();
		Logging_Functions.Echo(input, mass, "mass");
		invMass = input.ReadSingle();
		Logging_Functions.Echo(input, invMass, "invMass");
		armor = input.ReadSingle();
		Logging_Functions.Echo(input, armor, "armor");
		invArmor = input.ReadSingle();
		Logging_Functions.Echo(input, invArmor, "invArmor");
		if (version >= 72)
		{
			propertyExplosionShielding = input.ReadSingle();
			Logging_Functions.Echo(input, propertyExplosionShielding, "propertyExplosionShielding");
		}
		if (version > 53)
		{
			geometrySimple = input.ReadByte();
			Logging_Functions.Echo(input, geometrySimple, "geometrySimple");
		}
		if (version >= 54)
		{
			geometryPhys = input.ReadByte();
			Logging_Functions.Echo(input, geometryPhys, "geometryPhys");
		}
		memory = input.ReadByte();
		Logging_Functions.Echo(input, memory, "memory");
		geometry = input.ReadByte();
		Logging_Functions.Echo(input, geometry, "geometry");
		geometryFire = input.ReadByte();
		Logging_Functions.Echo(input, geometryFire, "geometryFire");
		geometryView = input.ReadByte();
		Logging_Functions.Echo(input, geometryView, "geometryView");
		geometryViewPilot = input.ReadByte();
		Logging_Functions.Echo(input, geometryViewPilot, "geometryViewPilot");
		geometryViewGunner = input.ReadByte();
		Logging_Functions.Echo(input, geometryViewGunner, "geometryViewGunner");
		input.ReadSByte();
		geometryViewCargo = input.ReadByte();
		Logging_Functions.Echo(input, geometryViewCargo, "geometryViewCargo");
		landContact = input.ReadByte();
		Logging_Functions.Echo(input, landContact, "landContact");
		roadway = input.ReadByte();
		Logging_Functions.Echo(input, roadway, "roadway");
		paths = input.ReadByte();
		Logging_Functions.Echo(input, paths, "paths");
		hitpoints = input.ReadByte();
		Logging_Functions.Echo(input, hitpoints, "hitpoints");
		minShadow = (byte)input.ReadUInt32();
		Logging_Functions.Echo(input, minShadow, "minShadow");
		if (version >= 38)
		{
			canBlend = input.ReadBoolean();
			Logging_Functions.Echo(input, canBlend, "canBlend");
		}
		propertyClass = input.ReadAsciiz();
		Logging_Functions.Echo(input, propertyClass, "propertyClass");
		propertyDamage = input.ReadAsciiz();
		Logging_Functions.Echo(input, propertyDamage, "propertyDamage");
		propertyFrequent = input.ReadBoolean();
		Logging_Functions.Echo(input, propertyFrequent, "propertyFrequent");
		if (version >= 31)
		{
			input.ReadUInt32();
			Logging_Functions.Echo("Skipping 4 bytes.");
		}
		if (version >= 57)
		{
			preferredShadowVolumeLod = new int[nLods];
			preferredShadowBufferLod = new int[nLods];
			preferredShadowBufferLodVis = new int[nLods];
			for (int i = 0; i < nLods; i++)
			{
				preferredShadowVolumeLod[i] = input.ReadInt32();
			}
			for (int j = 0; j < nLods; j++)
			{
				preferredShadowBufferLod[j] = input.ReadInt32();
			}
			for (int k = 0; k < nLods; k++)
			{
				preferredShadowBufferLodVis[k] = input.ReadInt32();
			}
		}
	}
}
