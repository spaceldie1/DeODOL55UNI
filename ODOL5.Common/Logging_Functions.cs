using System;
using ODOL5.Stream;

namespace ODOL5.Common;

public static class Logging_Functions
{
	public static bool Verbose = true;

	public static void Echo(string message)
	{
		if (Verbose)
		{
			Console.WriteLine(message);
		}
	}

	public static void Echo(BinaryReaderEx input, int variable, string name, bool hex)
	{
		if (Verbose)
		{
			string text = name + ": " + variable + " Position: " + input.Position;
			if (hex)
			{
				text = text + " Hex: " + variable.ToString("X4");
			}
			Console.WriteLine(text);
		}
	}

	public static void Echo(BinaryReaderEx input, uint variable, string name, bool hex)
	{
		if (Verbose)
		{
			string text = name + ": " + variable + " Position: " + input.Position;
			if (hex)
			{
				text = text + " Hex: " + variable.ToString("X4");
			}
			Console.WriteLine(text);
		}
	}

	public static void Echo<T>(BinaryReaderEx input, T variable, string name)
	{
		if (Verbose)
		{
			string[] obj = new string[5] { name, ": ", null, null, null };
			int num = 2;
			T val = variable;
			obj[num] = val?.ToString();
			obj[3] = " Position: ";
			obj[4] = input.Position.ToString();
			Console.WriteLine(string.Concat(obj));
		}
	}
}
