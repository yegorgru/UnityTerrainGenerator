using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Utils
{
    public const int maxVerticesPerObject = 65536;

    public static GameObject[] ReadPrefabs(String path)
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

        // Group child meshes by material color
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
    }

    private static void CreateMergedObject(GameObject gameObject, List<CombineInstance> combineInstances, Material material)
    {
        if (combineInstances.Count == 0)
        {
            return;
        }
        GameObject mergedObject = new GameObject("MergedObject");
        mergedObject.transform.parent = gameObject.transform;
        mergedObject.transform.localPosition = Vector3.zero;
        mergedObject.transform.localRotation = Quaternion.identity;
        mergedObject.transform.localScale = Vector3.one;

        MeshFilter mergedMeshFilter = mergedObject.AddComponent<MeshFilter>();
        MeshRenderer mergedMeshRenderer = mergedObject.AddComponent<MeshRenderer>();
        mergedMeshRenderer.material = material;

        mergedMeshFilter.sharedMesh = new Mesh();
        mergedMeshFilter.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        mergedObject.SetActive(true);
    }
}
