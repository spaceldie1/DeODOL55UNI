using System;
using System.Collections.Generic;
using System.Linq;
using ODOL6.Common;
using ODOL6.Common.Math;
using ODOL6.Stream;

namespace ODOL6.Model.ODOL;

public class LOD : P3D_LOD, IComparable<LOD>
{
	private static readonly bool TraceEnabled = Environment.GetEnvironmentVariable("DEODOL_TRACE") == "1";

	private struct PointWeight
	{
		public int pointIndex;

		public byte weight;

		public PointWeight(int index, byte weight)
		{
			pointIndex = index;
			this.weight = weight;
		}
	}

	private uint odolVersion;

	private Proxy[] proxies;

	private int[] subSkeletonsToSkeleton;

	private SubSkeletonIndexSet[] skeletonToSubSkeleton;

	private uint vertexCount;

	private float faceArea;

	private ClipFlags[] clipOldFormat;

	private ClipFlags[] clip;

	private ClipFlags orHints;

	private ClipFlags andHints;

	private Vector3P bMin;

	private Vector3P bMax;

	private Vector3P bCenter;

	private float bRadius;

	private string[] textures;

	private VertexIndex[] pointToVertex;

	private VertexIndex[] vertexToPoint;

	private Polygons polygons;

	private Section[] sections;

	private NamedSelection[] namedSelections;

	private uint nNamedProperties;

	private string[,] namedProperties;

	private Keyframe[] frames;

	private int colorTop;

	private int color;

	private int special;

	private bool vertexBoneRefIsSimple;

	private uint sizeOfRestData;

	private uint nUVSets;

	private UVSet[] uvSets;

	private Vector3P[] vertices;

	private Vector3P[] normals;

	private STPair[] STCoords;

	private AnimationRTWeight[] vertexBoneRef;

	private VertexNeighborInfo[] neighborBoneRef;

	public NamedSelection[] NamedSelections => namedSelections;

	public override string[] MaterialNames => Materials.Select((EmbeddedMaterial m) => m.materialName).ToArray();

	internal EmbeddedMaterial[]? Materials { get; private set; }

	public int VertexCount => vertices.Length;

	public int SectionCount => sections.Length;

	public int TextureCount => textures.Length;

	public int PolygonCount => polygons.Faces.Length;

	public int MaterialCount => Materials.Length;

	public AnimationRTWeight[] VertexBoneRef => vertexBoneRef;

	public VertexNeighborInfo[] NeighborBoneRef => neighborBoneRef;

	public ClipFlags[] ClipFlags
	{
		get
		{
			if (odolVersion < 50)
			{
				return clipOldFormat;
			}
			return clip;
		}
	}

	public Vector3P[] Vertices => vertices;

	public override Vector3P[] Normals => normals;

	public Section[] Sections => sections;

	public UVSet[] UVSets => uvSets;

	public Polygon[] Faces => polygons.Faces;

	public string[,] NamedProperties => namedProperties;

	public Keyframe[] Frames => frames;

	public int[] SubSkeletonsToSkeleton => subSkeletonsToSkeleton;

	public Proxy[] Proxies => proxies;

	public override Vector3P[] Points => Vertices;

	public override string[] Textures => textures;

	private static string? PeekAsciiZ(BinaryReaderEx input, long position, int maxLength = 260)
	{
		if (position < 0 || position >= input.BaseStream.Length)
		{
			return null;
		}
		long position2 = input.Position;
		try
		{
			input.Position = position;
			List<char> list = new List<char>(Math.Min(maxLength, 64));
			for (int i = 0; i < maxLength && input.Position < input.BaseStream.Length; i++)
			{
				byte b = input.ReadByte();
				if (b == 0)
				{
					return new string(list.ToArray());
				}
				char c = BinaryReaderEx.ReadSanitised(b);
				if (char.IsControl(c))
				{
					return null;
				}
				list.Add(c);
			}
			return null;
		}
		finally
		{
			input.Position = position2;
		}
	}

	private static bool LooksLikeMaterialStart(BinaryReaderEx input, long position)
	{
		string text = PeekAsciiZ(input, position);
		if (string.IsNullOrWhiteSpace(text) || text.Length < 6)
		{
			return false;
		}
		if (!text.EndsWith(".rvmat", StringComparison.OrdinalIgnoreCase) && !text.Contains("\\", StringComparison.Ordinal))
		{
			return false;
		}
		long position2 = input.Position;
		try
		{
			input.Position = position + text.Length + 1;
			uint num = input.ReadUInt32();
			return num > 0 && num <= 32;
		}
		catch
		{
			return false;
		}
		finally
		{
			input.Position = position2;
		}
	}

