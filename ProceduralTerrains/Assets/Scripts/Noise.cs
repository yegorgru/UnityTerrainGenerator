using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public const int NOISE_MAP_WIDTH = 241;

    public static float[,] GenerateNoiseMap(NoiseData noiseData, Vector2 center, bool useLerp)
    {
        Vector2 offset = noiseData.offset + center;
        if (noiseData.noiseScale <= 0)
        {
            noiseData.noiseScale = 0.0001f;
        }

        System.Random r = new System.Random(noiseData.seed);
        Vector2[] octaveOffsets = new Vector2[noiseData.numberOctaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < noiseData.numberOctaves; i++)
        {
            float offsetX = r.Next(-100000, 100000) + offset.x;
            float offsetY = r.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= noiseData.persistance;
        }

        float[,] noiseMap = new float[NOISE_MAP_WIDTH, NOISE_MAP_WIDTH];

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = NOISE_MAP_WIDTH / 2f;
        float halfHeight = NOISE_MAP_WIDTH / 2f;

        for (int x = 0; x < NOISE_MAP_WIDTH; x++)
        {
            for (int y = 0; y < NOISE_MAP_WIDTH; y++)
            {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < noiseData.numberOctaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / noiseData.noiseScale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / noiseData.noiseScale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= noiseData.persistance;
                    frequency *= noiseData.lacunarity;
                }
                
                maxLocalNoiseHeight = Mathf.Max(maxLocalNoiseHeight,noiseHeight);
                minLocalNoiseHeight = Mathf.Min(minLocalNoiseHeight,noiseHeight);
                noiseMap[x, y] = noiseHeight;
            }
        }

        if (useLerp)
        {
            for (int x = 0; x < NOISE_MAP_WIDTH; x++)
            {
                for (int y = 0; y < NOISE_MAP_WIDTH; y++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }
}
