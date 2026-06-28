using System;
using System.Collections.Generic;
using System.Linq;
using ODOL6.Common.Math;
using ODOL6.Model.MLOD;
using ODOL6.Model.ODOL;

namespace ODOL6.Model;

public static class Conversion
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

	private static PointFlags ClipFlagsToPointFlags(ClipFlags clipFlags)
	{
		PointFlags pointFlags = PointFlags.NONE;
		if ((clipFlags & ClipFlags.ClipLandStep) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.ONLAND;
		}
		else if ((clipFlags & ClipFlags.ClipLandUnder) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.UNDERLAND;
		}
		else if ((clipFlags & ClipFlags.ClipLandAbove) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.ABOVELAND;
		}
		else if ((clipFlags & ClipFlags.ClipLandKeep) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.KEEPLAND;
		}
		if ((clipFlags & ClipFlags.ClipDecalStep) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.DECAL;
		}
		else if ((clipFlags & ClipFlags.ClipDecalVertical) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.VDECAL;
		}
		if ((clipFlags & (ClipFlags)209715200) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.NOLIGHT;
		}
		else if ((clipFlags & (ClipFlags)212860928) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.FULLLIGHT;
		}
		else if ((clipFlags & (ClipFlags)211812352) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.HALFLIGHT;
		}
		else if ((clipFlags & (ClipFlags)210763776) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.AMBIENT;
		}
		if ((clipFlags & ClipFlags.ClipFogStep) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.NOFOG;
		}
		else if ((clipFlags & ClipFlags.ClipFogSky) != ClipFlags.ClipNone)
		{
			pointFlags |= PointFlags.SKYFOG;
		}
		int num = (int)(clipFlags & ClipFlags.ClipUserMask) / 1048576;
		return (PointFlags)((uint)pointFlags | (uint)(65536 * num));
	}

	public static ODOL6.Model.MLOD.MLOD ODOL2MLOD(ODOL6.Model.ODOL.ODOL odol)
	{
		P3D_LOD[] lODs = odol.LODs;
		int num = lODs.Length;
		MLOD_LOD[] array = new MLOD_LOD[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = OdolLod2MLOD(odol, (LOD)lODs[i]);
		}
		return new ODOL6.Model.MLOD.MLOD(array);
	}

	private static MLOD_LOD OdolLod2MLOD(ODOL6.Model.ODOL.ODOL odol, LOD src)
	{
		MLOD_LOD mLOD_LOD = new MLOD_LOD(src.Resolution);
		int vertexCount = src.VertexCount;
		ConvertPoints(odol, mLOD_LOD, src);
		mLOD_LOD.normals = src.Normals;
		ConvertFaces(odol, mLOD_LOD, src);
		float mass = odol.modelInfo.mass;
		_ = odol.Skeleton;
		mLOD_LOD.taggs = new List<Tagg>();
		if (src.Resolution == 1E+13f)
		{
			MassTagg item = CreateMassTagg(vertexCount, mass);
			mLOD_LOD.taggs.Add(item);
		}
		IEnumerable<UVSetTagg> collection = CreateUVSetTaggs(src);
		mLOD_LOD.taggs.AddRange(collection);
		IEnumerable<PropertyTagg> collection2 = CreatePropertyTaggs(src);
		mLOD_LOD.taggs.AddRange(collection2);
		IEnumerable<NamedSelectionTagg> collection3 = CreateNamedSelectionTaggs(src);
		mLOD_LOD.taggs.AddRange(collection3);
		IEnumerable<AnimationTagg> collection4 = CreateAnimTaggs(src);
		mLOD_LOD.taggs.AddRange(collection4);
		if (Resolution.KeepsNamedSelections(src.Resolution))
		{
			return mLOD_LOD;
		}
		Dictionary<string, List<PointWeight>> points = new Dictionary<string, List<PointWeight>>();
		Dictionary<string, List<int>> faces = new Dictionary<string, List<int>>();
		ReconstructNamedSelectionBySections(src, out points, out faces);
		Dictionary<string, List<PointWeight>> points2 = new Dictionary<string, List<PointWeight>>();
		Dictionary<string, List<int>> faces2 = new Dictionary<string, List<int>>();
		ReconstructProxies(src, out points2, out faces2);
		Dictionary<string, List<PointWeight>> points3 = new Dictionary<string, List<PointWeight>>();
		ReconstructNamedSelectionsByBones(src, odol.Skeleton, out points3);
		ApplySelectedPointsAndFaces(mLOD_LOD, points, faces);
		ApplySelectedPointsAndFaces(mLOD_LOD, points2, faces2);
		ApplySelectedPointsAndFaces(mLOD_LOD, points3, null);
		return mLOD_LOD;
	}

	private static void ApplySelectedPointsAndFaces(MLOD_LOD dstLod, Dictionary<string, List<PointWeight>> nsPoints, Dictionary<string, List<int>> nsFaces)
	{
		foreach (Tagg tagg in dstLod.taggs)
		{
			if (!(tagg is NamedSelectionTagg))
			{
				continue;
			}
			NamedSelectionTagg namedSelectionTagg = tagg as NamedSelectionTagg;
			if (nsPoints != null && nsPoints.TryGetValue(namedSelectionTagg.Name, out List<PointWeight> value))
			{
				foreach (PointWeight item in value)
				{
					byte b = (byte)(-item.weight);
					if (b != 0)
					{
						namedSelectionTagg.points[item.pointIndex] = b;
					}
				}
			}
			if (nsFaces == null || !nsFaces.TryGetValue(namedSelectionTagg.Name, out List<int> value2))
			{
				continue;
			}
			foreach (int item2 in value2)
			{
				namedSelectionTagg.faces[item2] = 1;
			}
		}
	}

	private static void ConvertPoints(ODOL6.Model.ODOL.ODOL odol, MLOD_LOD dstLod, LOD srcLod)
	{
		Vector3P boundingCenter = odol.modelInfo.boundingCenter;
		_ = odol.modelInfo.bboxMinVisual;
		_ = odol.modelInfo.bboxMaxVisual;
		int num = srcLod.Vertices.Length;
		dstLod.points = new Point[num];
		for (int i = 0; i < num; i++)
		{
			Vector3P pos = srcLod.Vertices[i] + boundingCenter;
			dstLod.points[i] = new Point(pos, ClipFlagsToPointFlags(srcLod.ClipFlags[i]));
		}
	}

	private static void ConvertFaces(ODOL6.Model.ODOL.ODOL odol, MLOD_LOD dstLod, LOD srcLOD)
	{
		List<Face> list = new List<Face>(srcLOD.VertexCount * 2);
		Section[] sections = srcLOD.Sections;
		foreach (Section section in sections)
		{
			float[] uVData = srcLOD.UVSets[0].UVData;
			uint[] faceIndexes = section.getFaceIndexes(srcLOD.Faces);
			foreach (uint num in faceIndexes)
			{
				int num2 = srcLOD.Faces[num].VertexIndices.Length;
				Vertex[] array = new Vertex[num2];
				for (int k = 0; k < num2; k++)
				{
					int num3 = srcLOD.Faces[num].VertexIndices[num2 - 1 - k];
					array[k] = new Vertex(num3, num3, uVData[num3 * 2], uVData[num3 * 2 + 1]);
				}
				string texture = ((section.textureIndex == -1) ? "" : srcLOD.Textures[section.textureIndex]);
				string material = ((section.materialIndex == -1) ? "" : srcLOD.Materials[section.materialIndex].materialName);
				Face item = new Face(num2, array, FaceFlags.DEFAULT, texture, material);
				list.Add(item);
			}
		}
		dstLod.faces = list.ToArray();
	}

	private static void ReconstructNamedSelectionBySections(LOD src, out Dictionary<string, List<PointWeight>> points, out Dictionary<string, List<int>> faces)
	{
		points = new Dictionary<string, List<PointWeight>>(src.NamedSelections.Length * 2);
		faces = new Dictionary<string, List<int>>(src.NamedSelections.Length * 2);
		NamedSelection[] namedSelections = src.NamedSelections;
		foreach (NamedSelection namedSelection in namedSelections)
		{
			if (namedSelection.IsSectional)
			{
				IEnumerable<uint> source = namedSelection.Sections.SelectMany((int si) => src.Sections[si].getFaceIndexes(src.Faces));
				IEnumerable<PointWeight> source2 = from vi in source.SelectMany((uint fi) => src.Faces[fi].VertexIndices)
					select new PointWeight(vi, byte.MaxValue);
				faces[namedSelection.Name] = source.Select((uint fi) => (int)fi).ToList();
				points[namedSelection.Name] = source2.ToList();
			}
		}
	}

	private static void ReconstructProxies(LOD src, out Dictionary<string, List<PointWeight>> points, out Dictionary<string, List<int>> faces)
	{
		points = new Dictionary<string, List<PointWeight>>(src.NamedSelections.Length * 2);
		faces = new Dictionary<string, List<int>>(src.NamedSelections.Length * 2);
		for (int i = 0; i < src.Faces.Length; i++)
		{
			Polygon polygon = src.Faces[i];
			if (polygon.VertexIndices.Length != 3)
			{
				continue;
			}
			VertexIndex vertexIndex = polygon.VertexIndices[0];
			VertexIndex vertexIndex2 = polygon.VertexIndices[1];
			VertexIndex vertexIndex3 = polygon.VertexIndices[2];
			Vector3P v = src.Vertices[(int)vertexIndex];
			Vector3P v2 = src.Vertices[(int)vertexIndex2];
			Vector3P v3 = src.Vertices[(int)vertexIndex3];
			float v4 = v.Distance(v2);
			float v5 = v.Distance(v3);
			float v6 = v2.Distance(v3);
			if (v4 > v5)
			{
				Methods.Swap(ref v2, ref v3);
				Methods.Swap(ref v4, ref v5);
			}
			if (v4 > v6)
			{
				Methods.Swap(ref v, ref v3);
				Methods.Swap(ref v4, ref v6);
			}
			if (v5 > v6)
			{
				Methods.Swap(ref v, ref v2);
				Methods.Swap(ref v5, ref v6);
			}
			Vector3P vector3P = v;
			Vector3P vector3P2 = v2 - v;
			Vector3P vector3P3 = v3 - v;
			vector3P2.Normalize();
			vector3P3.Normalize();
			if (!Methods.EqualsFloat(vector3P3 * vector3P2, 0f, 0.05f))
			{
				continue;
			}
			for (int j = 0; j < src.Proxies.Length; j++)
			{
				Vector3P position = src.Proxies[j].transformation.Position;
				Vector3P up = src.Proxies[j].transformation.Orientation.Up;
				Vector3P dir = src.Proxies[j].transformation.Orientation.Dir;
				if (vector3P.Equals(position) && vector3P2.Equals(dir) && vector3P3.Equals(up))
				{
					Proxy proxy = src.Proxies[j];
					string name = src.NamedSelections[proxy.namedSelectionIndex].Name;
					if (!faces.ContainsKey(name))
					{
						faces[name] = i.Yield().ToList();
						points[name] = Methods.Yield<PointWeight>(new PointWeight(vertexIndex, byte.MaxValue), new PointWeight(vertexIndex2, byte.MaxValue), new PointWeight(vertexIndex3, byte.MaxValue)).ToList();
						break;
					}
				}
			}
		}
	}

	private static void ReconstructNamedSelectionsByBones(LOD src, Skeleton skeleton, out Dictionary<string, List<PointWeight>> points)
	{
		points = new Dictionary<string, List<PointWeight>>(src.NamedSelections.Length * 2);
		if (src.VertexBoneRef.Length == 0)
		{
			return;
		}
		ushort num = 0;
		AnimationRTWeight[] vertexBoneRef = src.VertexBoneRef;
		for (int i = 0; i < vertexBoneRef.Length; i++)
		{
			AnimationRTPair[] animationRTPairs = vertexBoneRef[i].AnimationRTPairs;
			foreach (AnimationRTPair obj in animationRTPairs)
			{
				byte selectionIndex = obj.SelectionIndex;
				byte weight = obj.Weight;
				int num2 = src.SubSkeletonsToSkeleton[selectionIndex];
				string key = skeleton.bones[num2 * 2];
				PointWeight item = new PointWeight(num, weight);
				if (!points.TryGetValue(key, out List<PointWeight> value))
				{
					value = new List<PointWeight>(10000);
					value.Add(item);
					points[key] = value;
				}
				else
				{
					value.Add(item);
				}
			}
			num++;
		}
	}

	private static IEnumerable<NamedSelectionTagg> CreateNamedSelectionTaggs(LOD src)
	{
		int nPoints = src.VertexCount;
		int nFaces = src.Faces.Length;
		NamedSelection[] namedSelections = src.NamedSelections;
		NamedSelection[] array = namedSelections;
		foreach (NamedSelection namedSelection in array)
		{
			NamedSelectionTagg namedSelectionTagg = new NamedSelectionTagg
			{
				Name = namedSelection.Name,
				DataSize = (uint)(nPoints + nFaces),
				points = new byte[nPoints],
				faces = new byte[nFaces]
			};
			bool flag = namedSelection.SelectedVerticesWeights.Length != 0;
			int num = 0;
			VertexIndex[] selectedVertices = namedSelection.SelectedVertices;
			foreach (int num2 in selectedVertices)
			{
				if (num2 >= 0 && num2 < namedSelectionTagg.points.Length)
				{
					byte b = (byte)((!flag) ? 1 : ((byte)(-namedSelection.SelectedVerticesWeights[num++])));
					namedSelectionTagg.points[num2] = b;
				}
			}
			selectedVertices = namedSelection.SelectedFaces;
			foreach (int num3 in selectedVertices)
			{
				try
				{
					if (num3 >= 0 && num3 < namedSelectionTagg.faces.Length)
					{
						namedSelectionTagg.faces[num3] = 1;
					}
				}
				catch (Exception)
				{
					break;
				}
			}
			yield return namedSelectionTagg;
		}
	}

	private static IEnumerable<AnimationTagg> CreateAnimTaggs(LOD src)
	{
		Keyframe[] frames = src.Frames;
		Keyframe[] array = frames;
		foreach (Keyframe keyframe in array)
		{
			int num = keyframe.points.Length;
			AnimationTagg animationTagg = new AnimationTagg();
			animationTagg.Name = "#Animation#";
			animationTagg.DataSize = (uint)(num * 12 + 4);
			animationTagg.frameTime = keyframe.time;
			animationTagg.framePoints = new Vector3P[num];
			Array.Copy(keyframe.points, animationTagg.framePoints, num);
			yield return animationTagg;
		}
	}

	private static MassTagg CreateMassTagg(int nPoints, float totalMass)
	{
		MassTagg massTagg = new MassTagg();
		massTagg.Name = "#Mass#";
		massTagg.DataSize = (uint)(nPoints * 4);
		massTagg.mass = new float[nPoints];
		for (int i = 0; i < nPoints; i++)
		{
			massTagg.mass[i] = totalMass / (float)nPoints;
		}
		return massTagg;
	}

	private static IEnumerable<UVSetTagg> CreateUVSetTaggs(LOD src)
	{
		int nFaces = src.Faces.Length;
		int i = 0;
		while (i < src.UVSets.Length)
		{
			UVSetTagg uVSetTagg = new UVSetTagg();
			uVSetTagg.Name = "#UVSet#";
			uVSetTagg.uvSetNr = (uint)i;
			uVSetTagg.faceUVs = new float[nFaces][,];
			float[] uVData = src.UVSets[i].UVData;
			uint num = 4u;
			for (int j = 0; j < nFaces; j++)
			{
				Polygon polygon = src.Faces[j];
				int num2 = polygon.VertexIndices.Length;
				uVSetTagg.faceUVs[j] = new float[num2, 2];
				for (int k = 0; k < num2; k++)
				{
					VertexIndex vertexIndex = polygon.VertexIndices[num2 - 1 - k];
					uVSetTagg.faceUVs[j][k, 0] = uVData[(int)vertexIndex * 2];
					uVSetTagg.faceUVs[j][k, 1] = uVData[(int)vertexIndex * 2 + 1];
					num += 8;
				}
			}
			uVSetTagg.DataSize = num;
			yield return uVSetTagg;
			int num3 = i + 1;
			i = num3;
		}
	}

	private static IEnumerable<PropertyTagg> CreatePropertyTaggs(LOD src)
	{
		int i = 0;
		while (i < src.NamedProperties.Length / 2)
		{
			yield return new PropertyTagg
			{
				Name = "#Property#",
				DataSize = 128u,
				name = src.NamedProperties[i, 0],
				value = src.NamedProperties[i, 1]
			};
			int num = i + 1;
			i = num;
		}
	}
}
