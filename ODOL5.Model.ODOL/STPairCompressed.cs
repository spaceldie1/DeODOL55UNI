using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class STPairCompressed : STPair, IDeserializable
{
	public void ReadObject(BinaryReaderEx input)
	{
		base.S.ReadCompressed(input);
		base.T.ReadCompressed(input);
	}
}
