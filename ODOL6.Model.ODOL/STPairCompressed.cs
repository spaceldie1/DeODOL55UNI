using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class STPairCompressed : STPair, IDeserializable
{
	public void ReadObject(BinaryReaderEx input)
	{
		base.S.ReadCompressed(input);
		base.T.ReadCompressed(input);
	}
}
