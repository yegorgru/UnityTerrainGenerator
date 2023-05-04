using Codice.Client.BaseCommands.Changelist;
using UnityEditor;
using UnityEngine;

public class TileWindow : EditorWindow
{
    public enum TileType
    {
        PerlinNoiseTableland
    }

    private MapGenerator mapGenerator;
    private TileType tileType;

    public static TileWindow CreateTileWindow(MapGenerator mapGenerator)
    {
        TileWindow window = ScriptableObject.CreateInstance<TileWindow>();
        window.mapGenerator = mapGenerator;
        return window;
    }

    void OnGUI()
    {
        GUILayout.Label("Tile Parameters", EditorStyles.boldLabel);

        tileType = (TileType)EditorGUILayout.EnumPopup("Tile Type", tileType);

        if (GUILayout.Button("Create Tile"))
        {
            
        }
    }
}
