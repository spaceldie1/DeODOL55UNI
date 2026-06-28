using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class Polygons
{
	public Polygon[] Faces { get; private set; }

	public Polygons(BinaryReaderEx input)
	{
		uint num = input.ReadUInt32();
		input.ReadUInt32();
		input.ReadUInt16();
		Faces = new Polygon[num];
		for (int i = 0; i < num; i++)
		{
			Faces[i] = new Polygon();
			Faces[i].ReadObject(input);
		}
	}
}
