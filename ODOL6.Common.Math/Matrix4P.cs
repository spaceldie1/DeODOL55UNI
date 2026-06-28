using ODOL6.Stream;

namespace ODOL6.Common.Math;

public class Matrix4P
{
	private Matrix3P orientation;

	private Vector3P position;

	public Matrix3P Orientation => orientation;

	public Vector3P Position => position;

	public float this[int row, int col]
	{
		get
		{
			if (col != 3)
			{
				return orientation[col][row];
			}
			return position[row];
		}
		set
		{
			if (col == 3)
			{
				position[row] = value;
			}
			else
			{
				orientation[col][row] = value;
			}
		}
	}

	public Matrix4P()
		: this(0f)
	{
	}

	public Matrix4P(float val)
		: this(new Matrix3P(val), new Vector3P(val))
	{
	}

	public Matrix4P(BinaryReaderEx input)
		: this(new Matrix3P(input), new Vector3P(input))
	{
	}

	private Matrix4P(Matrix3P orientation, Vector3P position)
	{
		this.orientation = orientation;
		this.position = position;
	}

	public static Matrix4P operator *(Matrix4P a, Matrix4P b)
	{
		Matrix4P matrix4P = new Matrix4P();
		float num = b[0, 0];
		float num2 = b[1, 0];
		float num3 = b[2, 0];
		matrix4P[0, 0] = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3;
		matrix4P[1, 0] = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3;
		matrix4P[2, 0] = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3;
		num = b[0, 1];
		num2 = b[1, 1];
		num3 = b[2, 1];
		matrix4P[0, 1] = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3;
		matrix4P[1, 1] = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3;
		matrix4P[2, 1] = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3;
		num = b[0, 2];
		num2 = b[1, 2];
		num3 = b[2, 2];
		matrix4P[0, 2] = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3;
		matrix4P[1, 2] = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3;
		matrix4P[2, 2] = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3;
		num = b.Position.X;
		num2 = b.Position.Y;
		num3 = b.Position.Z;
		matrix4P.Position.X = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3 + a.Position.X;
		matrix4P.Position.Y = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3 + a.Position.Y;
		matrix4P.Position.Z = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3 + a.Position.Z;
		return matrix4P;
	}

	public void write(BinaryWriterEx output)
	{
		orientation.write(output);
		position.Write(output);
	}
}
