using UnityEditor;
using UnityEngine;

public class PropertiesNoise : PropertiesTile
{
    private static NoiseData noiseData;

    private static TerrainData terrainData;

    private static RegionsData regionsData;

    private static int blendingWidth = 50;

    private static Blending.BlendingType blendingType;

    private static TileNoise.RenderMode renderMode;

    public override void DrawGUI()
    {
        noiseData = (NoiseData)EditorGUILayout.ObjectField("Noise Data", noiseData, typeof(NoiseData), false);
        terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", terrainData, typeof(TerrainData), false);
        regionsData = (RegionsData)EditorGUILayout.ObjectField("Regions Data", regionsData, typeof(RegionsData), false);

        blendingWidth = EditorGUILayout.IntField("Blending width", blendingWidth);
        blendingType = (Blending.BlendingType)EditorGUILayout.EnumPopup("Blending type", blendingType);
        renderMode = (TileNoise.RenderMode)EditorGUILayout.EnumPopup("Render mode", renderMode);
    }

    public override void CreateTile(MapGenerator mapGenerator, int xCoord, int yCoord)
    {
        Vector2Int coordinates = new Vector2Int(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TileNoise.GenerateTile(mapGenerator.GetHeightDictionary(), coordinates, noiseData, terrainData, regionsData, mapGenerator.GetWidthOfRegion(), mapGenerator.GetLengthOfRegion(), mapGenerator.transform, blendingWidth, blendingType, renderMode);
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
