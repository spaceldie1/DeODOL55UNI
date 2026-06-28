using System;
using System.IO;
using System.Runtime.InteropServices;
using ODOL6.Compression;

namespace ODOL6.Stream;

public class BinaryReaderEx : BinaryReader
{
	public bool UseCompressionFlag { get; set; }

	public bool UseLZOCompression { get; set; }

	public int Version { get; set; }

	public long Position
	{
		get
		{
			return BaseStream.Position;
		}
		set
		{
			BaseStream.Position = value;
		}
	}

	public BinaryReaderEx(System.IO.Stream stream)
		: base(stream)
	{
		UseCompressionFlag = false;
	}

	public uint ReadUInt24()
	{
		return (uint)(ReadByte() + (ReadByte() << 8) + (ReadByte() << 16));
	}

	public string ReadAscii(int count)
	{
		string text = "";
		for (int i = 0; i < count; i++)
		{
			text += ReadSanitised(ReadByte());
		}
		return text;
	}

	public string ReadAsciiz()
	{
		string text = "";
		char c = '\0';
		while ((c = ReadSanitised(ReadByte())) != 0)
		{
			text += c;
		}
		return text;
	}

	public static char ReadSanitised(byte b)
	{
		if (b > 127)
		{
			return '?';
		}
		return (char)b;
	}

	private T ReadObject<T>() where T : IDeserializable, new()
	{
		T result = new T();
		result.ReadObject(this);
		return result;
	}

	private T[] ReadArrayBase<T>(Func<BinaryReaderEx, T> readElement, int size)
	{
		try
		{
			T[] array = new T[size];
			for (int i = 0; i < size; i++)
			{
				try
				{
					array[i] = readElement(this);
				}
				catch
				{
				}
			}
			return array;
		}
		catch
		{
			Console.WriteLine("Error!");
			return Array.Empty<T>();
		}
	}

	public T[] ReadArray<T>(Func<BinaryReaderEx, T> readElement)
	{
		int num = ReadInt32();
		if (num < 0)
		{
			return Array.Empty<T>();
		}
		long num2 = BaseStream.Length - BaseStream.Position;
		if ((long)num > num2)
		{
			return Array.Empty<T>();
		}
		return ReadArrayBase(readElement, num);
	}

	public T[] ReadArray<T>() where T : IDeserializable, new()
	{
		return ReadArray((BinaryReaderEx i) => i.ReadObject<T>());
	}

	public float[] ReadFloatArray()
	{
		return ReadArray((BinaryReaderEx i) => i.ReadSingle());
	}

	public int[] ReadIntArray()
	{
		return ReadArray((BinaryReaderEx i) => i.ReadInt32());
	}

	public string[] ReadStringArray()
	{
		return ReadArray((BinaryReaderEx i) => i.ReadAsciiz());
	}

	public T[] ReadCompressedArray<T>(Func<BinaryReaderEx, T> readElement, int elemSize)
	{
		int num = ReadInt32();
		if (num < 0 || num > 10000000)
		{
			return Array.Empty<T>();
		}
		uint expectedSize = (uint)(num * elemSize);
		return new BinaryReaderEx(new MemoryStream(ReadCompressed(expectedSize))).ReadArrayBase(readElement, num);
	}

	public T[] ReadCompressedArray<T>(Func<BinaryReaderEx, T> readElement)
	{
		return ReadCompressedArray(readElement, Marshal.SizeOf(typeof(T)));
	}

	public T[] ReadCompressedObjectArray<T>(int sizeOfT) where T : IDeserializable, new()
	{
		return ReadCompressedArray((BinaryReaderEx i) => i.ReadObject<T>(), sizeOfT);
	}

	public short[] ReadCompressedShortArray()
	{
		return ReadCompressedArray((BinaryReaderEx i) => i.ReadInt16());
	}

	public int[] ReadCompressedIntArray()
	{
		return ReadCompressedArray((BinaryReaderEx i) => i.ReadInt32());
	}

	public float[] ReadCompressedFloatArray()
	{
		return ReadCompressedArray((BinaryReaderEx i) => i.ReadSingle());
	}

	public T[] ReadCondensedArray<T>(Func<BinaryReaderEx, T> readElement, int sizeOfT)
	{
		int num = ReadInt32();
		if (num < 0 || num > 10000000)
		{
			return Array.Empty<T>();
		}
		T[] array = new T[num];
		if (ReadBoolean())
		{
			T val = readElement(this);
			for (int i = 0; i < num; i++)
			{
				array[i] = val;
			}
			return array;
		}
		uint expectedSize = (uint)(num * sizeOfT);
		BinaryReaderEx binaryReaderEx = new BinaryReaderEx(new MemoryStream(ReadCompressed(expectedSize)));
		array = binaryReaderEx.ReadArrayBase(readElement, num);
		binaryReaderEx.Close();
		return array;
	}

	public T[] ReadCondensedObjectArray<T>(int sizeOfT) where T : IDeserializable, new()
	{
		return ReadCondensedArray((BinaryReaderEx i) => i.ReadObject<T>(), sizeOfT);
	}

	public int[] ReadCondensedIntArray()
	{
		return ReadCondensedArray((BinaryReaderEx i) => i.ReadInt32(), 4);
	}

	public int ReadCompactInteger()
	{
		int num = ReadByte();
		if ((num & 0x80) != 0)
		{
			int num2 = ReadByte();
			num += (num2 - 1) * 128;
		}
		return num;
	}

	public byte[] ReadCompressed(uint expectedSize)
	{
		if (expectedSize == 0)
		{
			return new byte[0];
		}
		if (UseLZOCompression)
		{
			return ReadLZO(expectedSize);
		}
		return ReadLZSS(expectedSize);
	}

	public byte[] ReadLZO(uint expectedSize)
	{
		bool flag = expectedSize >= 1024;
		if (UseCompressionFlag)
		{
			flag = ReadBoolean();
		}
		if (!flag)
		{
			return ReadBytes((int)expectedSize);
		}
		return LZO.readLZO(BaseStream, expectedSize);
	}

	public byte[] ReadLZSS(uint expectedSize, bool inPAA = false)
	{
		if (expectedSize < 1024 && !inPAA)
		{
			return ReadBytes((int)expectedSize);
		}
		byte[] dst = new byte[expectedSize];
		LZSS.readLZSS(BaseStream, out dst, expectedSize, inPAA);
		return dst;
	}

	public byte[] ReadCompressedIndices(int bytesToRead, uint expectedSize)
	{
		byte[] array = new byte[expectedSize];
		int num = 0;
		for (int i = 0; i < bytesToRead; i++)
		{
			byte b = ReadByte();
			if ((b & 0x80) != 0)
			{
				byte b2 = (byte)(b - 127);
				byte b3 = ReadByte();
				for (int j = 0; j < b2; j++)
				{
					array[num++] = b3;
				}
			}
			else
			{
				for (int k = 0; k < b + 1; k++)
				{
					array[num++] = ReadByte();
				}
			}
		}
		return array;
	}

	public uint skipGridCompressed()
	{
		long position = Position;
		ushort num = ReadUInt16();
		for (int i = 0; i < 16; i++)
		{
			if ((num & 1) == 1)
			{
				skipGridCompressed();
			}
			else
			{
				Position += 4L;
			}
			num >>= 1;
		}
		return (uint)(Position - position);
	}
}
