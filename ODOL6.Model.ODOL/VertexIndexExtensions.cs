using System;
using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public static class VertexIndexExtensions
{
	public static VertexIndex ReadVertexIndex(this BinaryReaderEx input)
	{
		if (input.Version >= 69)
		{
			return input.ReadInt32();
		}
		return input.ReadUInt16();
	}

	public static VertexIndex[] ReadCompressedVertexIndexArray(this BinaryReaderEx input)
	{
		if (input.Version >= 69)
		{
			return input.ReadCompressedArray((Func<BinaryReaderEx, VertexIndex>)((BinaryReaderEx i) => i.ReadInt32()), 4);
		}
		return input.ReadCompressedArray((Func<BinaryReaderEx, VertexIndex>)((BinaryReaderEx i) => i.ReadUInt16()), 2);
	}
}
