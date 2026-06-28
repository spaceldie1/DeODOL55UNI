using System;

namespace ODOL5.Common.Math;

public class Quaternion
{
	private float x;

	private float y;

	private float z;

	private float w;

	public float X => x;

	public float Y => x;

	public float Z => x;

	public float W => x;

	public Quaternion Inverse
	{
		get
		{
			normalize();
			return Conjugate;
		}
	}

	public Quaternion Conjugate => new Quaternion(0f - x, 0f - y, 0f - z, w);

	public static Quaternion readCompressed(byte[] data)
	{
		float num = (float)((0.0 - (double)BitConverter.ToInt16(data, 0)) / 16384.0);
		float num2 = (float)((double)BitConverter.ToInt16(data, 2) / 16384.0);
		float num3 = (float)((0.0 - (double)BitConverter.ToInt16(data, 4)) / 16384.0);
		float num4 = (float)((double)BitConverter.ToInt16(data, 6) / 16384.0);
		return new Quaternion(num, num2, num3, num4);
	}

	public Quaternion()
	{
		w = 1f;
		x = 0f;
		y = 0f;
		z = 0f;
	}

	public Quaternion(float x, float y, float z, float w)
	{
		this.w = w;
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static Quaternion operator *(Quaternion a, Quaternion b)
	{
		float num = a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z;
		float num2 = a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y;
		float num3 = a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x;
		float num4 = a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w;
		return new Quaternion(num2, num3, num4, num);
	}

	public void normalize()
	{
		float num = (float)(1.0 / System.Math.Sqrt(x * x + y * y + z * z + w * w));
		x *= num;
		y *= num;
		z *= num;
		w *= num;
	}

	public Vector3P transform(Vector3P xyz)
	{
		Quaternion quaternion = new Quaternion(xyz.X, xyz.Y, xyz.Z, 0f);
		Quaternion quaternion2 = this * quaternion * Inverse;
		return new Vector3P(quaternion2.x, quaternion2.y, quaternion2.z);
	}

	public float[,] asRotationMatrix()
	{
		float[,] array = new float[3, 3];
		double num = x * y;
		double num2 = w * z;
		double num3 = w * x;
		double num4 = w * y;
		double num5 = x * z;
		double num6 = y * z;
		double num7 = z * z;
		double num8 = y * y;
		double num9 = x * x;
		array[0, 0] = (float)(1.0 - 2.0 * (num8 + num7));
		array[0, 1] = (float)(2.0 * (num - num2));
		array[0, 2] = (float)(2.0 * (num5 + num4));
		array[1, 0] = (float)(2.0 * (num + num2));
		array[1, 1] = (float)(1.0 - 2.0 * (num9 + num7));
		array[1, 2] = (float)(2.0 * (num6 - num3));
		array[2, 0] = (float)(2.0 * (num5 - num4));
		array[2, 1] = (float)(2.0 * (num6 + num3));
		array[2, 2] = (float)(1.0 - 2.0 * (num9 + num8));
		return array;
	}
}
