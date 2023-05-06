using Codice.Client.BaseCommands.Changelist;
using System;
using UnityEditor;
using UnityEngine;

public class GenerateTilesWindow : EditorWindow
{
    private MapGenerator mapGenerator;
    private static NoiseData noiseData;
    private static TerrainData terrainData;

    public TerrainType[] regions;
    private static TerrainType[] savedRegions;
    private SerializedObject so;
    private SerializedProperty regionsProp;

    public static GenerateTilesWindow CreateGenerateTilesWindow(MapGenerator mapGenerator)
    {
        GenerateTilesWindow window = ScriptableObject.CreateInstance<GenerateTilesWindow>();
        window.mapGenerator = mapGenerator;
        return window;
    }

    void OnGUI()
    {
        GUILayout.Label("Generate Tiles Parameters", EditorStyles.boldLabel);

        noiseData = (NoiseData)EditorGUILayout.ObjectField("Noise Data", noiseData, typeof(NoiseData), false);
        terrainData = (TerrainData)EditorGUILayout.ObjectField("Noise Data", terrainData, typeof(TerrainData), false);

        EditorGUILayout.PropertyField(regionsProp, true);
        so.ApplyModifiedProperties();

        if (GUILayout.Button("Generate Tiles"))
        {
            savedRegions = new TerrainType[regions.Length];
            Array.Copy(regions, savedRegions, regions.Length);
            mapGenerator.GenerateChunks(noiseData, terrainData, regions);
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
