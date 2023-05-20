using Codice.Client.BaseCommands.Changelist;
using System;
using UnityEditor;
using UnityEngine;

public class GenerateTilesWindow : EditorWindow
{
    private MapGenerator mapGenerator;


    public static GenerateTilesWindow CreateGenerateTilesWindow(MapGenerator mapGenerator)
    {
        GenerateTilesWindow window = CreateInstance<GenerateTilesWindow>();
        window.mapGenerator = mapGenerator;
        return window;
    }

    void OnGUI()
    {
        GUILayout.Label("Generate Tiles Parameters", EditorStyles.boldLabel);


        if (GUILayout.Button("Generate Tiles"))
        {
            //mapGenerator.GenerateChunks();
            Close();
        }
    }
}
