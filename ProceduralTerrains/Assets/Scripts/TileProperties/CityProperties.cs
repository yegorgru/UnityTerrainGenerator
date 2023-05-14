using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CityProperties : TileProperties
{
    public override void DrawGUI()
    {
        //noiseData = (NoiseData)EditorGUILayout.ObjectField("Noise Data", noiseData, typeof(NoiseData), false);
        //terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", terrainData, typeof(TerrainData), false);
        //regionsData = (RegionsData)EditorGUILayout.ObjectField("Regions Data", regionsData, typeof(RegionsData), false);
        //material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);
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
