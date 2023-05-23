using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TilePerlinNoise : Tile
{
    public Vector2 position;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    TerrainData terrainData;

    private const int size = 240;

    public static TilePerlinNoise GenerateTile(Dictionary<Vector2Int, float[,]> heigthMapDict,  Vector2Int coordinates, NoiseData noiseData, TerrainData terrainData, RegionsData regionsData, int widthOfRegion, int lengthOfRegion, Transform transform, int blendingWidth, Blending.BlendingType blendingType)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);
        Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets\\Materials\\DefaultMaterial.mat");

        TilePerlinNoise tile = new TilePerlinNoise(viewedChunkCoord, transform, material, terrainData, 100f, noiseData);
        float[,] heightMap = tile.GetHeightMap();
        tile.SetHeightMap(Blending.ApplyBlending(coordinates, blendingWidth, blendingType, in heigthMapDict, in heightMap));
        tile.CreateMesh(tile.GenerateMapData(regionsData));
        return tile;
    }

    public TilePerlinNoise(Vector2 coord, Transform parent, Material material, TerrainData terrainData, float sizeScale, NoiseData noiseData)
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
        meshObject.transform.localScale = Vector3.one * 1f / size * sizeScale;
        meshObject.SetActive(true);

        float[,] noiseMap = Noise.GenerateNoise(noiseData, position, true);
        heightMap = noiseMap;
    }

    private PerlinNoiseMapData GenerateMapData(RegionsData regionsData)
    {
        Color[] colourMap = new Color[Noise.NOISE_MAP_WIDTH * Noise.NOISE_MAP_WIDTH];
        for (int y = 0; y < Noise.NOISE_MAP_WIDTH; y++)
        {
            for (int x = 0; x < Noise.NOISE_MAP_WIDTH; x++)
            {
                float currentHeight = heightMap[x, y];
                for (int i = 0; i < regionsData.regions.Length; i++)
                {
                    if (i == regionsData.regions.Length - 1 || currentHeight <= regionsData.regions[i].height)
                    {
                        colourMap[Noise.NOISE_MAP_WIDTH * y + x] = regionsData.regions[i].colour;
                        break;
                    }
                }
            }
        }

        return new PerlinNoiseMapData(heightMap, colourMap);
    }

    public void CreateMesh(PerlinNoiseMapData mapData)
    {
        Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colorMap, Noise.NOISE_MAP_WIDTH, Noise.NOISE_MAP_WIDTH);
        Material material = GameObject.Instantiate(meshRenderer.sharedMaterial);
        material.SetTexture("_MainTex", texture);
        meshRenderer.material = material;

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve);
        Mesh mesh = meshData.CreateMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}

public struct PerlinNoiseMapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public PerlinNoiseMapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}