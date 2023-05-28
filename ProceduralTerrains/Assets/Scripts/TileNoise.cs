using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileNoise : Tile
{
    public Vector2 position;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    TerrainData terrainData;

    private const int size = 240;

    private GameObject objContainer;

    public enum RenderMode
    {
        Mesh3d,
        Noise2d
    }
     
    public static TileNoise GenerateTile(Dictionary<Vector2Int, float[,]> heigthMapDict,  Vector2Int coordinates, NoiseData noiseData, TerrainData terrainData, RegionsData regionsData, int widthOfRegion, int lengthOfRegion, Transform transform, int blendingWidth, Blending.BlendingType blendingType, RenderMode renderMode, string pathToObjects, int numberOfObjects)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);

        TileNoise tile = new TileNoise(viewedChunkCoord, transform, terrainData, 100f, noiseData);
        float[,] heightMap = tile.GetHeightMap();
        tile.SetHeightMap(Blending.ApplyBlending(coordinates, blendingWidth, blendingType, in heigthMapDict, in heightMap));
        tile.CreateMesh(tile.GenerateMapData(regionsData), renderMode, pathToObjects, numberOfObjects);
        return tile;
    }

    public TileNoise(Vector2 coord, Transform parent, TerrainData terrainData, float sizeScale, NoiseData noiseData)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets\\Materials\\DefaultMaterial.mat");
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

        objContainer = new GameObject("Non-buildings object");
        objContainer.transform.parent = meshObject.transform;
        objContainer.transform.localPosition = Vector3.zero;
        objContainer.transform.localScale = Vector3.one;
    }

    private MapData GenerateMapData(RegionsData regionsData)
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

        return new MapData(heightMap, colourMap);
    }

    public void CreateMesh(MapData mapData, RenderMode renderMode, string pathToObjects, int numberOfObjects)
    {
        Texture2D texture;
        if (renderMode == RenderMode.Mesh3d)
        {
            texture = Utils.TextureFromColourMap(mapData.colorMap, Noise.NOISE_MAP_WIDTH, Noise.NOISE_MAP_WIDTH);
        }
        else
        {
            texture = Utils.TextureFromHeightMap(mapData.heightMap);
        }
        Material material = GameObject.Instantiate(meshRenderer.sharedMaterial);
        material.SetTexture("_MainTex", texture);
        meshRenderer.material = material;

        Mesh mesh = Utils.GenerateTerrainMesh(mapData.heightMap, terrainData, renderMode);
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;

        GameObject[] objPrefabs = Utils.ReadPrefabs(pathToObjects);

        var vertices = mesh.vertices;

        for(int i = 0; i < numberOfObjects; ++i)
        {
            int idx = UnityEngine.Random.Range(0, vertices.Length);
            GameObject gameObject = objPrefabs[UnityEngine.Random.Range(0, objPrefabs.Length)];
            var obj = GameObject.Instantiate(gameObject, Vector3.zero, Quaternion.identity, objContainer.transform);
            float scale = 5;
            obj.transform.localPosition = vertices[idx];
            obj.transform.localScale = obj.transform.localScale * scale;
        }

        Utils.MergeChildMeshesByMaterialColor(objContainer);
    }
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}