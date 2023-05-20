using UnityEditor;
using UnityEngine;

public class PerlinNoiseProperties : TileProperties
{
    private static NoiseData noiseData;
    private static TerrainData terrainData;
    private static RegionsData regionsData;

    bool upDescent;
    bool downDescent;
    bool leftDescent;
    bool rightDescent;

    public override void DrawGUI()
    {
        noiseData = (NoiseData)EditorGUILayout.ObjectField("Noise Data", noiseData, typeof(NoiseData), false);
        terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", terrainData, typeof(TerrainData), false);
        regionsData = (RegionsData)EditorGUILayout.ObjectField("Regions Data", regionsData, typeof(RegionsData), false);

        upDescent = EditorGUILayout.Toggle("Up descent", upDescent);
        downDescent = EditorGUILayout.Toggle("Down descent", downDescent);
        leftDescent = EditorGUILayout.Toggle("Left descent", leftDescent);
        rightDescent = EditorGUILayout.Toggle("Right descent", rightDescent);
    }

    public override void CreateTile(MapGenerator mapGenerator, int xCoord, int yCoord)
    {
        Vector2Int coordinates = new Vector2Int(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TilePerlinNoise.GenerateTile(coordinates, noiseData, terrainData, regionsData, mapGenerator.GetWidthOfRegion(), mapGenerator.GetLengthOfRegion(), mapGenerator.transform, upDescent, downDescent, leftDescent, rightDescent);
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
