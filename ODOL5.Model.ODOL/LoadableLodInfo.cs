using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class LoadableLodInfo : IDeserializable
{
	private int nFaces;

	private uint color;

	private int special;

	private uint orHints;

	private bool hasSkeleton;

	private int nVertices;

	private float faceArea;

	public void ReadObject(BinaryReaderEx input)
	{
		nFaces = input.ReadInt32();
		color = input.ReadUInt32();
		special = input.ReadInt32();
		orHints = input.ReadUInt32();
		int version = input.Version;
		if (version >= 39)
		{
			hasSkeleton = input.ReadBoolean();
		}
		if (version >= 51)
		{
			nVertices = input.ReadInt32();
			faceArea = input.ReadSingle();
		}
	}
}
