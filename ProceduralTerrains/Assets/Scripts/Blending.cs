using System.Collections.Generic;
using UnityEngine;

public static class Blending
{
    public enum BlendingType
    {
        None,
        Linear,
        Cosine,
        Cubic,
        Bilinear,
    }

    public static float[,] ApplyBlending(Vector2Int center, int blendingWidth, BlendingType blendingType, in Dictionary<Vector2Int, float[,]> dict, in float[,] centerHeightMap)
    {
        switch (blendingType)
        {
            case BlendingType.Linear:
                return LinearBlending(center, blendingWidth, dict, centerHeightMap);
            default:
                return centerHeightMap;
        }
    }

    private static float[,] LinearBlending(Vector2Int centerCoord, int blendingWidth, in Dictionary<Vector2Int, float[,]> dict, in float[,] center)
    {
        float[,] result = (float[,])center.Clone();

        float[,] left = null;
        bool isLeft = dict.TryGetValue(new Vector2Int(centerCoord.x - 1, centerCoord.y), out left);

        float[,] right = null;
        bool isRight = dict.TryGetValue(new Vector2Int(centerCoord.x + 1, centerCoord.y), out right);

        float[,] top = null;
        bool isTop = dict.TryGetValue(new Vector2Int(centerCoord.x, centerCoord.y + 1), out top);

        float[,] down = null;
        bool isDown = dict.TryGetValue(new Vector2Int(centerCoord.x, centerCoord.y - 1), out down);

        for (int i = 0; i < result.GetLength(0); ++i)
        {
            for (int j = 0; j < result.GetLength(1); ++j)
            {
                float downValue = isDown ? down[i, 0] : 0f;
                float topValue = isTop ? top[i, top.GetLength(1) - 1] : 0f;
                float leftValue = isLeft ? left[left.GetLength(0) - 1, j] : 0f;
                float rightValue = isRight ? right[0, j] : 0f;
                if (j == result.GetLength(1) - 1 && isDown)
                {
                    result[i, j] = downValue;
                    continue;
                }
                else if(j == 0 && isTop)
                {
                    result[i, j] = topValue;
                    continue;
                }
                else if (i == 0 && isLeft)
                {
                    result[i, j] = leftValue;
                    continue;
                }
                else if (i == result.GetLength(0) - 1 && isRight)
                {
                    result[i, j] = rightValue;
                    continue;
                }
                int counter = 0;
                float sum = 0f;
                if (i <= blendingWidth && isLeft)
                {
                    sum += ((blendingWidth - i) * leftValue + i * center[i, j]) / blendingWidth;
                    counter++;
                }
                if(i >= result.GetLength(0) - blendingWidth && isRight)
                {
                    sum += ((blendingWidth - result.GetLength(0) + 1 + i) * rightValue + (result.GetLength(0) - 1 - i) * center[i, j]) / blendingWidth;
                    counter++;
                }
                if (j <= blendingWidth && isTop)
                {
                    sum += ((blendingWidth - j) * topValue + j * center[i, j]) / blendingWidth;
                    counter++;
                }
                if (j >= result.GetLength(1) - blendingWidth && isDown)
                {
                    sum += ((result.GetLength(1) - 1 - j) * center[i, j] + (j + 1 - result.GetLength(1) + blendingWidth) * downValue) / blendingWidth;
                    counter++;
                }
                if(counter != 0)
                {
                    result[i, j] = sum / counter;
                }
                else
                {
                    result[i, j] = center[i, j];
                }
            }
        }
        return result;
    }
}
