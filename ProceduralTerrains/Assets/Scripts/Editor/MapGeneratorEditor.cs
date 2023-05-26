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
        DrawDefaultInspector();
        if (GUILayout.Button("Clear"))
        {
            mapGen.Clear();
        }
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateCityMap();
        }
        if (mapGen.IsGenerated() && GUILayout.Button("Add item"))
        {
            TileWindow window = TileWindow.CreateTileWindow(mapGen);
            window.Show();
        }
    }
}
