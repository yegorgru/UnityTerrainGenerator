using UnityEditor;
using UnityEngine;

public class CityProperties : TileProperties
{
    public int width = 200;
    public float buildingChance = 0.5f;
    public int maxFloor = 10;
    public Color color = Color.white;

    public override void DrawGUI()
    {
        width = EditorGUILayout.IntField(width);
        buildingChance = EditorGUILayout.FloatField(buildingChance);
        maxFloor = EditorGUILayout.IntField(maxFloor);
        color = EditorGUILayout.ColorField("Sidewalk color", color);
    }

    public override void CreateTile(MapGenerator mapGenerator, float xCoord, float yCoord)
    {
        Vector2 coordinates = new Vector2(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TileCity.GenerateTile(coordinates, mapGenerator.widthOfRegion, mapGenerator.lengthOfRegion, mapGenerator.transform, color, buildingChance, maxFloor);
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
