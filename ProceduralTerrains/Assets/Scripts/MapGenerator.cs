using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapGenerator : MonoBehaviour
{
    public const int mapChunkSize = 241;

    public bool autoUpdate;

    public Material mapMaterial;

    public Dictionary<Vector2, TilePerlinNoise> terrainChunkDictionary = new Dictionary<Vector2, TilePerlinNoise>();

    [SerializeField]
    private int widthOfRegion = 5;

    [SerializeField]
    private int lengthOfRegion = 5;

    public static MapData GenerateMapData(Vector2 center, NoiseData noiseData, RegionsData regionsData)
    {
        float[,] noiseMap = Noise.generateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.numberOctaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regionsData.regions.Length; i++)
                {
                    if(i == regionsData.regions.Length - 1 || currentHeight <= regionsData.regions[i].height)
                    {
                        colourMap[mapChunkSize * y + x] = regionsData.regions[i].colour;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);
    }

    public void GenerateChunks(NoiseData noiseData, TerrainData terrainData, RegionsData regionsData)
    {
        for (int y = 0; y < lengthOfRegion; y++)
        {
            for (int x = 0; x < widthOfRegion; x++)
            {
                GenerateChunk(new Vector2(x, y), noiseData, terrainData, regionsData);
            }
        }
    }

    public void GenerateChunk(Vector2 coordinates, NoiseData noiseData, TerrainData terrainData, RegionsData regionsData)
    {
        if(coordinates.x < 0 || coordinates.x >= widthOfRegion || coordinates.y < 0 || coordinates.y > lengthOfRegion)
        {
            Debug.LogWarning("Incorrect tile coordinates");
            return;
        }

        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);

        if (!terrainChunkDictionary.ContainsKey(coordinates))
        {
            TilePerlinNoise chunk = new TilePerlinNoise(viewedChunkCoord, 240, transform, mapMaterial, noiseData, terrainData, regionsData);
            chunk.CreateMesh();
            terrainChunkDictionary.Add(coordinates, chunk);
        }
    }

    public void Clear()
    {
        foreach (KeyValuePair<Vector2, TilePerlinNoise> pair in terrainChunkDictionary)
        {
            TilePerlinNoise chunk = pair.Value;
            chunk.Remove();
        }
        terrainChunkDictionary.Clear();
    }

    public void RemoveChunk(Vector2 coordinates)
    {
        if(terrainChunkDictionary.ContainsKey(coordinates))
        {
            terrainChunkDictionary[coordinates].Remove();
        }
        terrainChunkDictionary.Remove(coordinates);
    }
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