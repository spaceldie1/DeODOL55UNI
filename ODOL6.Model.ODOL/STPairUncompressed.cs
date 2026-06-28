using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class STPairUncompressed : STPair, IDeserializable
{
	public void ReadObject(BinaryReaderEx input)
	{
		base.S.ReadObject(input);
		base.T.ReadObject(input);
	}
}
