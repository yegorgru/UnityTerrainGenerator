using System;
using UnityEditor;
using UnityEngine;

public class CityProperties : TileProperties
{
    public int width = 200;
    public float buildingChance = 0.5f;
    public int maxFloor = 10;
    public Color color = Color.white;
    public Building.FloorSizePolicy floorSizePolicy = Building.FloorSizePolicy.Constant;

    public override void DrawGUI()
    {
        width = EditorGUILayout.IntField(width);
        buildingChance = EditorGUILayout.FloatField(buildingChance);
        maxFloor = EditorGUILayout.IntField(maxFloor);
        floorSizePolicy = (Building.FloorSizePolicy)EditorGUILayout.EnumPopup("Floor size policy", floorSizePolicy);
        color = EditorGUILayout.ColorField("Base color", color);
    }

    public override void CreateTile(MapGenerator mapGenerator, float xCoord, float yCoord)
    {
        Vector2 coordinates = new Vector2(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TileCity.GenerateTile(coordinates, mapGenerator.widthOfRegion, mapGenerator.lengthOfRegion, mapGenerator.transform, color, buildingChance, maxFloor, floorSizePolicy);
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
