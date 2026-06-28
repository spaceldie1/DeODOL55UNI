using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class VertexNeighborInfo : IDeserializable
{
	public ushort PosA { get; private set; }

	public AnimationRTWeight RtwA { get; private set; }

	public ushort PosB { get; private set; }

	public AnimationRTWeight RtwB { get; private set; }

	public void ReadObject(BinaryReaderEx input)
	{
		PosA = input.ReadUInt16();
		input.ReadBytes(2);
		RtwA = new AnimationRTWeight();
		RtwA.ReadObject(input);
		PosB = input.ReadUInt16();
		input.ReadBytes(2);
		RtwB = new AnimationRTWeight();
		RtwB.ReadObject(input);
	}
}
