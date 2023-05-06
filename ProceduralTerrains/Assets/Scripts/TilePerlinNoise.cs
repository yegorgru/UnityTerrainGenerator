using System;
using UnityEngine;

public class TilePerlinNoise
{
    GameObject meshObject;
    Vector2 position;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    MapData mapData;

    NoiseData noiseData;
    TerrainData terrainData;
    TerrainType[] regions;

    public TilePerlinNoise(Vector2 coord, int size, Transform parent, Material material, NoiseData noiseData, TerrainData terrainData, TerrainType[] regions)
    {
        this.noiseData = noiseData;
        this.terrainData = terrainData;
        this.regions = regions;

        position = coord * size;
        Vector3 position3 = new Vector3(position.x, 0, position.y) * terrainData.uniformScale + new Vector3(parent.transform.position.x, 0, parent.transform.position.z);

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshObject.transform.position = position3;
        meshRenderer.material = material;

        meshObject.transform.parent = parent;
        meshObject.transform.localScale = Vector3.one * terrainData.uniformScale;
        meshObject.SetActive(true);
        this.regions = regions;
    }

    public void CreateMesh()
    {
        mapData = MapGenerator.GenerateMapData(position, noiseData, regions);

        Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
        Material material = GameObject.Instantiate(meshRenderer.sharedMaterial);
        material.SetTexture("_MainTex", texture);
        meshRenderer.material = material;

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve);
        Mesh mesh = meshData.CreateMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void Remove()
    {
        meshObject.transform.SetParent(null);
        UnityEngine.Object.DestroyImmediate(meshObject);
    }
}