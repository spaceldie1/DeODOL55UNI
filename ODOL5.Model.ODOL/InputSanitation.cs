using System.Text;

namespace ODOL5.Model.ODOL;

public class InputSanitation
{
	private static bool IsAscii(byte[] bytes)
	{
		Decoder decoder = Encoding.ASCII.GetDecoder();
		try
		{
			if (decoder.GetCharCount(bytes, 0, bytes.Length) == bytes.Length)
			{
				return true;
			}
		}
		catch (DecoderFallbackException)
		{
			return false;
		}
		return false;
	}

	private static string ConvertBytesToAscii(byte[] bytes)
	{
		Decoder decoder = Encoding.ASCII.GetDecoder();
		char[] array = new char[decoder.GetCharCount(bytes, 0, bytes.Length)];
		decoder.GetChars(bytes, 0, bytes.Length, array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == '\ufffd')
			{
				array[i] = '?';
			}
		}
		return new string(array);
	}
}
