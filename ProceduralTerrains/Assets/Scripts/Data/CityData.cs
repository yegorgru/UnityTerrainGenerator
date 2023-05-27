using System;
using UnityEngine;

[CreateAssetMenu]
public class CityData : ScriptableObject
{
    public int cityFrequency = TileCity.DEFAULT_CITY_FREQUENCY;

    public float buildingChance = 0.5f;

    public float nonBuildingChance = 0.5f;

    public int maxNumberOfFloors = 10;

    public Color sidewalkColor = Color.white;

    public Building.FloorSizePolicy floorSizePolicy = Building.FloorSizePolicy.Random;

    public int startRoadItemsNumber = 1;

    [Range(0f, 1f)]
    public float roadChance = 0.5f;

    private void OnValidate()
    {
        cityFrequency = Mathf.RoundToInt(cityFrequency / 5f) * 5;
        cityFrequency = Math.Max(5, cityFrequency);
        buildingChance = Mathf.Min(1, Mathf.Max(0, buildingChance));
        nonBuildingChance = Mathf.Min(1, Mathf.Max(0, nonBuildingChance));
        maxNumberOfFloors = Math.Min(50, Math.Max(1, maxNumberOfFloors));
    }
}
