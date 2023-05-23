using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using static TileCity;

public class MapGenerator : MonoBehaviour
{
    public bool autoUpdate;

    public int widthOfRegion = 5;

    public int lengthOfRegion = 5;

    public int cityFrequency = DEFAULT_CITY_FREQUENCY;

    public float buildingChance;

    public float nonBuildingChance;

    public int maxNumberOfFloors;

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
        cityFrequency = Mathf.RoundToInt(cityFrequency / 5f) * 5;
        cityFrequency = Math.Max(5, cityFrequency);
        buildingChance = Mathf.Min(1, Mathf.Max(0, buildingChance));
        nonBuildingChance = Mathf.Min(1, Mathf.Max(0, nonBuildingChance));
        maxNumberOfFloors = Math.Min(50, Math.Max(1, maxNumberOfFloors));
        cityMapGenerated = false;
    }

    public void GenerateCityMap()
    {
        roadItems = GenerateRoadMap(lengthOfRegion * cityFrequency / UNITS_PER_ROAD_ITEM, widthOfRegion * cityFrequency / UNITS_PER_ROAD_ITEM);
        cityItems = GenerateCityItemsMap(roadItems, buildingChance, nonBuildingChance, maxNumberOfFloors);
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

    public int GetCityFrequency()
    {
        return cityFrequency;
    }

    public float GetBuildingChance()
    {
        return buildingChance;
    }

    public float GetNonBuildingChance()
    {
        return nonBuildingChance;
    }

    public void GenerateChunks()
    {
        PerlinNoiseData noiseData = AssetDatabase.LoadAssetAtPath<PerlinNoiseData>("Assets\\TerrainAssets\\DefaultNoise.asset");
        TerrainData terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>("Assets\\TerrainAssets\\DefaultTerrain.asset");
        RegionsData regionsData = AssetDatabase.LoadAssetAtPath<RegionsData>("Assets\\TerrainAssets\\DefaultRegions.asset");
        for (int y = 0; y < lengthOfRegion; y++)
        {
            for (int x = 0; x < widthOfRegion; x++)
            {
                /*Vector2Int coordinates = new Vector2Int(x, y);
                Tile tile = TilePerlinNoise.GenerateTile(coordinates, noiseData, terrainData, regionsData, widthOfRegion, lengthOfRegion, transform);
                AddChunk(coordinates, tile);*/
            }
        }
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

    public void RemoveChunk(Vector2Int coordinates)
    {
        if(terrainChunkDictionary.ContainsKey(coordinates))
        {
            terrainChunkDictionary[coordinates].Remove();
        }
        terrainChunkDictionary.Remove(coordinates);
    }

    
}