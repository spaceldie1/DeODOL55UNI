using System.Collections.Generic;
using System.Linq;
using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class Section : IDeserializable
{
	private int faceLowerIndex;

	private int faceUpperIndex;

	private int minBoneIndex;

	private int bonesCount;

	public short textureIndex;

	public uint special;

	public int materialIndex;

	private string mat;

	private uint nStages;

	private float[] areaOverTex;

	private bool shortIndices;

	public uint[] getFaceIndexes(Polygon[] faces)
	{
		uint num = 0u;
		uint num2 = (shortIndices ? 8u : 16u);
		uint num3 = (shortIndices ? 2u : 4u);
		List<uint> list = new List<uint>();
		for (uint num4 = 0u; (ulong)num4 < (ulong)faces.Length; num4++)
		{
			if ((ulong)num >= (ulong)faceLowerIndex && (ulong)num < (ulong)faceUpperIndex)
			{
				list.Add(num4);
			}
			num += num2;
			if (faces[num4].VertexIndices.Length == 4)
			{
				num += num3;
			}
			if ((ulong)num >= (ulong)faceUpperIndex)
			{
				break;
			}
		}
		return list.ToArray();
	}

	public void ReadObject(BinaryReaderEx input)
	{
		int version = input.Version;
		shortIndices = version < 69;
		faceLowerIndex = input.ReadInt32();
		faceUpperIndex = input.ReadInt32();
		minBoneIndex = input.ReadInt32();
		bonesCount = input.ReadInt32();
		input.ReadUInt32();
		textureIndex = input.ReadInt16();
		special = input.ReadUInt32();
		materialIndex = input.ReadInt32();
		if (materialIndex == -1)
		{
			mat = input.ReadAsciiz();
		}
		if (version >= 36)
		{
			nStages = input.ReadUInt32();
			areaOverTex = new float[nStages];
			for (int i = 0; i < nStages; i++)
			{
				areaOverTex[i] = input.ReadSingle();
			}
			if (version >= 67 && input.ReadInt32() >= 1)
			{
				(from _ in Enumerable.Range(0, 11)
					select input.ReadSingle()).ToArray();
			}
		}
		else
		{
			areaOverTex = new float[1];
			areaOverTex[0] = input.ReadSingle();
		}
	}
}
