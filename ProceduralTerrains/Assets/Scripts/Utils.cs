using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Utils
{
    public const int maxVerticesPerObject = 65536;
    public const string MERGED_OBJ_NAME = "MergedObject";

    public static GameObject[] ReadPrefabs(string path)
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { path });
        GameObject[] objects = new GameObject[guids.Length];
        int objCounter = 0;
        foreach (string guid in guids)
        {
            string guidPath = AssetDatabase.GUIDToAssetPath(guid);
            objects[objCounter++] = AssetDatabase.LoadAssetAtPath(guidPath, typeof(GameObject)) as GameObject;
        }
        return objects;
    }

    public static void MergeChildMeshesByMaterialColor(GameObject gameObject)
    {
        MeshFilter[] childMeshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true);

        Dictionary<Color, List<MeshFilter>> meshGroups = new Dictionary<Color, List<MeshFilter>>();
        foreach (MeshFilter childMeshFilter in childMeshFilters)
        {
            Material meshMaterial = childMeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
            Color materialColor = meshMaterial.color;

            if (!meshGroups.ContainsKey(materialColor))
            {
                meshGroups[materialColor] = new List<MeshFilter>();
            }
            meshGroups[materialColor].Add(childMeshFilter);
        }

        foreach (var group in meshGroups)
        {
            int totalVertices = 0;
            List<MeshFilter> meshes = group.Value;

            List<CombineInstance> combineInstances = new List<CombineInstance>();
            for (int i = 0; i < meshes.Count; i++)
            {
                MeshFilter meshFilter = meshes[i];
                int meshVertices = meshFilter.sharedMesh.vertexCount;

                if (totalVertices + meshVertices > maxVerticesPerObject)
                {
                    CreateMergedObject(gameObject, combineInstances, meshes[0].GetComponent<MeshRenderer>().sharedMaterial);
                    combineInstances.Clear();
                    totalVertices = 0;
                }

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = meshFilter.sharedMesh;
                combineInstance.transform = meshFilter.transform.localToWorldMatrix;
                combineInstances.Add(combineInstance);
                meshFilter.gameObject.SetActive(false);

                totalVertices += meshVertices;
            }

            CreateMergedObject(gameObject, combineInstances, meshes[0].GetComponent<MeshRenderer>().sharedMaterial);
        }
        RemoveRecoursively(gameObject.transform);
    }

    private static void RemoveRecoursively(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child.name != MERGED_OBJ_NAME)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
            else
            {
                RemoveRecoursively(child);
            }
        }
    }

    private static void CreateMergedObject(GameObject gameObject, List<CombineInstance> combineInstances, Material material)
    {
        if (combineInstances.Count == 0)
        {
            return;
        }
        GameObject mergedObject = new GameObject(MERGED_OBJ_NAME);
        mergedObject.transform.parent = gameObject.transform;

        MeshFilter mergedMeshFilter = mergedObject.AddComponent<MeshFilter>();
        MeshRenderer mergedMeshRenderer = mergedObject.AddComponent<MeshRenderer>();
        mergedMeshRenderer.material = material;

        mergedMeshFilter.sharedMesh = new Mesh();
        mergedMeshFilter.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        mergedObject.SetActive(true);
    }

    public static Texture2D TextureFromColourMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }
        return TextureFromColourMap(colorMap, width, height);
    }

    public static Mesh GenerateTerrainMesh(float[,] heightMap, TerrainData terrainData, TileNoise.RenderMode renderMode)
    {
        AnimationCurve heightCurve = new AnimationCurve(terrainData.meshHeightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;


        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (renderMode == TileNoise.RenderMode.Mesh3d)
                {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * terrainData.meshHeightMultiplier, topLeftZ - y);
                }
                else
                {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, 0, topLeftZ - y);
                }
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }

        return meshData.CreateMesh();
    }

    private class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        private int triangleIndex;

        public MeshData(int meshWidth, int meshHeight)
        {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
            triangleIndex = 0;
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
