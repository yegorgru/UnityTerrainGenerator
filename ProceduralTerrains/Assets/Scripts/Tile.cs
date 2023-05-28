using UnityEngine;

public abstract class Tile
{
    public GameObject meshObject;
    public float[,] heightMap;

    public float[,] GetHeightMap()
    {
        return heightMap;
    }

    public void SetHeightMap(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}
