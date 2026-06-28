using System;
using System.IO;

namespace ODOL5.Compression;

public static class LZSS
{
	public static uint readLZSS(System.IO.Stream input, out byte[] dst, uint expectedSize, bool useSignedChecksum)
	{
		char[] array = new char[4113];
		dst = new byte[expectedSize];
		if (expectedSize == 0)
		{
			return 0u;
		}
		long position = input.Position;
		uint num = expectedSize;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < 4078; i++)
		{
			array[i] = ' ';
		}
		int num4 = 4078;
		int num5 = 0;
		while (num != 0)
		{
			if (((num5 >>= 1) & 0x100) == 0)
			{
				num5 = input.ReadByte() | 0xFF00;
			}
			if ((num5 & 1) != 0)
			{
				int num6 = input.ReadByte();
				num3 = ((!useSignedChecksum) ? (num3 + (byte)num6) : (num3 + (sbyte)num6));
				dst[num2++] = (byte)num6;
				num--;
				array[num4] = (char)num6;
				num4++;
				num4 &= 0xFFF;
				continue;
			}
			int num7 = input.ReadByte();
			int num8 = input.ReadByte();
			num7 |= (num8 & 0xF0) << 4;
			num8 &= 0xF;
			num8 += 2;
			int j = num4 - num7;
			int num9 = num8 + j;
			if (num8 + 1 > num)
			{
				throw new ArgumentException("LZSS overflow");
			}
			for (; j <= num9; j++)
			{
				int num10 = (byte)array[j & 0xFFF];
				num3 = ((!useSignedChecksum) ? (num3 + (byte)num10) : (num3 + (sbyte)num10));
				dst[num2++] = (byte)num10;
				num--;
				array[num4] = (char)num10;
				num4++;
				num4 &= 0xFFF;
			}
		}
		byte[] array2 = new byte[4];
		input.Read(array2, 0, 4);
		if (BitConverter.ToInt32(array2, 0) != num3)
		{
			throw new ArgumentException("Checksum mismatch");
		}
		return (uint)(input.Position - position);
	}
}
