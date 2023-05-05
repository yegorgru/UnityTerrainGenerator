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
    private int xCoord;
    private int yCoord;

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
        xCoord = EditorGUILayout.IntField("X coordinate of tile", xCoord);
        yCoord = EditorGUILayout.IntField("Y coordinate of tile", yCoord);

        if (GUILayout.Button("Create Tile"))
        {
            mapGenerator.GenerateChunk(new Vector2(xCoord, yCoord));
        }
    }
}
