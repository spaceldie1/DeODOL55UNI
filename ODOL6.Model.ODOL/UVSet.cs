using System;
using ODOL6.Common;
using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class UVSet
{
	private bool isDiscretized;

	private float minU;

	private float minV;

	private float maxU;

	private float maxV;

	private uint nVertices;

	private bool defaultFill;

	private byte[] defaultValue;

	private byte[] uvData;

	public float[] UVData
	{
		get
		{
			float[] array = new float[nVertices * 2];
			float num = 0f;
			float num2 = 0f;
			double scale = 1.0;
			double scale2 = 1.0;
			if (isDiscretized)
			{
				scale = maxU - minU;
				scale2 = maxV - minV;
			}
			if (defaultFill)
			{
				if (isDiscretized)
				{
					num = Scale(BitConverter.ToInt16(defaultValue, 0), scale, minU);
					num2 = Scale(BitConverter.ToInt16(defaultValue, 2), scale2, minV);
				}
				else
				{
					num = BitConverter.ToSingle(defaultValue, 0);
					num2 = BitConverter.ToSingle(defaultValue, 4);
				}
			}
			for (int i = 0; i < nVertices; i++)
			{
				if (isDiscretized)
				{
					array[i * 2] = (defaultFill ? num : Scale(BitConverter.ToInt16(uvData, i * 4), scale, minU));
					array[i * 2 + 1] = (defaultFill ? num2 : Scale(BitConverter.ToInt16(uvData, i * 4 + 2), scale2, minV));
				}
				else
				{
					array[i * 2] = (defaultFill ? num : BitConverter.ToSingle(uvData, i * 8));
					array[i * 2 + 1] = (defaultFill ? num2 : BitConverter.ToSingle(uvData, i * 8 + 4));
				}
			}
			return array;
		}
	}

	private float Scale(short value, double scale, float min)
	{
		return (float)(1.52587890625E-05 * (double)(value + 32767) * scale) + min;
	}

	public void Read(BinaryReaderEx input, uint odolVersion)
	{
		isDiscretized = false;
		if (odolVersion >= 45)
		{
			isDiscretized = true;
			minU = input.ReadSingle();
			Logging_Functions.Echo(input, minU, "minU");
			minV = input.ReadSingle();
			Logging_Functions.Echo(input, minV, "minV");
			maxU = input.ReadSingle();
			Logging_Functions.Echo(input, maxU, "maxU");
			maxV = input.ReadSingle();
			Logging_Functions.Echo(input, maxV, "maxV");
		}
		nVertices = input.ReadUInt32();
		Logging_Functions.Echo(input, nVertices, "nVertices", hex: true);
		defaultFill = input.ReadBoolean();
		int num = ((odolVersion >= 45) ? 4 : 8);
		if (defaultFill)
		{
			defaultValue = input.ReadBytes(num);
		}
		else
		{
			uvData = input.ReadCompressed((uint)(nVertices * num));
		}
	}
}
