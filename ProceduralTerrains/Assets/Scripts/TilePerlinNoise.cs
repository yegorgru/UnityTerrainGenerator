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

    private const int mapChunkSize = 241;

    public static TilePerlinNoise GenerateTile(Vector2 coordinates, NoiseData noiseData, TerrainData terrainData, RegionsData regionsData, int widthOfRegion, int lengthOfRegion, Transform transform)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);
        Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets\\Materials\\DefaultMaterial.mat");

        TilePerlinNoise chunk = new TilePerlinNoise(viewedChunkCoord, transform, material, terrainData, 100f);
        chunk.CreateMesh(GenerateMapData(chunk.position, noiseData, regionsData));
        return chunk;
    }

    public static MapData GenerateMapData(Vector2 center, NoiseData noiseData, RegionsData regionsData)
    {
        float[,] noiseMap = Noise.generateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.numberOctaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regionsData.regions.Length; i++)
                {
                    if (i == regionsData.regions.Length - 1 || currentHeight <= regionsData.regions[i].height)
                    {
                        colourMap[mapChunkSize * y + x] = regionsData.regions[i].colour;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);
    }

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
        meshObject.transform.localScale = Vector3.one * 1f / size * sizeScale;
        meshObject.SetActive(true);
    }

    public void CreateMesh(MapData mapData)
    {
        Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colorMap, mapChunkSize, mapChunkSize);
        Material material = GameObject.Instantiate(meshRenderer.sharedMaterial);
        material.SetTexture("_MainTex", texture);
        meshRenderer.material = material;

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve);
        Mesh mesh = meshData.CreateMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}