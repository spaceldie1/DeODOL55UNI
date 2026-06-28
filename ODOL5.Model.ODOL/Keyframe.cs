using ODOL5.Common.Math;
using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class Keyframe : IDeserializable
{
	public float time;

	public Vector3P[] points;

	public void ReadObject(BinaryReaderEx input)
	{
		time = input.ReadSingle();
		uint num = input.ReadUInt32();
		points = new Vector3P[num];
		for (int i = 0; i < num; i++)
		{
			points[i] = new Vector3P(input);
		}
	}
}