	private EmbeddedMaterial[] ReadMaterialsWithRecovery(BinaryReaderEx input)
	{
		int num = input.ReadInt32();
		EmbeddedMaterial[] array = new EmbeddedMaterial[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = new EmbeddedMaterial();
			array[i].ReadObject(input);
			if (i == num - 1 || LooksLikeMaterialStart(input, input.Position))
			{
				continue;
			}
			for (int j = 1; j <= 96 && input.Position + j < input.BaseStream.Length; j++)
			{
				if (LooksLikeMaterialStart(input, input.Position + j))
				{
					Logging_Functions.Echo($"Recovered material alignment: skipping {j} byte(s) before material {i + 1}.");
					input.Position += j;
					break;
				}
			}
		}
		return array;
	}

	private static uint? PeekUInt32(BinaryReaderEx input, long position)
	{
		if (position < 0 || position + 4 > input.BaseStream.Length)
		{
			return null;
		}
		long position2 = input.Position;
		try
		{
			input.Position = position;
			return input.ReadUInt32();
		}
		catch
		{
			return null;
		}
		finally
		{
			input.Position = position2;
		}
	}

	private bool IsPlausibleVertexIndexArray(VertexIndex[]? values)
	{
		if (values == null || values.Length == 0)
		{
			return false;
		}
		if (vertexCount > 0 && values.Length > vertexCount * 8)
		{
			return false;
		}
		int num = (vertexCount > 0) ? ((int)vertexCount * 4) : int.MaxValue;
		int num2 = Math.Min(values.Length, 64);
		for (int i = 0; i < num2; i++)
		{
			if (values[i].value < -1 || values[i].value > num)
			{
				return false;
			}
		}
		return true;
	}

	private VertexIndex[]? TryReadVertexIndexArray(BinaryReaderEx input, long position, out long endPosition)
	{
		endPosition = position;
		uint? num = PeekUInt32(input, position);
		if (!num.HasValue || num.Value == 0)
		{
			return null;
		}
		if (vertexCount > 0 && num.Value > vertexCount * 8)
		{
			return null;
		}
		long position2 = input.Position;
		try
		{
			input.Position = position;
			VertexIndex[] array = input.ReadCompressedVertexIndexArray();
			if (!IsPlausibleVertexIndexArray(array))
			{
				return null;
			}
			endPosition = input.Position;
			return array;
		}
		catch
		{
			return null;
		}
		finally
		{
			if (endPosition == position)
			{
				input.Position = position2;
			}
		}
	}

	private static bool HasExplicitEmptyVertexIndexArrays(BinaryReaderEx input)
	{
		long position = input.Position;
		uint? num = PeekUInt32(input, position);
		uint? num2 = PeekUInt32(input, position + 4);
		return num.HasValue && num.Value == 0 && num2.HasValue && num2.Value == 0;
	}

