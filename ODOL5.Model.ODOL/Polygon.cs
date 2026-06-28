using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class Polygon : IDeserializable
{
	public VertexIndex[] VertexIndices { get; private set; }

	public void ReadObject(BinaryReaderEx input)
	{
		_ = input.Version;
		byte b = input.ReadByte();
		VertexIndices = new VertexIndex[b];
		for (int i = 0; i < b; i++)
		{
			VertexIndices[i] = input.ReadVertexIndex();
		}
	}
}
