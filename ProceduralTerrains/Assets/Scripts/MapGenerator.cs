using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
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

    private Dictionary<Vector2Int, Tile> terrainChunkDictionary = new Dictionary<Vector2Int, Tile>();
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
        roadItems = GenerateRoadMap(lengthOfRegion * cityData.cityFrequency / UNITS_PER_ROAD_ITEM, widthOfRegion * cityData.cityFrequency / UNITS_PER_ROAD_ITEM, cityData);
        cityItems = GenerateCityItemsMap(roadItems, cityData);
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
        terrainChunkDictionary.Add(coordinates, tile);
        heightMapDictionary[coordinates] = tile.GetHeightMap();
    }

    public Dictionary<Vector2Int, float[,]> GetHeightDictionary()
    {
        return heightMapDictionary;
    }

    public void RemoveTile(Vector2Int coordinates)
    {
        if(terrainChunkDictionary.ContainsKey(coordinates))
        {
            terrainChunkDictionary[coordinates].Remove();
        }
        terrainChunkDictionary.Remove(coordinates);
    }

    public CityData GetCityData()
    {
        return cityData;
    }
}