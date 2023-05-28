using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TileCity;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private int widthOfRegion = 5;

    [SerializeField]
    private int lengthOfRegion = 5;

    [SerializeField]
    private CityData cityData;

    private bool cityMapGenerated = false;

    private Dictionary<Vector2Int, int> terrainChunkDictionary = new Dictionary<Vector2Int, int>();
    private Dictionary<Vector2Int, float[,]> heightMapDictionary = new Dictionary<Vector2Int, float[,]>();

    private RoadItem[,] roadItems;
    private CityItem[,] cityItems;

    public enum TileType
    {
        NoiseTableland,
        City
    }

    public bool IsGenerated()
    {
        return cityMapGenerated;
    }

    private void OnValidate()
    {
        widthOfRegion = Math.Max(1, widthOfRegion);
        lengthOfRegion = Math.Max(1, lengthOfRegion);
        cityMapGenerated = false;
        Clear();
    }

    public void GenerateCityMap()
    {
        roadItems = GenerateRoadMap(lengthOfRegion * cityData.cityFrequency / UNITS_PER_ROAD_ITEM, widthOfRegion * cityData.cityFrequency / UNITS_PER_ROAD_ITEM, cityData, true);
        cityItems = GenerateCityItemsMap(roadItems, cityData, true);
        cityMapGenerated = true;
    }

    public RoadItem[,] GetRoadItems() { 
        return roadItems; 
    }

    public CityItem[,] GetCityItems()
    {
        return cityItems;
    }

    public int GetWidthOfRegion()
    {
        return widthOfRegion;
    }

    public int GetLengthOfRegion()
    {
        return lengthOfRegion;
    }

    public void Clear()
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            GameObject.DestroyImmediate(child.gameObject);
        }
        List<Vector2Int> keys = new List<Vector2Int>(terrainChunkDictionary.Keys);
        foreach(var item in keys)
        {
            RemoveTile(item);
        }
        terrainChunkDictionary.Clear();
        heightMapDictionary.Clear();
    }

    public bool CheckPosition(Vector2Int coordinates)
    {
        if (coordinates.x < 0 || coordinates.x >= widthOfRegion || coordinates.y < 0 || coordinates.y >= lengthOfRegion || terrainChunkDictionary.ContainsKey(coordinates))
        {
            Debug.LogWarning("Incorrect tile coordinates");
            return false;
        }
        return true;
    }

    public void AddChunk(Vector2Int coordinates, Tile tile)
    {
        terrainChunkDictionary.Add(coordinates, tile.meshObject.GetInstanceID());
        heightMapDictionary[coordinates] = tile.GetHeightMap();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    public Dictionary<Vector2Int, float[,]> GetHeightDictionary()
    {
        return heightMapDictionary;
    }

    public void RemoveTile(Vector2Int coordinates)
    {
        if(terrainChunkDictionary.ContainsKey(coordinates))
        {
            int id;
            terrainChunkDictionary.TryGetValue(coordinates, out id);
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (obj.GetInstanceID() == id)
                {
                    GameObject.DestroyImmediate(obj);
                    break;
                }
            }
        }
        terrainChunkDictionary.Remove(coordinates);
        heightMapDictionary.Remove(coordinates);
    }

    public CityData GetCityData()
    {
        return cityData;
    }
}