using ODOL5.Stream;

namespace ODOL5.Model.MLOD;

public class UVSetTagg : Tagg
{
	public uint uvSetNr;

	public float[][,] faceUVs;

	public void Read(BinaryReaderEx input, Face[] faces)
	{
		uvSetNr = input.ReadUInt32();
		faceUVs = new float[faces.Length][,];
		for (int i = 0; i < faces.Length; i++)
		{
			faceUVs[i] = new float[faces[i].NumberOfVertices, 2];
			for (int j = 0; j < faces[i].NumberOfVertices; j++)
			{
				faceUVs[i][j, 0] = input.ReadSingle();
				faceUVs[i][j, 1] = input.ReadSingle();
			}
		}
	}

	public void Write(BinaryWriterEx output)
	{
		output.Write(value: true);
		output.writeAsciiz(base.Name);
		output.Write(base.DataSize);
		output.Write(uvSetNr);
		for (int i = 0; i < faceUVs.Length; i++)
		{
			for (int j = 0; j < faceUVs[i].Length / 2; j++)
			{
				output.Write(faceUVs[i][j, 0]);
				output.Write(faceUVs[i][j, 1]);
			}
		}
	}
}
