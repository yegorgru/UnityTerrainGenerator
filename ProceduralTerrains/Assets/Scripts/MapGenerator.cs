﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public TerrainData terrainData;
    public NoiseData noiseData;

    public const int mapChunkSize = 241;

    public bool autoUpdate;

    public TerrainType[] regions;

    public Material mapMaterial;

    public Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    public void DrawMapInEditor()
    {
        UpdateVisibleChunks();
    }

    public MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.generateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.numberOctaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if(i == regions.Length - 1 || currentHeight <= regions[i].height)
                    {
                        colourMap[mapChunkSize * y + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);
    }

    private void UpdateVisibleChunks()
    {
        for (int yOffset = -1; yOffset <= 1; yOffset++)
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(xOffset, yOffset);

                if (!terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    TerrainChunk chunk = new TerrainChunk(viewedChunkCoord, 240, transform, mapMaterial, this);
                    chunk.CreateMesh();
                    terrainChunkDictionary.Add(viewedChunkCoord, chunk);
                }
            }
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}