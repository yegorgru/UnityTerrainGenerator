using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorCity
{
    public TileCity GenerateCityTile(Vector2 coordinates, int widthOfRegion, int lengthOfRegion, Transform transform)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);

        TileCity chunk = new TileCity(viewedChunkCoord, 240, transform, 100f);
        return chunk;
    }
}
