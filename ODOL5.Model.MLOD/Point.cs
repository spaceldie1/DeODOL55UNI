using ODOL5.Common.Math;
using ODOL5.Stream;

namespace ODOL5.Model.MLOD;

public class Point : Vector3P
{
	public PointFlags PointFlags { get; private set; }

	public Point(Vector3P pos, PointFlags flags)
		: base(pos.X, pos.Y, pos.Z)
	{
		PointFlags = flags;
	}

	public Point(BinaryReaderEx input)
		: base(input)
	{
		PointFlags = (PointFlags)input.ReadUInt32();
	}

	public new void Write(BinaryWriterEx output)
	{
		base.Write(output);
		output.Write((uint)PointFlags);
	}
}
