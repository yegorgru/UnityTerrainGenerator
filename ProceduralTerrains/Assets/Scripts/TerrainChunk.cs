using System;
using UnityEngine;

public class TerrainChunk
{
    GameObject meshObject;
    Vector2 position;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    MapData mapData;

    MapGenerator mapGenerator;

    public TerrainChunk(Vector2 coord, int size, Transform parent, Material material, MapGenerator mapGenerator)
    {
        this.mapGenerator = mapGenerator;

        position = coord * size;
        Vector3 position3 = new Vector3(position.x, 0, position.y) * mapGenerator.terrainData.uniformScale + new Vector3(mapGenerator.transform.position.x, 0, mapGenerator.transform.position.z);

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshObject.transform.position = position3;
        meshRenderer.material = material;

        meshObject.transform.parent = parent;
        meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
        meshObject.SetActive(true);
    }

    public void CreateMesh()
    {
        mapData = mapGenerator.GenerateMapData(position);

        Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
        Material material = GameObject.Instantiate(meshRenderer.sharedMaterial);
        material.SetTexture("_MainTex", texture);
        meshRenderer.material = material;

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, mapGenerator.terrainData.meshHeightMultiplier, mapGenerator.terrainData.meshHeightCurve);
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