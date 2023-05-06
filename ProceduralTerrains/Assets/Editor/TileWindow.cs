using Codice.Client.BaseCommands.Changelist;
using System;
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
    private static NoiseData noiseData;
    private static TerrainData terrainData;

    public TerrainType[] regions;
    private static TerrainType[] savedRegions;
    private SerializedObject so;
    private SerializedProperty regionsProp;

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

        noiseData = (NoiseData)EditorGUILayout.ObjectField("Noise Data", noiseData, typeof(NoiseData), false);
        terrainData = (TerrainData)EditorGUILayout.ObjectField("Noise Data", terrainData, typeof(TerrainData), false);

        EditorGUILayout.PropertyField(regionsProp, true);
        so.ApplyModifiedProperties();

        if (GUILayout.Button("Create Tile"))
        {
            savedRegions = new TerrainType[regions.Length];
            Array.Copy(regions, savedRegions, regions.Length);
            mapGenerator.GenerateChunk(new Vector2(xCoord, yCoord), noiseData, terrainData, regions);
            Close();
        }
    }

    private void OnEnable()
    {
        if (savedRegions != null)
        {
            regions = new TerrainType[savedRegions.Length];
            Array.Copy(savedRegions, regions, regions.Length);
        }
        so = new SerializedObject(this);
        regionsProp = so.FindProperty("regions");
    }
}
