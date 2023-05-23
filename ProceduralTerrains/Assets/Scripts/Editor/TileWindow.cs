using UnityEditor;
using UnityEngine;

public class TileWindow : EditorWindow
{

    private MapGenerator mapGenerator;
    private MapGenerator.TileType tileType;
    private int xCoord;
    private int yCoord;
    private TileProperties tileProperties;

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
            if (tileProperties == null || !(tileProperties is PerlinNoiseProperties))
            {
                tileProperties = new PerlinNoiseProperties();
            }
        }
        if (tileType == MapGenerator.TileType.City)
        {
            if (tileProperties == null || !(tileProperties is CityProperties))
            {
                tileProperties = new CityProperties();
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
