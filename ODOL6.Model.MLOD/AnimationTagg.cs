using ODOL6.Common.Math;
using ODOL6.Stream;

namespace ODOL6.Model.MLOD;

public class AnimationTagg : Tagg
{
	public float frameTime;

	public Vector3P[] framePoints;

	public void read(BinaryReaderEx input)
	{
		uint num = (base.DataSize - 4) / 12;
		frameTime = input.ReadSingle();
		framePoints = new Vector3P[num];
		for (int i = 0; i < num; i++)
		{
			framePoints[i] = new Vector3P(input);
		}
	}

	public void write(BinaryWriterEx output)
	{
		output.Write(value: true);
		output.writeAsciiz(base.Name);
		output.Write(base.DataSize);
		output.Write(frameTime);
		for (int i = 0; i < framePoints.Length; i++)
		{
			framePoints[i].Write(output);
		}
	}
}
