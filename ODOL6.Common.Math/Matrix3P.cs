using ODOL6.Stream;

namespace ODOL6.Common.Math;

public class Matrix3P
{
	private Vector3P[] columns;

	public Vector3P Aside => columns[0];

	public Vector3P Up => columns[1];

	public Vector3P Dir => columns[2];

	public Vector3P this[int col] => columns[col];

	public float this[int row, int col]
	{
		get
		{
			return this[col][row];
		}
		set
		{
			this[col][row] = value;
		}
	}

	public Matrix3P()
		: this(0f)
	{
	}

	public Matrix3P(float val)
		: this(new Vector3P(val), new Vector3P(val), new Vector3P(val))
	{
	}

	public Matrix3P(BinaryReaderEx input)
		: this(new Vector3P(input), new Vector3P(input), new Vector3P(input))
	{
	}

	private Matrix3P(Vector3P aside, Vector3P up, Vector3P dir)
	{
		columns = new Vector3P[3] { aside, up, dir };
	}

	public static Matrix3P operator -(Matrix3P a)
	{
		return new Matrix3P(-a.Aside, -a.Up, -a.Dir);
	}

	public static Matrix3P operator *(Matrix3P a, Matrix3P b)
	{
		Matrix3P matrix3P = new Matrix3P();
		float num = b[0, 0];
		float num2 = b[1, 0];
		float num3 = b[2, 0];
		matrix3P[0, 0] = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3;
		matrix3P[1, 0] = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3;
		matrix3P[2, 0] = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3;
		num = b[0, 1];
		num2 = b[1, 1];
		num3 = b[2, 1];
		matrix3P[0, 1] = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3;
		matrix3P[1, 1] = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3;
		matrix3P[2, 1] = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3;
		num = b[0, 2];
		num2 = b[1, 2];
		num3 = b[2, 2];
		matrix3P[0, 2] = a[0, 0] * num + a[0, 1] * num2 + a[0, 2] * num3;
		matrix3P[1, 2] = a[1, 0] * num + a[1, 1] * num2 + a[1, 2] * num3;
		matrix3P[2, 2] = a[2, 0] * num + a[2, 1] * num2 + a[2, 2] * num3;
		return matrix3P;
	}

	public void setTilda(Vector3P a)
	{
		Aside.Y = 0f - a.Z;
		Aside.Z = a.Y;
		Up.X = a.Z;
		Up.Z = 0f - a.X;
		Dir.X = 0f - a.Y;
		Dir.Y = a.X;
	}

	public void write(BinaryWriterEx output)
	{
		Aside.Write(output);
		Up.Write(output);
		Dir.Write(output);
	}
}
