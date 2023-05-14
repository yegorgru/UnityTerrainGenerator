using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapGenerator : MonoBehaviour
{
    public bool autoUpdate;

    public Dictionary<Vector2, Tile> terrainChunkDictionary = new Dictionary<Vector2, Tile>();

    public int widthOfRegion = 5;

    public int lengthOfRegion = 5;

    public GeneratorPerlinNoise generatorPerlinNoise = new GeneratorPerlinNoise();
    public GeneratorCity generatorCity = new GeneratorCity();

    public void GenerateChunks()
    {
        NoiseData noiseData = AssetDatabase.LoadAssetAtPath<NoiseData>("Assets\\TerrainAssets\\DefaultNoise.asset");
        TerrainData terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>("Assets\\TerrainAssets\\DefaultTerrain.asset");
        RegionsData regionsData = AssetDatabase.LoadAssetAtPath<RegionsData>("Assets\\TerrainAssets\\DefaultRegions.asset");
        Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets\\Materials\\DefaultMaterial.mat");
        for (int y = 0; y < lengthOfRegion; y++)
        {
            for (int x = 0; x < widthOfRegion; x++)
            {
                Vector2 coordinates = new Vector2(x, y);
                Tile tile = generatorPerlinNoise.GeneratePerlinNoiseTile(coordinates, noiseData, terrainData, regionsData, widthOfRegion, lengthOfRegion, transform);
                AddChunk(coordinates, tile);
            }
        }
    }

    public void Clear()
    {
        foreach (KeyValuePair<Vector2, Tile> pair in terrainChunkDictionary)
        {
            Tile chunk = pair.Value;
            chunk.Remove();
        }
        terrainChunkDictionary.Clear();
    }

    public bool CheckPosition(Vector2 coordinates)
    {
        if (coordinates.x < 0 || coordinates.x >= widthOfRegion || coordinates.y < 0 || coordinates.y > lengthOfRegion || terrainChunkDictionary.ContainsKey(coordinates))
        {
            Debug.LogWarning("Incorrect tile coordinates");
            return false;
        }
        return true;
    }

    public void AddChunk(Vector2 coordinates, Tile tile)
    {
        terrainChunkDictionary.Add(coordinates, tile);
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