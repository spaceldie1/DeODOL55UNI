using ODOL5.Stream;

namespace ODOL5.Model.MLOD;

public class MassTagg : Tagg
{
	public float[] mass;

	public void Read(BinaryReaderEx input)
	{
		uint num = base.DataSize / 4;
		mass = new float[num];
		for (int i = 0; i < num; i++)
		{
			mass[i] = input.ReadSingle();
		}
	}

	public void Write(BinaryWriterEx output)
	{
		output.Write(value: true);
		output.writeAsciiz(base.Name);
		output.Write(base.DataSize);
		uint num = base.DataSize / 4;
		for (int i = 0; i < num; i++)
		{
			output.Write(mass[i]);
		}
	}
}
