using System;
using System.Linq;
using ODOL5.Common;
using ODOL5.Common.Math;
using ODOL5.Stream;

namespace ODOL5.Model.ODOL;

public class LOD : P3D_LOD, IComparable<LOD>
{
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

	public void Read(BinaryReaderEx input, float resolution)
	{
		odolVersion = (uint)input.Version;
		Logging_Functions.Echo(input, odolVersion, "odolVersion");
		base.resolution = resolution;
		Logging_Functions.Echo(input, base.resolution, "resolution");
		proxies = input.ReadArray<Proxy>();
		Logging_Functions.Echo(input, proxies, "proxies");
		subSkeletonsToSkeleton = input.ReadIntArray();
		Logging_Functions.Echo(input, subSkeletonsToSkeleton, "subSkeletonsToSkeleton");
		skeletonToSubSkeleton = input.ReadArray<SubSkeletonIndexSet>();
		Logging_Functions.Echo(input, skeletonToSubSkeleton, "skeletonToSubSkeleton");
		if (odolVersion >= 50)
		{
			vertexCount = input.ReadUInt32();
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
		Logging_Functions.Echo(input, textures.Length, "textures");
		Materials = input.ReadArray<EmbeddedMaterial>();
		Logging_Functions.Echo(input, Materials.Length, "materials");
		pointToVertex = input.ReadCompressedVertexIndexArray();
		Logging_Functions.Echo(input, pointToVertex.Length, "pointToVertex");
		vertexToPoint = input.ReadCompressedVertexIndexArray();
		Logging_Functions.Echo(input, vertexToPoint.Length, "vertexToPoint");
		polygons = new Polygons(input);
		Logging_Functions.Echo(input, polygons.Faces.Length, "polygon faces");
		sections = input.ReadArray<Section>();
		Logging_Functions.Echo(input, sections.Length, "sections");
		namedSelections = input.ReadArray<NamedSelection>();
		Logging_Functions.Echo(input, namedSelections.Length, "namedSelections");
		nNamedProperties = input.ReadUInt32();
		Logging_Functions.Echo(input, nNamedProperties, "nNamedProperties");
		namedProperties = new string[nNamedProperties, 2];
		Logging_Functions.Echo(input, namedProperties.Length, "namedProperties");
		for (int num = 0; num < nNamedProperties; num++)
		{
			namedProperties[num, 0] = input.ReadAsciiz();
			namedProperties[num, 1] = input.ReadAsciiz();
		}
		frames = input.ReadArray<Keyframe>();
		Logging_Functions.Echo(input, frames, "frames");
		colorTop = input.ReadInt32();
		Logging_Functions.Echo(input, colorTop, "colorTop");
		color = input.ReadInt32();
		Logging_Functions.Echo(input, color, "color");
		special = input.ReadInt32();
		Logging_Functions.Echo(input, special, "special");
		vertexBoneRefIsSimple = input.ReadBoolean();
		sizeOfRestData = input.ReadUInt32();
		Logging_Functions.Echo(input, sizeOfRestData, "sizeOfRestData", hex: true);
		if (odolVersion >= 50)
		{
			int[] array2 = input.ReadCondensedIntArray();
			Logging_Functions.Echo(input, array2, "array2");
			clip = Array.ConvertAll(array2, (int item) => (ClipFlags)item);
		}
		UVSet uVSet = new UVSet();
		uVSet.Read(input, odolVersion);
		nUVSets = input.ReadUInt32();
		uvSets = new UVSet[nUVSets];
		uvSets[0] = uVSet;
		for (int num2 = 1; num2 < nUVSets; num2++)
		{
			uvSets[num2] = new UVSet();
			uvSets[num2].Read(input, odolVersion);
		}
		vertices = input.ReadCompressedObjectArray<Vector3P>(12);
		Logging_Functions.Echo(input, vertices, "vertices");
		if (odolVersion >= 45)
		{
			Vector3PCompressed[] array3 = input.ReadCondensedObjectArray<Vector3PCompressed>(4);
			normals = Array.ConvertAll(array3, (Converter<Vector3PCompressed, Vector3P>)((Vector3PCompressed item) => item));
			Logging_Functions.Echo(input, normals, "normals");
		}
		else
		{
			normals = input.ReadCondensedObjectArray<Vector3P>(12);
			Logging_Functions.Echo(input, normals, "normals");
		}
		STCoords = (STPair[])((odolVersion >= 45) ? ((Array)input.ReadCompressedObjectArray<STPairCompressed>(8)) : ((Array)input.ReadCompressedObjectArray<STPairUncompressed>(24)));
		Logging_Functions.Echo(input, STCoords, "STCoords");
		vertexBoneRef = input.ReadCompressedObjectArray<AnimationRTWeight>(12);
		Logging_Functions.Echo(input, vertexBoneRef, "vertexBoneRef");
		neighborBoneRef = input.ReadCompressedObjectArray<VertexNeighborInfo>(32);
		Logging_Functions.Echo(input, neighborBoneRef, "neighborBoneRef");
		if (odolVersion >= 67)
		{
			input.ReadUInt32();
		}
		if (odolVersion >= 68)
		{
			input.ReadByte();
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
