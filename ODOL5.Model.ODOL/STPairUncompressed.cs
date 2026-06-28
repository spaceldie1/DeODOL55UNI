using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class STPairUncompressed : STPair, IDeserializable
{
	public void ReadObject(BinaryReaderEx input)
	{
		base.S.ReadObject(input);
		base.T.ReadObject(input);
	}
}
