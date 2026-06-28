using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class SubSkeletonIndexSet : IDeserializable
{
	private int[] subSkeletons;

	public void ReadObject(BinaryReaderEx input)
	{
		subSkeletons = input.ReadIntArray();
	}
}
