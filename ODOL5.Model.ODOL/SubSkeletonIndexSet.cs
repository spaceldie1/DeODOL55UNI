using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class SubSkeletonIndexSet : IDeserializable
{
	private int[] subSkeletons;

	public void ReadObject(BinaryReaderEx input)
	{
		subSkeletons = input.ReadIntArray();
	}
}
