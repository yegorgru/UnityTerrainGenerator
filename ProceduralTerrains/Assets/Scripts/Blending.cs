using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Blending
{
    public enum BlendingType
    {
        None,
        Linear,
        Smooth
    }

    public static float[,] ApplyBlending(Vector2Int centerCoord, int blendingWidth, BlendingType blendingType, in Dictionary<Vector2Int, float[,]> dict, in float[,] center)
    {
        if(blendingType  == BlendingType.None) {
            return center;
        }
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
                else if (j == 0 && isTop)
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
                    float coefficient = ((float)blendingWidth - i) / blendingWidth;
                    sum += Interpolate(center[i, j], leftValue, coefficient, blendingType);
                    counter++;
                }
                if (i >= result.GetLength(0) - blendingWidth && isRight)
                {
                    float coefficient = ((float)blendingWidth - result.GetLength(0) + 1 + i) / blendingWidth;
                    sum += Interpolate(center[i, j], rightValue, coefficient, blendingType);
                    counter++;
                }
                if (j <= blendingWidth && isTop)
                {
                    float coefficient = ((float)blendingWidth - j) / blendingWidth;
                    sum += Interpolate(center[i, j], topValue, coefficient, blendingType);
                    counter++;
                }
                if (j >= result.GetLength(1) - blendingWidth && isDown)
                {
                    float coefficient = ((float)j + 1 - result.GetLength(1) + blendingWidth) / blendingWidth;
                    sum += Interpolate(center[i, j], downValue, coefficient, blendingType);
                    counter++;
                }
                if (counter != 0)
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

    private static float Interpolate(float a, float b, float coefficient, BlendingType blendingType)
    {
        return blendingType == BlendingType.Linear ? Mathf.Lerp(a, b, coefficient) : Mathf.SmoothStep(a, b, coefficient);
    }
}
