using System;
using UnityEditor;
using UnityEngine;

public class CityProperties : TileProperties
{
    public float buildingChance = 0.5f;
    public float nonBuildingChance = 0.5f;
    public int maxFloor = 10;
    public Color color = Color.white;
    public Building.FloorSizePolicy floorSizePolicy = Building.FloorSizePolicy.Constant;

    public override void DrawGUI()
    {
        buildingChance = EditorGUILayout.FloatField("Chance of building", buildingChance);
        nonBuildingChance = EditorGUILayout.FloatField("Chance of non-building on free space", nonBuildingChance);
        maxFloor = EditorGUILayout.IntField("Max number of floors", maxFloor);
        floorSizePolicy = (Building.FloorSizePolicy)EditorGUILayout.EnumPopup("Floor size policy", floorSizePolicy);
        color = EditorGUILayout.ColorField("Base color", color);
    }

    public override void CreateTile(MapGenerator mapGenerator, float xCoord, float yCoord)
    {
        Vector2 coordinates = new Vector2(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TileCity.GenerateTile(coordinates, mapGenerator.widthOfRegion, mapGenerator.lengthOfRegion, mapGenerator.transform, color, buildingChance, nonBuildingChance, maxFloor, floorSizePolicy);
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
