using System;
using UnityEngine;

public class TileBuilding : MonoBehaviour
{
    [SerializeField]
    private GameObject wallPrefab;

    [SerializeField]
    private GameObject roofPrefab;

    [SerializeField]
    private int width = 3;

    [SerializeField]
    private int length = 3;

    [SerializeField]
    private float cellUnitSize;

    [SerializeField]
    private int numberOfFloors;

    private Floor[] floors;

    Vector2 position;

    public TileBuilding(Vector2 coord, int size, Transform parent)
    {
    }

    public void Generate()
    {
        floors = new Floor[numberOfFloors];
        int floorCount = 0;
        foreach (Floor floor in floors)
        {
            Room[,] rooms = new Room[width, length];
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < length; j++)
                {
                    rooms[i, j] = new Room(new Vector2(i, j), floorCount == numberOfFloors - 1);
                }
            }
            floors[floorCount] = new Floor(floorCount++, rooms);
        }
    }

    public void Render()
    {
        foreach(Floor floor in floors)
        {
            for(int i = 0; i < width; ++i)
            {
                for (int j = 0; j < length; ++j)
                {
                    Room room = floor.rooms[i, j];
                    var wall1 = Instantiate(wallPrefab, new Vector3(room.position.x * cellUnitSize, floor.FloorNumber * cellUnitSize, room.position.y * cellUnitSize), Quaternion.Euler(0, 0, 0));
                    wall1.transform.parent = transform;
                    var wall2 = Instantiate(wallPrefab, new Vector3(room.position.x * cellUnitSize, floor.FloorNumber * cellUnitSize, room.position.y * cellUnitSize), Quaternion.Euler(0, 90, 0));
                    wall2.transform.parent = transform;
                    var wall3 = Instantiate(wallPrefab, new Vector3(room.position.x * cellUnitSize, floor.FloorNumber * cellUnitSize, room.position.y * cellUnitSize), Quaternion.Euler(0, 180, 0));
                    wall3.transform.parent = transform;
                    var wall4 = Instantiate(wallPrefab, new Vector3(room.position.x * cellUnitSize, floor.FloorNumber * cellUnitSize, room.position.y * cellUnitSize), Quaternion.Euler(0, 270, 0));
                    wall4.transform.parent = transform;

                    if(room.hasRoof)
                    {
                        var roof = Instantiate(roofPrefab, new Vector3(room.position.x * cellUnitSize, floor.FloorNumber + 1.5f * cellUnitSize, room.position.y * cellUnitSize), Quaternion.Euler(-90, 270, 0));
                        roof.transform.parent = transform;
                    }
                }
            }
        }
    }
}

public class Wall
{
    public enum WallType
    {
        Normal
    }

    public WallType walType;

    public Wall(WallType walType = WallType.Normal)
    {
        this.walType = walType;
    }
}

public class Room
{
    public Wall[] walls;
    public Vector2 position;
    public bool hasRoof;

    public Room(Vector2 position, bool hasRoof = false)
    {
        this.position = position;
        this.hasRoof = hasRoof;
    }
}

public class Floor
{
    public int FloorNumber;

    public Room[,] rooms;

    public Floor(int floorNumber, Room[,] rooms)
    {
        FloorNumber = floorNumber;
        this.rooms = rooms;
    }
}