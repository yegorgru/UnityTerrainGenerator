using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public const int NOISE_MAP_WIDTH = 241;

    public static float[,] GenerateNoise(NoiseData noiseData, Vector2 center, bool useLerp)
    {
        switch(noiseData.GetNoiseType())
        {
            case NoiseData.NoiseType.ValueNoise:
                return GenerateValueNoise(noiseData, center, useLerp);
            case NoiseData.NoiseType.PerlinNoise:
            default:
                return GeneratePerlinNoise(noiseData, center, useLerp);
        }
    }

    public static float[,] GeneratePerlinNoise(NoiseData noiseData, Vector2 center, bool useLerp)
    {
        Vector2 offset = noiseData.GetOffset() + center;

        System.Random r = new System.Random(noiseData.GetSeed());
        Vector2[] octaveOffsets = new Vector2[noiseData.GetNumberOctaves()];

        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < noiseData.GetNumberOctaves(); i++)
        {
            float offsetX = r.Next(-100000, 100000) + offset.x;
            float offsetY = r.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= noiseData.GetPersistance();
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

                for (int i = 0; i < noiseData.GetNumberOctaves(); i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / noiseData.GetNoiseScale() * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / noiseData.GetNoiseScale() * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= noiseData.GetPersistance();
                    frequency *= noiseData.GetLacunarity();
                }
                
                maxLocalNoiseHeight = Mathf.Max(maxLocalNoiseHeight,noiseHeight);
                minLocalNoiseHeight = Mathf.Min(minLocalNoiseHeight,noiseHeight);
                noiseMap[x, y] = noiseHeight;
            }
        }

        ApplyLerp(noiseMap, minLocalNoiseHeight, maxLocalNoiseHeight, useLerp);

        return noiseMap;
    }

    private static float[,] GenerateWhiteNoise(Vector2 offset)
    {
        float[,] noiseMap = new float[NOISE_MAP_WIDTH, NOISE_MAP_WIDTH];

        for (int x = 0; x < NOISE_MAP_WIDTH; x++)
        {
            for (int y = 0; y < NOISE_MAP_WIDTH; y++)
            {
                float random = Vector2.Dot(offset + new Vector2(x, y), new Vector2(12, 78));
                random = Mathf.Sin(random);
                random *= 43758.5453f;
                noiseMap[x, y] = random - Mathf.Floor(random);
            }
        }

        return noiseMap;
    }

    private static void ApplyLerp(float[,] noiseMap, float minLocal, float maxLocal, bool useLerp)
    {
        if (useLerp)
        {
            for (int x = 0; x < NOISE_MAP_WIDTH; x++)
            {
                for (int y = 0; y < NOISE_MAP_WIDTH; y++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocal, maxLocal, noiseMap[x, y]);
                }
            }
        }
    }

    public static float[,] GenerateValueNoise(NoiseData noiseData, Vector2 center, bool useLerp)
    {
        Vector2 offset = noiseData.GetOffset() + center;

        float[,] noiseMap = GenerateWhiteNoise(offset);

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        int gridCellSize = (NOISE_MAP_WIDTH - 1) / noiseData.GetGridFrequency();

        for (int x = 0; x < NOISE_MAP_WIDTH; x++)
        {
            for (int y = 0; y < NOISE_MAP_WIDTH; y++)
            {
                if(y == NOISE_MAP_WIDTH - 1)
                {
                    noiseMap[x, y] = noiseMap[x, y - 1];
                    continue;
                }
                if (x == NOISE_MAP_WIDTH - 1)
                {
                    noiseMap[x, y] = noiseMap[x - 1, y];
                    continue;
                }

                Vector2Int leftDownGridPos = new Vector2Int(x / gridCellSize, y / gridCellSize);
                Vector2Int rightDownGridPos = new Vector2Int(leftDownGridPos.x + 1, leftDownGridPos.y);
                Vector2Int leftTopGridPos = new Vector2Int(leftDownGridPos.x, leftDownGridPos.y + 1);
                Vector2Int rightTopGridPos = new Vector2Int(leftDownGridPos.x + 1, leftDownGridPos.y + 1);

                float b = Mathf.SmoothStep(noiseMap[leftDownGridPos.x * gridCellSize, leftDownGridPos.y * gridCellSize], noiseMap[rightDownGridPos.x * gridCellSize, rightDownGridPos.y * gridCellSize], (float)(x % gridCellSize) / gridCellSize);
                float t = Mathf.SmoothStep(noiseMap[leftTopGridPos.x * gridCellSize, leftTopGridPos.y * gridCellSize], noiseMap[rightTopGridPos.x * gridCellSize, rightTopGridPos.y * gridCellSize], (float)(x % gridCellSize) / gridCellSize);

                noiseMap[x, y] = Mathf.SmoothStep(b, t, (float)(y % gridCellSize) / gridCellSize);

                maxLocalNoiseHeight = Mathf.Max(maxLocalNoiseHeight, noiseMap[x, y]);
                minLocalNoiseHeight = Mathf.Min(minLocalNoiseHeight, noiseMap[x, y]);
            }
        }

        ApplyLerp(noiseMap, minLocalNoiseHeight, maxLocalNoiseHeight, useLerp);

        return noiseMap;
    }
}