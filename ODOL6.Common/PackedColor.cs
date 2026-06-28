namespace ODOL6.Common;

public struct PackedColor
{
	private uint value;

	public byte A8 => (byte)((value >> 24) & 0xFF);

	public byte R8 => (byte)((value >> 16) & 0xFF);

	public byte G8 => (byte)((value >> 8) & 0xFF);

	public byte B8 => (byte)(value & 0xFF);

	public PackedColor(uint value)
	{
		this.value = value;
	}

	public PackedColor(byte r, byte g, byte b, byte a = byte.MaxValue)
	{
		value = PackColor(r, g, b, a);
	}

	public PackedColor(float r, float g, float b, float a)
	{
		byte r2 = (byte)(r * 255f);
		byte g2 = (byte)(g * 255f);
		byte b2 = (byte)(b * 255f);
		byte a2 = (byte)(a * 255f);
		value = PackColor(r2, g2, b2, a2);
	}

	internal static uint PackColor(byte r, byte g, byte b, byte a)
	{
		return (uint)((a << 24) | (r << 16) | (g << 8) | b);
	}
}
