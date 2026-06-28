using ODOL5.Common.Math;
using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class Proxy : IDeserializable
{
	public string proxyModel;

	public Matrix4P transformation;

	public int sequenceID;

	public int namedSelectionIndex;

	public int boneIndex;

	public int sectionIndex;

	public void ReadObject(BinaryReaderEx input)
	{
		proxyModel = input.ReadAsciiz();
		transformation = new Matrix4P(input);
		sequenceID = input.ReadInt32();
		namedSelectionIndex = input.ReadInt32();
		boneIndex = input.ReadInt32();
		if (input.Version >= 40)
		{
			sectionIndex = input.ReadInt32();
		}
	}
}
