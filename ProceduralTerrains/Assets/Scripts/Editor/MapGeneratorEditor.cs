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
        if (mapGen.IsGenerated() && GUILayout.Button("Add tile"))
        {
            TileWindow window = TileWindow.CreateTileWindow(mapGen, TileWindow.Mode.Create);
            window.Show();
        }
        if (mapGen.IsGenerated() && GUILayout.Button("Remove tile"))
        {
            TileWindow window = TileWindow.CreateTileWindow(mapGen, TileWindow.Mode.Remove);
            window.Show();
        }
    }
}
