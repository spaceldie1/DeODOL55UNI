using ODOL6.Stream;

namespace ODOL6.Common.Math;

public class Vector3PCompressed : IDeserializable
{
	private int value;

	private const float scaleFactor = -0.0019569471f;

	public float X
	{
		get
		{
			int num = value & 0x3FF;
			if (num > 511)
			{
				num -= 1024;
			}
			return (float)num * -0.0019569471f;
		}
	}

	public float Y
	{
		get
		{
			int num = (value >> 10) & 0x3FF;
			if (num > 511)
			{
				num -= 1024;
			}
			return (float)num * -0.0019569471f;
		}
	}

	public float Z
	{
		get
		{
			int num = (value >> 20) & 0x3FF;
			if (num > 511)
			{
				num -= 1024;
			}
			return (float)num * -0.0019569471f;
		}
	}

	public static implicit operator Vector3P(Vector3PCompressed src)
	{
		int num = src.value & 0x3FF;
		int num2 = (src.value >> 10) & 0x3FF;
		int num3 = (src.value >> 20) & 0x3FF;
		if (num > 511)
		{
			num -= 1024;
		}
		if (num2 > 511)
		{
			num2 -= 1024;
		}
		if (num3 > 511)
		{
			num3 -= 1024;
		}
		return new Vector3P((float)num * -0.0019569471f, (float)num2 * -0.0019569471f, (float)num3 * -0.0019569471f);
	}

	public static implicit operator int(Vector3PCompressed src)
	{
		return src.value;
	}

	public static implicit operator Vector3PCompressed(int src)
	{
		return new Vector3PCompressed
		{
			value = src
		};
	}

	public void ReadObject(BinaryReaderEx input)
	{
		value = input.ReadInt32();
	}
}
