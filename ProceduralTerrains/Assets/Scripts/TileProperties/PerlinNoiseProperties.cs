using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PerlinNoiseProperties : TileProperties
{
    private static NoiseData noiseData;
    private static TerrainData terrainData;
    private static RegionsData regionsData;

    public override void DrawGUI()
    {
        noiseData = (NoiseData)EditorGUILayout.ObjectField("Noise Data", noiseData, typeof(NoiseData), false);
        terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", terrainData, typeof(TerrainData), false);
        regionsData = (RegionsData)EditorGUILayout.ObjectField("Regions Data", regionsData, typeof(RegionsData), false);
    }

    public override void CreateTile(MapGenerator mapGenerator, float xCoord, float yCoord)
    {
        mapGenerator.GenerateChunk(new Vector2(xCoord, yCoord), noiseData, terrainData, regionsData);
    }

    public override void CreateTiles(MapGenerator mapGenerator)
    {
        mapGenerator.GenerateChunks(noiseData, terrainData, regionsData);
    }
}
