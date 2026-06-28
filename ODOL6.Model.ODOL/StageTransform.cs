using ODOL6.Common.Math;
using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class StageTransform
{
	public enum UVSource
	{
		UVNone,
		UVTex,
		UVTexWaterAnim,
		UVPos,
		UVNorm,
		UVTex1,
		UVWorldPos,
		UVWorldNorm,
		UVTexShoreAnim,
		NUVSource
	}

	public UVSource uvSource;

	public Matrix4P transformation;

	public StageTransform(BinaryReaderEx input)
	{
		uvSource = (UVSource)input.ReadUInt32();
		transformation = new Matrix4P(input);
	}
}
