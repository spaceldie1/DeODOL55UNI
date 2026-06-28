using System;
using System.Globalization;
using ODOL5.Stream;

namespace ODOL5.Common.Math;

public class Vector3P : IDeserializable
{
	private float[] xyz;

	public float X
	{
		get
		{
			return xyz[0];
		}
		set
		{
			xyz[0] = value;
		}
	}

	public float Y
	{
		get
		{
			return xyz[1];
		}
		set
		{
			xyz[1] = value;
		}
	}

	public float Z
	{
		get
		{
			return xyz[2];
		}
		set
		{
			xyz[2] = value;
		}
	}

	public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z);

	public float this[int i]
	{
		get
		{
			return xyz[i];
		}
		set
		{
			xyz[i] = value;
		}
	}

	public Vector3P()
		: this(0f)
	{
	}

	public Vector3P(float val)
		: this(val, val, val)
	{
	}

	public Vector3P(BinaryReaderEx input)
		: this(input.ReadSingle(), input.ReadSingle(), input.ReadSingle())
	{
	}

	public Vector3P(float x, float y, float z)
	{
		xyz = new float[3] { x, y, z };
	}

	public static Vector3P operator -(Vector3P a)
	{
		return new Vector3P(0f - a.X, 0f - a.Y, 0f - a.Z);
	}

	public void ReadCompressed(BinaryReaderEx input)
	{
		int num = input.ReadInt32();
		int num2 = num & 0x3FF;
		int num3 = (num >> 10) & 0x3FF;
		int num4 = (num >> 20) & 0x3FF;
		if (num2 > 511)
		{
			num2 -= 1024;
		}
		if (num3 > 511)
		{
			num3 -= 1024;
		}
		if (num4 > 511)
		{
			num4 -= 1024;
		}
		X = (float)((double)num2 * (-1.0 / 511.0));
		Y = (float)((double)num3 * (-1.0 / 511.0));
		Z = (float)((double)num4 * (-1.0 / 511.0));
	}

	public void Write(BinaryWriterEx output)
	{
		output.Write(X);
		output.Write(Y);
		output.Write(Z);
	}

	public static Vector3P operator *(Vector3P a, float b)
	{
		return new Vector3P(a.X * b, a.Y * b, a.Z * b);
	}

	public static float operator *(Vector3P a, Vector3P b)
	{
		return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
	}

	public static Vector3P operator +(Vector3P a, Vector3P b)
	{
		return new Vector3P(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}

	public static Vector3P operator -(Vector3P a, Vector3P b)
	{
		return new Vector3P(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}

	public override bool Equals(object obj)
	{
		if (obj is Vector3P other && base.Equals(obj))
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public bool Equals(Vector3P other)
	{
		Func<float, float, bool> func = (float f1, float f2) => (double)System.Math.Abs(f1 - f2) < 0.05;
		if (func(X, other.X) && func(Y, other.Y))
		{
			return func(Z, other.Z);
		}
		return false;
	}

	public override string ToString()
	{
		CultureInfo cultureInfo = new CultureInfo("en-GB");
		return "{" + X.ToString(cultureInfo.NumberFormat) + "," + Y.ToString(cultureInfo.NumberFormat) + "," + Z.ToString(cultureInfo.NumberFormat) + "}";
	}

	public void ReadObject(BinaryReaderEx input)
	{
		xyz[0] = input.ReadSingle();
		xyz[1] = input.ReadSingle();
		xyz[2] = input.ReadSingle();
	}

	public float Distance(Vector3P v)
	{
		Vector3P vector3P = this - v;
		return (float)System.Math.Sqrt(vector3P.X * vector3P.X + vector3P.Y * vector3P.Y + vector3P.Z * vector3P.Z);
	}

	public void Normalize()
	{
		float num = (float)Length;
		X /= num;
		Y /= num;
		Z /= num;
	}
}
