using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class BuoyancyTypeSpheres : BuoyancyType, IDeserializable
{
	public int ArraySizeX { get; private set; }

	public int ArraySizeY { get; private set; }

	public int ArraySizeZ { get; private set; }

	public float StepX { get; private set; }

	public float StepY { get; private set; }

	public float StepZ { get; private set; }

	public float FullSphereRadius { get; private set; }

	public int MinSpheres { get; private set; }

	public int MaxSpheres { get; private set; }

	public BuoyantPoint[] BuoyancyPoints { get; private set; }

	public new void ReadObject(BinaryReaderEx input)
	{
		ArraySizeX = input.ReadInt32();
		ArraySizeY = input.ReadInt32();
		ArraySizeZ = input.ReadInt32();
		StepX = input.ReadSingle();
		StepY = input.ReadSingle();
		StepZ = input.ReadSingle();
		FullSphereRadius = input.ReadSingle();
		MinSpheres = input.ReadInt32();
		MaxSpheres = input.ReadInt32();
		int num = ArraySizeX * ArraySizeY * ArraySizeZ;
		BuoyancyPoints = new BuoyantPoint[num];
		for (int i = 0; i < num; i++)
		{
			BuoyancyPoints[i] = new BuoyantPoint();
			BuoyancyPoints[i].ReadObject(input);
		}
		base.ReadObject(input);
	}
}
