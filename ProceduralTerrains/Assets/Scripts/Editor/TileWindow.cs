using UnityEditor;
using UnityEngine;

public class TileWindow : EditorWindow
{

    private MapGenerator mapGenerator;
    private MapGenerator.TileType tileType;
    private int xCoord;
    private int yCoord;
    private PropertiesTile tileProperties;

    public static TileWindow CreateTileWindow(MapGenerator mapGenerator)
    {
        TileWindow window = ScriptableObject.CreateInstance<TileWindow>();
        window.mapGenerator = mapGenerator;
        return window;
    }

    void OnGUI()
    {
        GUILayout.Label("Tile Parameters", EditorStyles.boldLabel);

        tileType = (MapGenerator.TileType)EditorGUILayout.EnumPopup("Tile Type", tileType);
        xCoord = EditorGUILayout.IntField("X coordinate of tile", xCoord);
        yCoord = EditorGUILayout.IntField("Y coordinate of tile", yCoord);

        if (tileType == MapGenerator.TileType.NoiseTableland)
        {
            if (tileProperties == null || !(tileProperties is PropertiesNoise))
            {
                tileProperties = new PropertiesNoise();
            }
        }
        if (tileType == MapGenerator.TileType.City)
        {
            if (tileProperties == null || !(tileProperties is PropertiesCity))
            {
                tileProperties = new PropertiesCity();
            }
        }

        tileProperties.DrawGUI();

        if (GUILayout.Button("Create Tile"))
        {
            tileProperties.CreateTile(mapGenerator, xCoord, yCoord);
            Close();
        }
    }
}
