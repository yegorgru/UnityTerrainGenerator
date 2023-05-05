using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapGenerator : MonoBehaviour
{
    public TerrainData terrainData;
    public NoiseData noiseData;

    public const int mapChunkSize = 241;

    public bool autoUpdate;

    public TerrainType[] regions;

    public Material mapMaterial;

    public Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    [SerializeField]
    private int widthOfRegion = 5;

    [SerializeField]
    private int lengthOfRegion = 5;

    public void DrawMapInEditor()
    {
        GenerateChunks();
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

    private void GenerateChunks()
    {
        for (int y = 0; y < lengthOfRegion; y++)
        {
            for (int x = 0; x < widthOfRegion; x++)
            {
                GenerateChunk(new Vector2(x, y));
            }
        }
    }

    public void GenerateChunk(Vector2 coordinates)
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
            TerrainChunk chunk = new TerrainChunk(viewedChunkCoord, 240, transform, mapMaterial, this);
            chunk.CreateMesh();
            terrainChunkDictionary.Add(coordinates, chunk);
        }
    }

    public void Clear()
    {
        foreach (KeyValuePair<Vector2, TerrainChunk> pair in terrainChunkDictionary)
        {
            TerrainChunk chunk = pair.Value;
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