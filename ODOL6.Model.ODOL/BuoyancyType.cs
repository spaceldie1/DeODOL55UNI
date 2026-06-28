using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class BuoyancyType : IDeserializable
{
	public float Volume { get; private set; }

	public void ReadObject(BinaryReaderEx input)
	{
		Volume = input.ReadSingle();
	}
}
