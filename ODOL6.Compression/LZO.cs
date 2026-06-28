using System;
using System.IO;

namespace ODOL6.Compression;

public static class LZO
{
	private static readonly uint M2_MAX_OFFSET = 2048u;

	public unsafe static uint decompress(byte* input, byte* output, uint expectedSize)
	{
		byte* ptr = output + expectedSize;
		byte* ptr2 = output;
		byte* ptr3 = input;
		uint num;
		if (*ptr3 > 17)
		{
			num = (uint)(*(ptr3++) - 17);
			if (num >= 4)
			{
				if (ptr - ptr2 < num)
				{
					throw new OverflowException("Outpur Overrun");
				}
				do
				{
					*(ptr2++) = *(ptr3++);
				}
				while (--num != 0);
				goto IL_004b;
			}
			goto IL_0303;
		}
		goto IL_0350;
		IL_0303:
		if (ptr - ptr2 < num)
		{
			throw new OverflowException("Output Overrun");
		}
		*(ptr2++) = *(ptr3++);
		if (num > 1)
		{
			*(ptr2++) = *(ptr3++);
			if (num > 2)
			{
				*(ptr2++) = *(ptr3++);
			}
		}
		num = *(ptr3++);
		goto IL_00cd;
		IL_00cd:
		byte* ptr4;
		if (num >= 64)
		{
			ptr4 = ptr2 - 1;
			ptr4 -= (num >> 2) & 7;
			ptr4 -= *(ptr3++) << 3;
			num = (num >> 5) - 1;
			if (ptr4 < output || ptr4 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if (ptr - ptr2 < num + 2)
			{
				throw new OverflowException("Output Overrun");
			}
		}
		else
		{
			if (num >= 32)
			{
				num &= 0x1F;
				if (num == 0)
				{
					for (; *ptr3 == 0; ptr3++)
					{
						num += 255;
					}
					num += (uint)(31 + *(ptr3++));
				}
				ptr4 = ptr2 - 1;
				ptr4 -= (*ptr3 >> 2) + (ptr3[1] << 6);
				ptr3 += 2;
			}
			else
			{
				if (num < 16)
				{
					ptr4 = ptr2 - 1;
					ptr4 -= num >> 2;
					ptr4 -= *(ptr3++) << 2;
					if (ptr4 < output || ptr4 >= ptr2)
					{
						throw new OverflowException("Lookbehind Overrun");
					}
					if (ptr - ptr2 < 2)
					{
						throw new OverflowException("Output Overrun");
					}
					*(ptr2++) = *(ptr4++);
					*(ptr2++) = *ptr4;
					goto IL_02f8;
				}
				ptr4 = ptr2;
				ptr4 -= (num & 8) << 11;
				num &= 7;
				if (num == 0)
				{
					for (; *ptr3 == 0; ptr3++)
					{
						num += 255;
					}
					num += (uint)(7 + *(ptr3++));
				}
				ptr4 -= (*ptr3 >> 2) + (ptr3[1] << 6);
				ptr3 += 2;
				if (ptr4 == ptr2)
				{
					_ = ptr2 - output;
					if (ptr4 != ptr)
					{
						throw new OverflowException("Output Underrun");
					}
					return (uint)(ptr3 - input);
				}
				ptr4 -= 16384;
			}
			if (ptr4 < output || ptr4 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if (ptr - ptr2 < num + 2)
			{
				throw new OverflowException("Output Overrun");
			}
			if (num >= 6 && ptr2 - ptr4 >= 4)
			{
				*(int*)ptr2 = *(int*)ptr4;
				ptr2 += 4;
				ptr4 += 4;
				num -= 2;
				do
				{
					*(int*)ptr2 = *(int*)ptr4;
					ptr2 += 4;
					ptr4 += 4;
					num -= 4;
				}
				while (num >= 4);
				if (num != 0)
				{
					do
					{
						*(ptr2++) = *(ptr4++);
					}
					while (--num != 0);
				}
				goto IL_02f8;
			}
		}
		*(ptr2++) = *(ptr4++);
		*(ptr2++) = *(ptr4++);
		do
		{
			*(ptr2++) = *(ptr4++);
		}
		while (--num != 0);
		goto IL_02f8;
		IL_02f8:
		num = (uint)(ptr3[-2] & 3);
		if (num != 0)
		{
			goto IL_0303;
		}
		goto IL_0350;
		IL_004b:
		num = *(ptr3++);
		if (num >= 16)
		{
			goto IL_00cd;
		}
		ptr4 = ptr2 - (1 + M2_MAX_OFFSET);
		ptr4 -= num >> 2;
		ptr4 -= *(ptr3++) << 2;
		if (ptr4 < output || ptr4 >= ptr2)
		{
			throw new OverflowException("Lookbehind Overrun");
		}
		if (ptr - ptr2 < 3)
		{
			throw new OverflowException("Output Overrun");
		}
		*(ptr2++) = *(ptr4++);
		*(ptr2++) = *(ptr4++);
		*(ptr2++) = *ptr4;
		goto IL_02f8;
		IL_0350:
		num = *(ptr3++);
		if (num < 16)
		{
			if (num == 0)
			{
				for (; *ptr3 == 0; ptr3++)
				{
					num += 255;
				}
				num += (uint)(15 + *(ptr3++));
			}
			if (ptr - ptr2 < num + 3)
			{
				throw new OverflowException("Output Overrun");
			}
			*(int*)ptr2 = *(int*)ptr3;
			ptr2 += 4;
			ptr3 += 4;
			if (--num != 0)
			{
				if (num < 4)
				{
					do
					{
						*(ptr2++) = *(ptr3++);
					}
					while (--num != 0);
				}
				else
				{
					do
					{
						*(int*)ptr2 = *(int*)ptr3;
						ptr2 += 4;
						ptr3 += 4;
						num -= 4;
					}
					while (num >= 4);
					if (num != 0)
					{
						do
						{
							*(ptr2++) = *(ptr3++);
						}
						while (--num != 0);
					}
				}
			}
			goto IL_004b;
		}
		goto IL_00cd;
	}

	private static byte ip(System.IO.Stream i)
	{
		byte result = (byte)i.ReadByte();
		i.Position--;
		return result;
	}

	private static byte ip(System.IO.Stream i, short offset)
	{
		i.Position += offset;
		byte result = (byte)i.ReadByte();
		i.Position -= offset + 1;
		return result;
	}

	private static byte next(System.IO.Stream i)
	{
		return (byte)i.ReadByte();
	}

	public unsafe static uint decompress(System.IO.Stream i, byte* output, uint expectedSize)
	{
		long position = i.Position;
		byte* ptr = output + expectedSize;
		byte* ptr2 = output;
		uint num;
		if (ip(i) > 17)
		{
			num = (uint)(next(i) - 17);
			if (num >= 4)
			{
				if (ptr - ptr2 < num)
				{
					throw new OverflowException("Outpur Overrun");
				}
				do
				{
					*(ptr2++) = next(i);
				}
				while (--num != 0);
				goto IL_0054;
			}
			goto IL_035d;
		}
		goto IL_03aa;
		IL_035d:
		if (ptr - ptr2 < num)
		{
			throw new OverflowException("Output Overrun");
		}
		*(ptr2++) = next(i);
		if (num > 1)
		{
			*(ptr2++) = next(i);
			if (num > 2)
			{
				*(ptr2++) = next(i);
			}
		}
		num = next(i);
		goto IL_00d6;
		IL_00d6:
		byte* ptr3;
		if (num >= 64)
		{
			ptr3 = ptr2 - 1;
			ptr3 -= (num >> 2) & 7;
			ptr3 -= next(i) << 3;
			num = (num >> 5) - 1;
			if (ptr3 < output || ptr3 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if (ptr - ptr2 < num + 2)
			{
				throw new OverflowException("Output Overrun");
			}
		}
		else
		{
			if (num >= 32)
			{
				num &= 0x1F;
				if (num == 0)
				{
					while (ip(i) == 0)
					{
						num += 255;
						i.Position++;
					}
					num += (uint)(31 + next(i));
				}
				ptr3 = ptr2 - 1;
				ptr3 -= (ip(i, 0) >> 2) + (ip(i, 1) << 6);
				i.Position += 2L;
			}
			else
			{
				if (num < 16)
				{
					ptr3 = ptr2 - 1;
					ptr3 -= num >> 2;
					ptr3 -= next(i) << 2;
					if (ptr3 < output || ptr3 >= ptr2)
					{
						throw new OverflowException("Lookbehind Overrun");
					}
					if (ptr - ptr2 < 2)
					{
						throw new OverflowException("Output Overrun");
					}
					*(ptr2++) = *(ptr3++);
					*(ptr2++) = *ptr3;
					goto IL_034f;
				}
				ptr3 = ptr2;
				ptr3 -= (num & 8) << 11;
				num &= 7;
				if (num == 0)
				{
					while (ip(i) == 0)
					{
						num += 255;
						i.Position++;
					}
					num += (uint)(7 + next(i));
				}
				ptr3 -= (ip(i, 0) >> 2) + (ip(i, 1) << 6);
				i.Position += 2L;
				if (ptr3 == ptr2)
				{
					_ = ptr2 - output;
					if (ptr3 != ptr)
					{
						throw new OverflowException("Output Underrun");
					}
					return (uint)(i.Position - position);
				}
				ptr3 -= 16384;
			}
			if (ptr3 < output || ptr3 >= ptr2)
			{
				throw new OverflowException("Lookbehind Overrun");
			}
			if (ptr - ptr2 < num + 2)
			{
				throw new OverflowException("Output Overrun");
			}
			if (num >= 6 && ptr2 - ptr3 >= 4)
			{
				*(int*)ptr2 = *(int*)ptr3;
				ptr2 += 4;
				ptr3 += 4;
				num -= 2;
				do
				{
					*(int*)ptr2 = *(int*)ptr3;
					ptr2 += 4;
					ptr3 += 4;
					num -= 4;
				}
				while (num >= 4);
				if (num != 0)
				{
					do
					{
						*(ptr2++) = *(ptr3++);
					}
					while (--num != 0);
				}
				goto IL_034f;
			}
		}
		*(ptr2++) = *(ptr3++);
		*(ptr2++) = *(ptr3++);
		do
		{
			*(ptr2++) = *(ptr3++);
		}
		while (--num != 0);
		goto IL_034f;
		IL_034f:
		num = (uint)(ip(i, -2) & 3);
		if (num != 0)
		{
			goto IL_035d;
		}
		goto IL_03aa;
		IL_0054:
		num = next(i);
		if (num >= 16)
		{
			goto IL_00d6;
		}
		ptr3 = ptr2 - (1 + M2_MAX_OFFSET);
		ptr3 -= num >> 2;
		ptr3 -= next(i) << 2;
		if (ptr3 < output || ptr3 >= ptr2)
		{
			throw new OverflowException("Lookbehind Overrun");
		}
		if (ptr - ptr2 < 3)
		{
			throw new OverflowException("Output Overrun");
		}
		*(ptr2++) = *(ptr3++);
		*(ptr2++) = *(ptr3++);
		*(ptr2++) = *ptr3;
		goto IL_034f;
		IL_03aa:
		num = next(i);
		if (num < 16)
		{
			if (num == 0)
			{
				while (ip(i) == 0)
				{
					num += 255;
					i.Position++;
				}
				num += (uint)(15 + next(i));
			}
			if (ptr - ptr2 < num + 3)
			{
				throw new OverflowException("Output Overrun");
			}
			*(ptr2++) = next(i);
			*(ptr2++) = next(i);
			*(ptr2++) = next(i);
			*(ptr2++) = next(i);
			if (--num != 0)
			{
				if (num < 4)
				{
					do
					{
						*(ptr2++) = next(i);
					}
					while (--num != 0);
				}
				else
				{
					do
					{
						*(ptr2++) = next(i);
						*(ptr2++) = next(i);
						*(ptr2++) = next(i);
						*(ptr2++) = next(i);
						num -= 4;
					}
					while (num >= 4);
					if (num != 0)
					{
						do
						{
							*(ptr2++) = next(i);
						}
						while (--num != 0);
					}
				}
			}
			goto IL_0054;
		}
		goto IL_00d6;
	}

	public unsafe static uint readLZO(System.IO.Stream input, out byte[] dst, uint expectedSize)
	{
		dst = new byte[expectedSize];
		fixed (byte* output = &dst[0])
		{
			return decompress(input, output, expectedSize);
		}
	}

	public unsafe static byte[] readLZO(System.IO.Stream input, uint expectedSize)
	{
		byte[] result = Array.Empty<byte>();
		try
		{
			byte[] array = new byte[expectedSize];
			fixed (byte* output = &array[0])
			{
				decompress(input, output, expectedSize);
			}
			return array;
		}
		catch
		{
			return result;
		}
	}
}
