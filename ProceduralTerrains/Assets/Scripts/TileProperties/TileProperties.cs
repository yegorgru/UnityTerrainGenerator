public abstract class TileProperties
{
    public abstract void DrawGUI();

    public abstract void CreateTile(MapGenerator mapGenerator, float xCoord, float yCoord);

    public abstract void CreateTiles(MapGenerator mapGenerator);
}