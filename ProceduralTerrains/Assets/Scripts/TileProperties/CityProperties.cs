using System;
using UnityEditor;
using UnityEngine;

public class CityProperties : TileProperties
{
    public Color color = Color.white;
    public Building.FloorSizePolicy floorSizePolicy = Building.FloorSizePolicy.Constant;

    public override void DrawGUI()
    {
        floorSizePolicy = (Building.FloorSizePolicy)EditorGUILayout.EnumPopup("Floor size policy", floorSizePolicy);
        color = EditorGUILayout.ColorField("Base color", color);
    }

    public override void CreateTile(MapGenerator mapGenerator, int xCoord, int yCoord)
    {
        Vector2Int coordinates = new Vector2Int(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TileCity.GenerateTile(coordinates, mapGenerator.GetWidthOfRegion(), mapGenerator.GetLengthOfRegion(), mapGenerator.GetCityFrequency(), mapGenerator.transform, color, floorSizePolicy, mapGenerator.GetRoadItems(), mapGenerator.GetCityItems());
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
