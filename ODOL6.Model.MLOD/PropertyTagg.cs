using ODOL6.Stream;

namespace ODOL6.Model.MLOD;

public class PropertyTagg : Tagg
{
	public string name;

	public string value;

	public void Read(BinaryReaderEx input)
	{
		name = input.ReadAscii(64);
		value = input.ReadAscii(64);
	}

	public void Write(BinaryWriterEx output)
	{
		output.Write(value: true);
		output.writeAsciiz(base.Name);
		output.Write(base.DataSize);
		output.writeAscii(name, 64u);
		output.writeAscii(value, 64u);
	}
}
