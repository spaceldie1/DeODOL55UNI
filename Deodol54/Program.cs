using System;
using System.IO;
using System.Linq;
using System.Text;
using ODOL5;
using ODOL6;

namespace Deodol54;

internal class Program
{
	public static bool extractModelCfg = true;

	private const string formattedText = "-----------------\np3d to mlod - dag and drop\nallp3ds path_to_p3ds\ncan add one bool to enable modelcfg extract\n~ExtractTool.exe allp3ds [path] true\n-----------------";

	private static void Main(string[] args)
	{
		ODOL5.Common.Logging_Functions.Verbose = false;
		ODOL6.Common.Logging_Functions.Verbose = false;
		Console.WriteLine("Program started. Args length: " + args.Length);
		foreach (string text in args)
		{
			Console.WriteLine("Arg: " + text);
		}
		if (args.Length < 1)
		{
			Console.WriteLine("-----------------\np3d to mlod - dag and drop\nallp3ds path_to_p3ds\ncan add one bool to enable modelcfg extract\n~ExtractTool.exe allp3ds [path] true\n-----------------");
			Console.ReadKey();
			return;
		}
		string text2 = args[0];
		if (args.Length == 1)
		{
			if (Path.GetExtension(text2) == ".p3d")
			{
				ConvertP3D(args[0], extractModelCfg);
			}
			else
			{
				Console.WriteLine("Unsupported file type.");
			}
		}
		string path = "";
		if (args.Length > 1)
		{
			path = args[1];
		}
		bool extractmodelcfg = extractModelCfg;
		if (args.Length > 2)
		{
			extractmodelcfg = bool.Parse(args[2]);
		}
		if (args.Length == 2 && text2 == "allp3ds")
		{
			ExtractAllP3Ds(path, extractmodelcfg);
		}
	}

	private static void ExtractAllP3Ds(string path, bool extractmodelcfg)
	{
		try
		{
			Console.WriteLine("Starting ExtractAllP3Ds for path: " + path);
			if (!Directory.Exists(path))
			{
				Console.WriteLine("Directory does not exist: " + path);
				return;
			}
			string[] files = Directory.GetFiles(path, "*.p3d", SearchOption.AllDirectories);
			if (files.Length == 0)
			{
				Console.WriteLine("No .p3d files found.");
				return;
			}
			string[] array = files.OrderByDescending((string f) => f.Count((char c) => c == Path.DirectorySeparatorChar)).ToArray();
			Console.WriteLine($"Found {array.Length} .p3d files.");
			string[] array2 = array;
			foreach (string text in array2)
			{
				Console.WriteLine("Extracting " + text);
				ConvertP3D(text, extractmodelcfg);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("An error occurred: " + ex.Message);
		}
	}

	private static void ConvertP3D(string filePath, bool extractmodelcfg)
	{
		byte[] array = new byte[5];
		using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
		{
			fileStream.Read(array, 0, 5);
		}
		if (Encoding.ASCII.GetString(array, 0, 4) == "ODOL")
		{
			int num = array[4];
			Console.WriteLine("file version: " + num);
			if (num <= 53)
			{
				ODOL5.Methods.Convert5P3D(filePath, extractmodelcfg, extractmaterial: false);
			}
			else
			{
				ODOL6.Methods.Convert6P3D(filePath, extractmodelcfg, extractmaterial: false);
			}
		}
		else
		{
			Console.WriteLine("Not a valid debin target");
		}
	}
}
