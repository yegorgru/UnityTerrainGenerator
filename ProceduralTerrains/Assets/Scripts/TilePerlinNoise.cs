using System;
using System.Drawing;
using UnityEngine;

public class TilePerlinNoise : Tile
{
    public Vector2 position;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    TerrainData terrainData;

    private const int size = 240;

    public TilePerlinNoise(Vector2 coord, Transform parent, Material material, TerrainData terrainData, float sizeScale)
    {
        this.terrainData = terrainData;

        position = coord * size;
        Vector3 position3 = new Vector3(coord.x, 0, coord.y) * sizeScale + new Vector3(parent.transform.position.x, 0, parent.transform.position.z);

        meshObject = new GameObject("Perline Noise Tile");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshObject.transform.position = position3;
        meshRenderer.material = material;

        meshObject.transform.parent = parent;
        //meshObject.transform.localScale = Vector3.one * terrainData.uniformScale;
        meshObject.transform.localScale = Vector3.one * 1f / size * sizeScale;
        meshObject.SetActive(true);
    }

    public void CreateMesh(MapData mapData)
    {
        Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colorMap, GeneratorPerlinNoise.mapChunkSize, GeneratorPerlinNoise.mapChunkSize);
        Material material = GameObject.Instantiate(meshRenderer.sharedMaterial);
        material.SetTexture("_MainTex", texture);
        meshRenderer.material = material;

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve);
        Mesh mesh = meshData.CreateMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}