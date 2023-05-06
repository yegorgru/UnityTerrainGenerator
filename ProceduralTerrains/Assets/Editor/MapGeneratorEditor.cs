using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            GenerateTilesWindow window = GenerateTilesWindow.CreateGenerateTilesWindow(mapGen);
            window.Show();
        }
        if (GUILayout.Button("Add item"))
        {
            TileWindow window = TileWindow.CreateTileWindow(mapGen);
            window.Show();
        }
    }
}
