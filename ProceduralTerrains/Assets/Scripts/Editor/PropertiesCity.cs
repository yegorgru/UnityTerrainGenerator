using UnityEngine;

public class PropertiesCity : PropertiesTile
{
    public override void DrawGUI()
    {

    }

    public override void CreateTile(MapGenerator mapGenerator, int xCoord, int yCoord)
    {
        Vector2Int coordinates = new Vector2Int(xCoord, yCoord);
        if(mapGenerator.CheckPosition(coordinates))
        {
            Tile tile = TileCity.GenerateTile(coordinates, mapGenerator.GetWidthOfRegion(), mapGenerator.GetLengthOfRegion(), mapGenerator.transform, mapGenerator.GetRoadItems(), mapGenerator.GetCityItems(), mapGenerator.GetCityData());
            mapGenerator.AddChunk(coordinates, tile);
        }
    }
}
