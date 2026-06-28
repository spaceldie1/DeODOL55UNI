using System;

namespace ODOL5.Model;

public static class Resolution
{
	private const float specialLod = 1E+15f;

	public const float GEOMETRY = 1E+13f;

	public const float BUOYANCY = 2E+13f;

	public const float PHYSXOLD = 3E+13f;

	public const float PHYSX = 4E+13f;

	public const float MEMORY = 1E+15f;

	public const float LANDCONTACT = 2E+15f;

	public const float ROADWAY = 3E+15f;

	public const float PATHS = 4E+15f;

	public const float HITPOINTS = 5E+15f;

	public const float VIEW_GEOMETRY = 6E+15f;

	public const float FIRE_GEOMETRY = 7E+15f;

	public const float VIEW_GEOMETRY_CARGO = 8E+15f;

	public const float VIEW_GEOMETRY_PILOT = 1.3E+16f;

	public const float VIEW_GEOMETRY_GUNNER = 1.5E+16f;

	public const float FIRE_GEOMETRY_GUNNER = 1.6E+16f;

	public const float SUBPARTS = 1.7E+16f;

	public const float SHADOWVOLUME_CARGO = 1.8E+16f;

	public const float SHADOWVOLUME_PILOT = 1.9E+16f;

	public const float SHADOWVOLUME_GUNNER = 2E+16f;

	public const float WRECK = 2.1E+16f;

	public const float VIEW_COMMANDER = 1E+16f;

	public const float VIEW_GUNNER = 1000f;

	public const float VIEW_PILOT = 1100f;

	public const float VIEW_CARGO = 1200f;

	public const float SHADOWVOLUME = 10000f;

	public const float SHADOWBUFFER = 11000f;

	public const float SHADOW_MIN = 10000f;

	public const float SHADOW_MAX = 20000f;

	public static bool KeepsNamedSelections(float r)
	{
		if (r != 1E+15f && r != 7E+15f && r != 1E+13f && r != 6E+15f && r != 1.3E+16f && r != 1.5E+16f && r != 8E+15f && r != 4E+15f && r != 5E+15f && r != 4E+13f)
		{
			return r == 2E+13f;
		}
		return true;
	}

	public static LodName getLODType(this float res)
	{
		if (res == 1E+15f)
		{
			return LodName.Memory;
		}
		if (res == 2E+15f)
		{
			return LodName.LandContact;
		}
		if (res == 3E+15f)
		{
			return LodName.Roadway;
		}
		if (res == 4E+15f)
		{
			return LodName.Paths;
		}
		if (res == 5E+15f)
		{
			return LodName.HitPoints;
		}
		if (res == 6E+15f)
		{
			return LodName.ViewGeometry;
		}
		if (res == 7E+15f)
		{
			return LodName.FireGeometry;
		}
		if (res == 8E+15f)
		{
			return LodName.ViewCargoGeometry;
		}
		if (res == 9E+15f)
		{
			return LodName.ViewCargoFireGeometry;
		}
		if (res == 1E+16f)
		{
			return LodName.ViewCommander;
		}
		if (res == 1.1E+16f)
		{
			return LodName.ViewCommanderGeometry;
		}
		if (res == 1.2E+16f)
		{
			return LodName.ViewCommanderFireGeometry;
		}
		if (res == 1.3E+16f)
		{
			return LodName.ViewPilotGeometry;
		}
		if (res == 1.4E+16f)
		{
			return LodName.ViewPilotFireGeometry;
		}
		if (res == 1.4999999E+16f)
		{
			return LodName.ViewGunnerGeometry;
		}
		if (res == 1.6E+16f)
		{
			return LodName.ViewGunnerFireGeometry;
		}
		if (res == 1.7E+16f)
		{
			return LodName.SubParts;
		}
		if (res == 1.8E+16f)
		{
			return LodName.ShadowVolumeViewCargo;
		}
		if (res == 1.9E+16f)
		{
			return LodName.ShadowVolumeViewPilot;
		}
		if (res == 2E+16f)
		{
			return LodName.ShadowVolumeViewGunner;
		}
		if (res == 2.1E+16f)
		{
			return LodName.Wreck;
		}
		if (res == 1000f)
		{
			return LodName.ViewGunner;
		}
		if (res == 1100f)
		{
			return LodName.ViewPilot;
		}
		if (res == 1200f)
		{
			return LodName.ViewCargo;
		}
		if (res == 1E+13f)
		{
			return LodName.Geometry;
		}
		if (res == 4E+13f)
		{
			return LodName.PhysX;
		}
		if ((double)res >= 10000.0 && (double)res <= 20000.0)
		{
			return LodName.ShadowVolume;
		}
		return LodName.Resolution;
	}

	public static string getLODName(this float res)
	{
		LodName lODType = res.getLODType();
		return lODType switch
		{
			LodName.Resolution => res.ToString("#.000"), 
			LodName.ShadowVolume => "ShadowVolume" + (res - 10000f).ToString("#.000"), 
			_ => Enum.GetName(typeof(LodName), lODType), 
		};
	}

	public static bool IsResolution(float r)
	{
		return r < 10000f;
	}

	public static bool IsShadow(float r)
	{
		if ((!(r >= 10000f) || !(r < 20000f)) && r != 2E+16f && r != 1.9E+16f)
		{
			return r == 1.8E+16f;
		}
		return true;
	}

	public static bool IsVisual(float r)
	{
		if (!IsResolution(r) && r != 1200f && r != 1000f && r != 1100f)
		{
			return r == 1E+16f;
		}
		return true;
	}
}
