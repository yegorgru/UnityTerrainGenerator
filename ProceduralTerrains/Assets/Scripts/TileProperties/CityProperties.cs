using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CityProperties : TileProperties
{
    static int width = 200;
    static int maxNumberOfBuildings = 5;

    public override void DrawGUI()
    {
        width = EditorGUILayout.IntField(width);
    }

    public override void CreateTile(MapGenerator mapGenerator, float xCoord, float yCoord)
    {
        Vector2 coordinates = new Vector2(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = mapGenerator.generatorCity.GenerateCityTile(coordinates, mapGenerator.widthOfRegion, mapGenerator.lengthOfRegion, mapGenerator.transform);
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
