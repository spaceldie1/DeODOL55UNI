using System;
using System.IO;
using ODOL5.Stream;

namespace ODOL5.Model.MLOD;

public class MLOD : P3D
{
	private MLOD_LOD[] lods;

	public override P3D_LOD[] LODs => lods;

	public override float Mass
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public MLOD(string fileName)
	{
		byte[] array = File.ReadAllBytes(fileName);
		BinaryReaderEx binaryReaderEx = new BinaryReaderEx(new MemoryStream(array, 0, array.Length, writable: false, publiclyVisible: true));
		Read(binaryReaderEx);
		binaryReaderEx.Close();
	}

	public MLOD(System.IO.Stream stream)
	{
		Read(new BinaryReaderEx(stream));
	}

	public MLOD(MLOD_LOD[] lods)
	{
		this.lods = lods;
	}

	private void Read(BinaryReaderEx input)
	{
		if (input.ReadAscii(4) != "MLOD")
		{
			throw new Exception("MLOD signature expected");
		}
		base.Version = input.ReadUInt32();
		if (base.Version != 257)
		{
			throw new Exception("Unknown MLOD version");
		}
		uint num = input.ReadUInt32();
		lods = new MLOD_LOD[num];
		for (int i = 0; i < num; i++)
		{
			lods[i] = new MLOD_LOD();
			lods[i].Read(input);
		}
	}

	private void Write(BinaryWriterEx output)
	{
		output.writeAscii("MLOD", 4u);
		output.Write(257);
		output.Write(lods.Length);
		for (int i = 0; i < lods.Length; i++)
		{
			lods[i].Write(output);
		}
	}

	public bool WriteToFile(string file, bool allowOverwriting = false)
	{
		try
		{
			FileMode mode = ((!allowOverwriting) ? FileMode.CreateNew : FileMode.Create);
			BinaryWriterEx binaryWriterEx = new BinaryWriterEx(new FileStream(file, mode));
			Write(binaryWriterEx);
			binaryWriterEx.Close();
		}
		catch (IOException ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	public MemoryStream WriteToMemory()
	{
		MemoryStream memoryStream = new MemoryStream(100000);
		BinaryWriterEx binaryWriterEx = new BinaryWriterEx(memoryStream);
		Write(binaryWriterEx);
		binaryWriterEx.Position = 0L;
		return memoryStream;
	}

	public void WriteToStream(System.IO.Stream stream)
	{
		BinaryWriterEx output = new BinaryWriterEx(stream);
		Write(output);
	}
}
