using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class StageTexture
{
	public enum TextureFilterType
	{
		Point,
		Linear,
		Triliniear,
		Anisotropic
	}

	public TextureFilterType textureFilter;

	public string texture;

	public uint stageID;

	public bool useWorldEnvMap;

	public void read(BinaryReaderEx input, uint matVersion)
	{
		if (matVersion >= 5)
		{
			textureFilter = (TextureFilterType)input.ReadUInt32();
		}
		texture = input.ReadAsciiz();
		if (matVersion >= 8)
		{
			stageID = input.ReadUInt32();
		}
		if (matVersion >= 11)
		{
			useWorldEnvMap = input.ReadBoolean();
		}
	}
}
