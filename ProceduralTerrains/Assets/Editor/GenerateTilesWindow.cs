using Codice.Client.BaseCommands.Changelist;
using System;
using UnityEditor;
using UnityEngine;
using static TileWindow;

public class GenerateTilesWindow : EditorWindow
{
    private MapGenerator mapGenerator;
    private TileProperties tileProperties;
    private TileType tileType;


    public static GenerateTilesWindow CreateGenerateTilesWindow(MapGenerator mapGenerator)
    {
        GenerateTilesWindow window = ScriptableObject.CreateInstance<GenerateTilesWindow>();
        window.mapGenerator = mapGenerator;
        return window;
    }

    void OnGUI()
    {
        GUILayout.Label("Generate Tiles Parameters", EditorStyles.boldLabel);

        tileType = (TileType)EditorGUILayout.EnumPopup("Tile Type", tileType);

        if (tileType == TileType.PerlinNoiseTableland)
        {
            if (tileProperties == null || !(tileProperties is PerlinNoiseProperties))
            {
                tileProperties = new PerlinNoiseProperties();
            }
        }
        tileProperties.DrawGUI();


        if (GUILayout.Button("Generate Tiles"))
        {
            tileProperties.CreateTiles(mapGenerator);
            Close();
        }
    }
}
