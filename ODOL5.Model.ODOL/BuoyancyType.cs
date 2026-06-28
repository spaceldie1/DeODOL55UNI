using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class BuoyancyType : IDeserializable
{
	public float Volume { get; private set; }

	public void ReadObject(BinaryReaderEx input)
	{
		Volume = input.ReadSingle();
	}
}
