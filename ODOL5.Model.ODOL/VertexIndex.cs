namespace ODOL5.Model.ODOL;

public struct VertexIndex
{
	public int value;

	public static implicit operator int(VertexIndex vi)
	{
		return vi.value;
	}

	public static implicit operator VertexIndex(ushort vi)
	{
		return new VertexIndex
		{
			value = ((vi == ushort.MaxValue) ? (-1) : vi)
		};
	}

	public static implicit operator VertexIndex(int vi)
	{
		return new VertexIndex
		{
			value = vi
		};
	}
}
