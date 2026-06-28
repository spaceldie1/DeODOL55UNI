using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

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
		input.ReadUInt32();
		IsSectional = input.ReadBoolean();
		Sections = input.ReadCompressedIntArray();
		SelectedVertices = input.ReadCompressedVertexIndexArray();
		uint expectedSize = input.ReadUInt32();
		SelectedVerticesWeights = input.ReadCompressed(expectedSize);
	}
}
