using UnityEditor;
using UnityEngine;

public class CityProperties : TileProperties
{
    static int width = 200;
    static int maxNumberOfBuildings = 5;
    static Color color = Color.white;

    public override void DrawGUI()
    {
        width = EditorGUILayout.IntField(width);
        color = EditorGUILayout.ColorField("Sidewalk color", color);
    }

    public override void CreateTile(MapGenerator mapGenerator, float xCoord, float yCoord)
    {
        Vector2 coordinates = new Vector2(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TileCity.GenerateTile(coordinates, mapGenerator.widthOfRegion, mapGenerator.lengthOfRegion, mapGenerator.transform, color);
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
