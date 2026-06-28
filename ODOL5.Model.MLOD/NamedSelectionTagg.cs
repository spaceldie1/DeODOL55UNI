using ODOL5.Stream;

namespace ODOL5.Model.MLOD;

public class NamedSelectionTagg : Tagg
{
	public byte[] points;

	public byte[] faces;

	public void Read(BinaryReaderEx input, int nPoints, int nFaces)
	{
		points = new byte[nPoints];
		for (int i = 0; i < nPoints; i++)
		{
			points[i] = input.ReadByte();
		}
		faces = new byte[nFaces];
		for (int j = 0; j < nFaces; j++)
		{
			faces[j] = input.ReadByte();
		}
	}

	public void Write(BinaryWriterEx output)
	{
		output.Write(value: true);
		output.writeAsciiz(base.Name);
		output.Write(base.DataSize);
		for (int i = 0; i < points.Length; i++)
		{
			output.Write(points[i]);
		}
		for (int j = 0; j < faces.Length; j++)
		{
			output.Write(faces[j]);
		}
	}
}