	private bool TryReadVertexIndexArraysWithRecovery(BinaryReaderEx input, out VertexIndex[] pointValues, out VertexIndex[] vertexValues)
	{
		long position = input.Position;
		for (int i = 0; i <= 64 && position + i < input.BaseStream.Length; i++)
		{
			VertexIndex[] array = TryReadVertexIndexArray(input, position + i, out var endPosition);
			if (array == null)
			{
				continue;
			}
			for (int j = 0; j <= 64 && endPosition + j < input.BaseStream.Length; j++)
			{
				VertexIndex[] array2 = TryReadVertexIndexArray(input, endPosition + j, out var endPosition2);
				if (array2 == null)
				{
					continue;
				}
				bool flag = false;
				for (int k = 0; k <= 128 && endPosition2 + k < input.BaseStream.Length; k++)
				{
					if (LooksLikePolygonStart(input, endPosition2 + k))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				if (i > 0)
				{
					Logging_Functions.Echo($"Recovered pointToVertex alignment: skipping {i} byte(s).");
				}
				if (j > 0)
				{
					Logging_Functions.Echo($"Recovered vertexToPoint alignment: skipping {j} byte(s).");
				}
				input.Position = endPosition2;
				pointValues = array;
				vertexValues = array2;
				return true;
			}
		}
		input.Position = position;
		pointValues = Array.Empty<VertexIndex>();
		vertexValues = Array.Empty<VertexIndex>();
		return false;
	}

	private bool LooksLikeFaceStream(BinaryReaderEx input, long position, int faceCountToSample)
	{
		long position2 = input.Position;
		try
		{
			int num = (input.Version >= 69) ? 4 : 2;
			int num2 = (vertexCount > 0) ? ((int)vertexCount * 4) : int.MaxValue;
			long num3 = position;
			for (int i = 0; i < faceCountToSample; i++)
			{
				if (num3 >= input.BaseStream.Length)
				{
					return false;
				}
				input.Position = num3;
				int num4 = input.ReadByte();
				if (num4 < 3 || num4 > 8)
				{
					return false;
				}
				long num5 = num3 + 1 + (long)(num4 * num);
				if (num5 > input.BaseStream.Length)
				{
					return false;
				}
				for (int j = 0; j < num4; j++)
				{
					int num6 = ((input.Version >= 69) ? input.ReadInt32() : input.ReadUInt16());
					if (num6 < 0 || num6 > num2)
					{
						return false;
					}
				}
				num3 = num5;
			}
			return true;
		}
		catch
		{
			return false;
		}
		finally
		{
			input.Position = position2;
		}
	}

	private bool LooksLikePolygonStart(BinaryReaderEx input, long position)
	{
		uint? num = PeekUInt32(input, position);
		if (!num.HasValue || num.Value == 0 || num.Value > 200000)
		{
			return false;
		}
		if (position + 10 >= input.BaseStream.Length)
		{
			return false;
		}
		uint? num2 = PeekUInt32(input, position + 4);
		if (!num2.HasValue)
		{
			return false;
		}
		long position2 = input.Position;
		try
		{
			long num4 = input.BaseStream.Length - (position + 10);
			long num5 = ((input.Version >= 69) ? 13L : 7L) * num.Value;
			if (num5 > num4)
			{
				return false;
			}
			input.Position = position + 8;
			ushort num3 = input.ReadUInt16();
			if (num3 != 0)
			{
				return false;
			}
			if (!LooksLikeFaceStream(input, position + 10, (int)Math.Min(num.Value, 8u)))
			{
				return false;
			}
			return num2.Value <= num.Value * 64;
		}
		catch
		{
			return false;
		}
		finally
		{
			input.Position = position2;
		}
	}

	private bool TryAlignToPolygonStart(BinaryReaderEx input, int maxSkip = 256)
	{
		long position = input.Position;
		for (int i = 0; i <= maxSkip && position + i < input.BaseStream.Length; i++)
		{
			if (!LooksLikePolygonStart(input, position + i))
			{
				continue;
			}
			if (i > 0)
			{
				Logging_Functions.Echo($"Recovered polygon alignment: skipping {i} byte(s).");
			}
			input.Position = position + i;
			return true;
		}
		return false;
	}

	private bool TrySkipMissingVertexIndexArrays(BinaryReaderEx input, int maxSkip = 64)
	{
		long position = input.Position;
		for (int i = 0; i <= maxSkip && position + i < input.BaseStream.Length; i++)
		{
			if (LooksLikePolygonStart(input, position + i))
			{
				if (i > 0)
				{
					Logging_Functions.Echo($"pointToVertex/vertexToPoint are absent here; skipping {i} byte(s) to polygons.");
				}
				input.Position = position + i;
				pointToVertex = Array.Empty<VertexIndex>();
				vertexToPoint = Array.Empty<VertexIndex>();
				return true;
			}
		}
		return false;
	}

	public void Read(BinaryReaderEx input, float resolution)
	{
		void Trace(string message)
		{
			if (TraceEnabled)
			{
				Console.WriteLine("[trace] lod " + message + " @ " + input.Position);
			}
		}

		odolVersion = (uint)input.Version;
		Logging_Functions.Echo(input, odolVersion, "odolVersion");
		base.resolution = resolution;
		Logging_Functions.Echo(input, base.resolution, "resolution");
		proxies = input.ReadArray<Proxy>();
		Trace("proxies-read");
		Logging_Functions.Echo(input, proxies.Length, "proxies");
		subSkeletonsToSkeleton = input.ReadIntArray();
		Trace("subSkeletonsToSkeleton-read");
		Logging_Functions.Echo(input, subSkeletonsToSkeleton.Length, "subSkeletonsToSkeleton");
		skeletonToSubSkeleton = input.ReadArray<SubSkeletonIndexSet>();
		Trace("skeletonToSubSkeleton-read");
		Logging_Functions.Echo(input, skeletonToSubSkeleton.Length, "skeletonToSubSkeleton");
		if (odolVersion >= 50)
		{
			vertexCount = input.ReadUInt32();
			Trace("vertexCount-read");
			Logging_Functions.Echo(input, vertexCount, "vertexCount");
		}
		else
		{
			int[] array = input.ReadCondensedIntArray();
			clipOldFormat = Array.ConvertAll(array, (int item) => (ClipFlags)item);
		}
		if (odolVersion >= 51)
		{
			faceArea = input.ReadSingle();
			Trace("faceArea-read");
			Logging_Functions.Echo(input, faceArea, "faceArea");
		}
		orHints = (ClipFlags)input.ReadInt32();
		Logging_Functions.Echo(input, orHints, "orHints");
		andHints = (ClipFlags)input.ReadInt32();
		Logging_Functions.Echo(input, andHints, "andHints");
		bMin = new Vector3P(input);
		Logging_Functions.Echo(input, bMin, "bMin");
		bMax = new Vector3P(input);
		Logging_Functions.Echo(input, bMax, "bMax");
		bCenter = new Vector3P(input);
		Logging_Functions.Echo(input, bCenter, "bCenter");
		bRadius = input.ReadSingle();
		Logging_Functions.Echo(input, bRadius, "bRadius");
		textures = input.ReadStringArray();
		Trace("textures-read");
		Logging_Functions.Echo(input, textures.Length, "textures");
		Materials = ReadMaterialsWithRecovery(input);
		Trace("materials-read");
		Logging_Functions.Echo(input, Materials.Length, "materials");
		if (vertexCount == 0 && TrySkipMissingVertexIndexArrays(input))
		{
			Logging_Functions.Echo(input, pointToVertex.Length, "pointToVertex");
			Logging_Functions.Echo(input, vertexToPoint.Length, "vertexToPoint");
		}
		else if (HasExplicitEmptyVertexIndexArrays(input))
		{
			pointToVertex = input.ReadCompressedVertexIndexArray();
			Logging_Functions.Echo(input, pointToVertex.Length, "pointToVertex");
			vertexToPoint = input.ReadCompressedVertexIndexArray();
			Logging_Functions.Echo(input, vertexToPoint.Length, "vertexToPoint");
		}
		else if (TrySkipMissingVertexIndexArrays(input, 256))
		{
			Logging_Functions.Echo(input, pointToVertex.Length, "pointToVertex");
			Logging_Functions.Echo(input, vertexToPoint.Length, "vertexToPoint");
		}
		else if (TryReadVertexIndexArraysWithRecovery(input, out pointToVertex, out vertexToPoint))
		{
			Logging_Functions.Echo(input, pointToVertex.Length, "pointToVertex");
			Logging_Functions.Echo(input, vertexToPoint.Length, "vertexToPoint");
		}
		else if (TrySkipMissingVertexIndexArrays(input, 128))
		{
			Logging_Functions.Echo(input, pointToVertex.Length, "pointToVertex");
			Logging_Functions.Echo(input, vertexToPoint.Length, "vertexToPoint");
		}
		else if (TrySkipMissingVertexIndexArrays(input))
		{
			Logging_Functions.Echo(input, pointToVertex.Length, "pointToVertex");
			Logging_Functions.Echo(input, vertexToPoint.Length, "vertexToPoint");
		}
		else
		{
			pointToVertex = input.ReadCompressedVertexIndexArray();
			Logging_Functions.Echo(input, pointToVertex.Length, "pointToVertex");
			vertexToPoint = input.ReadCompressedVertexIndexArray();
			Logging_Functions.Echo(input, vertexToPoint.Length, "vertexToPoint");
		}
		Trace("vertex-index-arrays-read");
		TryAlignToPolygonStart(input);
		polygons = new Polygons(input);
		Trace("polygons-read");
		Logging_Functions.Echo(input, polygons.Faces.Length, "polygon faces");
		sections = input.ReadArray<Section>();
		Trace("sections-read");
		Logging_Functions.Echo(input, sections.Length, "sections");
		namedSelections = input.ReadArray<NamedSelection>();
		Trace("namedSelections-read");
		Logging_Functions.Echo(input, namedSelections.Length, "namedSelections");
		if (vertexCount == 0 && polygons.Faces.Length == 0 && sections.Length == 0 && namedSelections.Length == 0)
		{
			nNamedProperties = 0u;
			namedProperties = new string[0, 2];
			frames = Array.Empty<Keyframe>();
			uvSets = Array.Empty<UVSet>();
			vertices = Array.Empty<Vector3P>();
			normals = Array.Empty<Vector3P>();
			STCoords = Array.Empty<STPair>();
			vertexBoneRef = Array.Empty<AnimationRTWeight>();
			neighborBoneRef = Array.Empty<VertexNeighborInfo>();
			clip = Array.Empty<ClipFlags>();
			return;
		}
		nNamedProperties = input.ReadUInt32();
		Trace("nNamedProperties-read");
		long num3 = input.BaseStream.Length - input.Position;
		if (nNamedProperties > 4096 || (long)nNamedProperties * 2L > num3)
		{
			nNamedProperties = 0u;
		}
		Logging_Functions.Echo(input, nNamedProperties, "nNamedProperties");
		namedProperties = new string[nNamedProperties, 2];
		Logging_Functions.Echo(input, namedProperties.Length, "namedProperties");
		for (int num = 0; num < nNamedProperties; num++)
		{
			namedProperties[num, 0] = input.ReadAsciiz();
			namedProperties[num, 1] = input.ReadAsciiz();
		}
		frames = input.ReadArray<Keyframe>();
		Trace("frames-read");
		Logging_Functions.Echo(input, frames.Length, "frames");
		colorTop = input.ReadInt32();
		Logging_Functions.Echo(input, colorTop, "colorTop");
		color = input.ReadInt32();
		Logging_Functions.Echo(input, color, "color");
		special = input.ReadInt32();
		Logging_Functions.Echo(input, special, "special");
		vertexBoneRefIsSimple = input.ReadBoolean();
		sizeOfRestData = input.ReadUInt32();
		Trace("rest-header-read");
		Logging_Functions.Echo(input, sizeOfRestData, "sizeOfRestData", hex: true);
		if (odolVersion >= 50)
		{
			int[] array2 = input.ReadCondensedIntArray();
			Logging_Functions.Echo(input, array2.Length, "array2");
			clip = Array.ConvertAll(array2, (int item) => (ClipFlags)item);
		}
		UVSet uVSet = new UVSet();
		uVSet.Read(input, odolVersion);
		Trace("uvset0-read");
		nUVSets = input.ReadUInt32();
		long num4 = input.BaseStream.Length - input.Position;
		if (nUVSets > 64 || (nUVSets > 1 && (long)(nUVSets - 1) * 21L > num4))
		{
			nUVSets = 1u;
		}
		uvSets = new UVSet[nUVSets];
		if (nUVSets > 0)
		{
			uvSets[0] = uVSet;
			for (int num2 = 1; num2 < nUVSets; num2++)
			{
				uvSets[num2] = new UVSet();
				uvSets[num2].Read(input, odolVersion);
			}
		}
		Trace("uvsets-read");
		vertices = input.ReadCompressedObjectArray<Vector3P>(12);
		Trace("vertices-read");
		Logging_Functions.Echo(input, vertices.Length, "vertices");
		if (odolVersion >= 45)
		{
			Vector3PCompressed[] array3 = input.ReadCondensedObjectArray<Vector3PCompressed>(4);
			normals = Array.ConvertAll(array3, (Converter<Vector3PCompressed, Vector3P>)((Vector3PCompressed item) => item));
			Trace("normals-read");
			Logging_Functions.Echo(input, normals.Length, "normals");
		}
		else
		{
			normals = input.ReadCondensedObjectArray<Vector3P>(12);
			Trace("normals-read");
			Logging_Functions.Echo(input, normals.Length, "normals");
		}
		STCoords = (STPair[])((odolVersion >= 45) ? ((Array)input.ReadCompressedObjectArray<STPairCompressed>(8)) : ((Array)input.ReadCompressedObjectArray<STPairUncompressed>(24)));
		Trace("stcoords-read");
		Logging_Functions.Echo(input, STCoords.Length, "STCoords");
		vertexBoneRef = input.ReadCompressedObjectArray<AnimationRTWeight>(12);
		Trace("vertexBoneRef-read");
		Logging_Functions.Echo(input, vertexBoneRef.Length, "vertexBoneRef");
		neighborBoneRef = input.ReadCompressedObjectArray<VertexNeighborInfo>(32);
		Trace("neighborBoneRef-read");
		Logging_Functions.Echo(input, neighborBoneRef.Length, "neighborBoneRef");
		if (odolVersion >= 67)
		{
			input.Position += 4L;
		}
		if (odolVersion >= 68)
		{
			input.Position++;
		}
	}

	public void Write(BinaryWriterEx output)
	{
		throw new NotImplementedException();
	}

	public int CompareTo(LOD other)
	{
		return resolution.CompareTo(other.resolution);
	}
}
