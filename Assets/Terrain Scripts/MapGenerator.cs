﻿using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode {NoiseMap, ColourMap, Mesh, FalloffMap};
	public DrawMode drawMode;
	public Noise.NormalizeMode normalizeMode;
	public const int mapChunkSize = 239;
	public float noiseScale;
	[Range(0,6)]
	public int EditorLevelOfDetail;

	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public float heightMultiplier;
	public AnimationCurve heightCurve;
	public Vector2 offset;
	public bool useFalloff;

	public bool autoUpdate;
	
	public TerrainType[] regions;
	float[,] falloffMap;

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

	private void Awake()
	{
		seed = Random.Range(1, 100000);
		falloffMap = FallOffGenerator.GenerateFalloffMap(mapChunkSize);

	}

	public void DrawMapInEditor()
	{
		MapData mapData = GenerateMapData(Vector2.zero);
		MapDisplay display = FindObjectOfType<MapDisplay> ();
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (mapData.heightMap));
		} else if (drawMode == DrawMode.ColourMap) {
			display.DrawTexture (TextureGenerator.TextureFromColourMap (mapData.colourMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.Mesh) {
			display.DrawMesh (MeshGenerator.GenerateTerrainMesh (mapData.heightMap, heightMultiplier, heightCurve, EditorLevelOfDetail));
		} else if (drawMode  == DrawMode.FalloffMap)
		{
			 display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFalloffMap(mapChunkSize)));
		}
	}

	public void RequestMapData(Vector2 centre, Action<MapData> callBack)
	{
		ThreadStart threadStart = delegate
		{
			MapDataThread(centre, callBack);
		};
		new Thread(threadStart).Start();
	}

	void MapDataThread(Vector2 centre, Action<MapData> callBack)
	{
		MapData mapData = GenerateMapData(centre);
		lock (mapDataThreadInfoQueue)
		{
			mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callBack, mapData));
		}
	}

	public void RequestMeshData(MapData mapData,int lod, Action<MeshData> callBack)
	{
		ThreadStart threadStart = delegate
		{
			MeshDataThread(mapData,lod,callBack);
		};
		new Thread(threadStart).Start();
	}

	void MeshDataThread(MapData mapData,int lod, Action<MeshData> callBack)
	{
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, heightMultiplier, heightCurve, lod);
		lock (meshDataThreadInfoQueue)
		{
			meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callBack, meshData));
		}
	}

	private void Update()
	{
		if (mapDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
				threadInfo.callBack(threadInfo.parameter);
			}
		}

		if (meshDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
				threadInfo.callBack(threadInfo.parameter);
			}
		}
	}

	MapData GenerateMapData(Vector2 centre) {
		
		float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize+2, mapChunkSize+2, seed, noiseScale, octaves, persistance, lacunarity, centre +offset, normalizeMode);

		Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				if (useFalloff)
				{
					noiseMap[x, y] =Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
				}
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight >= regions [i].height) {
						colourMap [y * mapChunkSize + x] = regions [i].colour;
					}
					else
					{
						break;
					}
				}
			}
		}

		return new MapData(noiseMap, colourMap);
	}

	void OnValidate() {
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}

		falloffMap = FallOffGenerator.GenerateFalloffMap(mapChunkSize);
	}

	struct MapThreadInfo<T>
	{
		public readonly Action<T> callBack;
		public readonly T parameter;
			public MapThreadInfo(Action<T> callBack, T parameter)
			{
				this.callBack = callBack;
				this.parameter = parameter;
			}
		
	}


}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}

public struct MapData
{
	public readonly float[,] heightMap;
	public readonly Color[] colourMap;

	public MapData(float[,] heightMap, Color[] colourMap)
	{
		this.heightMap = heightMap;
		this.colourMap = colourMap;
	}
}