using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static MapGenerator;
using System;

[CustomEditor(typeof(MapGenerator))]  
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                // mapGen.DrawMapInEditor();
            }
        }
        if (GUILayout.Button("Clear"))
        {
            mapGen.Clear();
        }
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateCityMap();
            //GenerateTilesWindow window = GenerateTilesWindow.CreateGenerateTilesWindow(mapGen);
            //window.Show();
        }
        if (mapGen.IsGenerated() && GUILayout.Button("Add item"))
        {
            TileWindow window = TileWindow.CreateTileWindow(mapGen);
            window.Show();
        }
    }
}
