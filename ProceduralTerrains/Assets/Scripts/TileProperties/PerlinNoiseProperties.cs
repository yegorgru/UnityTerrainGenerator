using UnityEditor;
using UnityEngine;

public class PerlinNoiseProperties : TileProperties
{
    private static NoiseData noiseData;
    private static TerrainData terrainData;
    private static RegionsData regionsData;

    public override void DrawGUI()
    {
        noiseData = (NoiseData)EditorGUILayout.ObjectField("Noise Data", noiseData, typeof(NoiseData), false);
        terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", terrainData, typeof(TerrainData), false);
        regionsData = (RegionsData)EditorGUILayout.ObjectField("Regions Data", regionsData, typeof(RegionsData), false);
    }

    public override void CreateTile(MapGenerator mapGenerator, int xCoord, int yCoord)
    {
        Vector2Int coordinates = new Vector2Int(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TilePerlinNoise.GenerateTile(coordinates, noiseData, terrainData, regionsData, mapGenerator.GetWidthOfRegion(), mapGenerator.GetLengthOfRegion(), mapGenerator.transform);
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
