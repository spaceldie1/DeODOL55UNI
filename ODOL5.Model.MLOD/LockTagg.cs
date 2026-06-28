using ODOL5.Stream;

namespace ODOL5.Model.MLOD;

public class LockTagg : Tagg
{
	public bool[] lockedPoints;

	public bool[] lockedFaces;

	public void Read(BinaryReaderEx input, int nPoints, int nFaces)
	{
		lockedPoints = new bool[nPoints];
		for (int i = 0; i < nPoints; i++)
		{
			lockedPoints[i] = input.ReadBoolean();
		}
		lockedFaces = new bool[nFaces];
		for (int j = 0; j < nFaces; j++)
		{
			lockedFaces[j] = input.ReadBoolean();
		}
	}

	public void Write(BinaryWriterEx output)
	{
		output.Write(value: true);
		output.writeAsciiz(base.Name);
		output.Write(base.DataSize);
		for (int i = 0; i < lockedPoints.Length; i++)
		{
			output.Write(lockedPoints[i]);
		}
		for (int j = 0; j < lockedFaces.Length; j++)
		{
			output.Write(lockedFaces[j]);
		}
	}
}
