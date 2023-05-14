using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneratorPerlinNoise
{
    public const int mapChunkSize = 241;

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
                    if (i == regionsData.regions.Length - 1 || currentHeight <= regionsData.regions[i].height)
                    {
                        colourMap[mapChunkSize * y + x] = regionsData.regions[i].colour;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);
    }

    public TilePerlinNoise GeneratePerlinNoiseTile(Vector2 coordinates, NoiseData noiseData, TerrainData terrainData, RegionsData regionsData, int widthOfRegion, int lengthOfRegion, Transform transform)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);
        Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets\\Materials\\DefaultMaterial.mat");

        TilePerlinNoise chunk = new TilePerlinNoise(viewedChunkCoord, transform, material, terrainData, 100f);
        chunk.CreateMesh(GenerateMapData(chunk.position, noiseData, regionsData));
        return chunk;
    }
}
