namespace ODOL6.Model;

public static class P3D_FaceFlags
{
	public static byte GetUserValue(this FaceFlags flags)
	{
		return (byte)((long)((ulong)flags & 0xFE000000uL) >> 24);
	}

	public static void SetUserValue(this FaceFlags flags, byte value)
	{
		flags &= (FaceFlags)33554431;
		flags += value << 24;
	}
}
