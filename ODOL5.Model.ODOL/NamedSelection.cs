using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class NamedSelection : IDeserializable
{
	public string Name { get; private set; }

	public bool IsSectional { get; private set; }

	public VertexIndex[] SelectedFaces { get; private set; }

	public int[] Sections { get; private set; }

	public byte[] SelectedVerticesWeights { get; private set; }

	public VertexIndex[] SelectedVertices { get; private set; }

	public void ReadObject(BinaryReaderEx input)
	{
		Name = input.ReadAsciiz();
		SelectedFaces = input.ReadCompressedVertexIndexArray();
		input.ReadInt32();
		IsSectional = input.ReadBoolean();
		Sections = input.ReadCompressedIntArray();
		SelectedVertices = input.ReadCompressedVertexIndexArray();
		int expectedSize = input.ReadInt32();
		SelectedVerticesWeights = input.ReadCompressed((uint)expectedSize);
	}
}
