using System;

namespace ODOL6.Model.ODOL;

[Flags]
public enum ClipFlags
{
	ClipNone = 0,
	ClipFront = 1,
	ClipBack = 2,
	ClipLeft = 4,
	ClipRight = 8,
	ClipBottom = 0x10,
	ClipTop = 0x20,
	ClipUser0 = 0x40,
	ClipAll = 0x3F,
	ClipLandMask = 0xF00,
	ClipLandStep = 0x100,
	ClipLandNone = 0,
	ClipLandOn = 0x100,
	ClipLandUnder = 0x200,
	ClipLandAbove = 0x400,
	ClipLandKeep = 0x800,
	ClipDecalMask = 0x3000,
	ClipDecalStep = 0x1000,
	ClipDecalNone = 0,
	ClipDecalNormal = 0x1000,
	ClipDecalVertical = 0x2000,
	ClipFogMask = 0xC000,
	ClipFogStep = 0x4000,
	ClipFogNormal = 0,
	ClipFogDisable = 0x4000,
	ClipFogSky = 0x8000,
	ClipLightMask = 0xF0000,
	ClipLightStep = 0x10000,
	ClipLightNormal = 0,
	ClipLightLine = 0x80000,
	ClipUserMask = 0xFF00000,
	ClipUserStep = 0x100000,
	MaxUserValue = 0xFF,
	ClipHints = 0xFFFFF00
}
