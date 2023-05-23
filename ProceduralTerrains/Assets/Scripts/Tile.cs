using UnityEngine;

public abstract class Tile
{
    public GameObject meshObject;
    public float[,] heightMap;
    public void Remove()
    {
        meshObject.transform.SetParent(null);
        Object.DestroyImmediate(meshObject);
    }

    public float[,] GetHeightMap()
    {
        return heightMap;
    }

    public void SetHeightMap(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}
