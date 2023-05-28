using System.Drawing.Drawing2D;
using UnityEditor;
using UnityEngine;

public class PropertiesCity : PropertiesTile
{
    private static CityData cityDataLocal;

    public override void DrawGUI()
    {
        cityDataLocal = (CityData)EditorGUILayout.ObjectField("City Data Local", cityDataLocal, typeof(CityData), false);
    }

    public override void CreateTile(MapGenerator mapGenerator, int xCoord, int yCoord)
    {
        Vector2Int coordinates = new Vector2Int(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TileCity.GenerateTile(coordinates, mapGenerator.GetWidthOfRegion(), mapGenerator.GetLengthOfRegion(), mapGenerator.transform, mapGenerator.GetRoadItems(), mapGenerator.GetCityItems(), mapGenerator.GetCityData(), cityDataLocal);
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
