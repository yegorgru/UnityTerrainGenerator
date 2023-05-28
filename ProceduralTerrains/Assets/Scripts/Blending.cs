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
        blendingWidth = Mathf.Max(blendingWidth, 0);
        blendingWidth = Mathf.Min(blendingWidth, 100);

        float[,] result = (float[,])center.Clone();

        float[,] left = null;
        bool isLeft = dict.TryGetValue(new Vector2Int(centerCoord.x - 1, centerCoord.y), out left);

        float[,] right = null;
        bool isRight = dict.TryGetValue(new Vector2Int(centerCoord.x + 1, centerCoord.y), out right);

        float[,] top = null;
        bool isTop = dict.TryGetValue(new Vector2Int(centerCoord.x, centerCoord.y + 1), out top);

        float[,] down = null;
        bool isDown = dict.TryGetValue(new Vector2Int(centerCoord.x, centerCoord.y - 1), out down);

        float[] values = new float[2];
        float[] coeffs = new float[2];

        for (int i = 0; i < result.GetLength(0); ++i)
        {
            for (int j = 0; j < result.GetLength(1); ++j)
            {
                float downValue = isDown ? down[i, 0] : 0f;
                float topValue = isTop ? top[i, top.GetLength(1) - 1] : 0f;
                float leftValue = isLeft ? left[left.GetLength(0) - 1, j] : 0f;
                float rightValue = isRight ? right[0, j] : 0f;
                int counter = 0;
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
                if (i <= blendingWidth && isLeft)
                {
                    coeffs[counter] = ((float)blendingWidth - i) / blendingWidth;
                    values[counter++] = leftValue;
                }
                if (i >= result.GetLength(0) - blendingWidth && isRight)
                {
                    coeffs[counter] = ((float)blendingWidth - result.GetLength(0) + 1 + i) / blendingWidth;
                    values[counter++] = rightValue;
                }
                if (j <= blendingWidth && isTop)
                {
                    coeffs[counter] = ((float)blendingWidth - j) / blendingWidth;
                    values[counter++] = topValue;
                }
                if (j >= result.GetLength(1) - blendingWidth && isDown)
                {
                    coeffs[counter] = ((float)j + 1 - result.GetLength(1) + blendingWidth) / blendingWidth;
                    values[counter++] = downValue;
                }
                if(counter == 1)
                {
                    result[i, j] = Interpolate(center[i, j], values[0], coeffs[0], blendingType);
                }
                else if (counter == 2)
                {
                    float value1 = Interpolate(center[i, j], values[0], coeffs[0], blendingType);
                    float value2 = Interpolate(center[i, j], values[1], coeffs[1], blendingType);
                    result[i, j] = Interpolate(value1, value2, coeffs[1] / (coeffs[0] + coeffs[1]), blendingType);
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
