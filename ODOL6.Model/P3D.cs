using System;
using System.IO;
using System.Linq;
using ODOL6.Model.MLOD;
using ODOL6.Model.ODOL;
using ODOL6.Stream;

namespace ODOL6.Model;

public abstract class P3D
{
	public uint Version { get; protected set; }

	public abstract P3D_LOD[] LODs { get; }

	public abstract float Mass { get; }

	public static P3D GetInstance(string fileName)
	{
		FileStream fileStream = File.OpenRead(fileName);
		string text = new BinaryReaderEx(fileStream).ReadAscii(4);
		fileStream.Close();
		if (text == "ODOL")
		{
			return new ODOL6.Model.ODOL.ODOL(fileName);
		}
		if (text == "MLOD")
		{
			return new ODOL6.Model.MLOD.MLOD(fileName);
		}
		throw new FormatException();
	}

	public virtual P3D_LOD GetLOD(float resolution)
	{
		return LODs.FirstOrDefault((P3D_LOD lod) => lod.Resolution == resolution);
	}
}
