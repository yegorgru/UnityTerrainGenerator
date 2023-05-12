using System;
using UnityEditor;
using UnityEngine;

public class TileBuilding : MonoBehaviour
{
    private GameObject[] wallPrefabs;

    private GameObject[] windowPrefabs;

    private GameObject[] floorPrefabs;

    private GameObject[] roofPrefabs;

    private GameObject[] doorPrefabs;

    [SerializeField]
    private string prefabsPath;

    [SerializeField]
    private int width = 3;

    [SerializeField]
    private int length = 3;

    [SerializeField]
    private float cellUnitSize;

    [SerializeField]
    private int numberOfFloors;

    [SerializeField]
    private float windowChance;

    private Floor[] floors;

    Vector2 position;

    public TileBuilding(Vector2 coord, int size, Transform parent)
    {
    }

    public void ReadPrefabs()
    {
        wallPrefabs = ReadPrefabs(prefabsPath + "\\Walls");
        windowPrefabs = ReadPrefabs(prefabsPath + "\\Windows");
        floorPrefabs = ReadPrefabs(prefabsPath + "\\Floors");
        roofPrefabs = ReadPrefabs(prefabsPath + "\\Roofs");
        doorPrefabs = ReadPrefabs(prefabsPath + "\\Doors");
    }

    private GameObject[] ReadPrefabs(String path)
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { path });
        GameObject[] objects = new GameObject[guids.Length];
        int objCounter = 0;
        foreach (string guid in guids)
        {
            string guidPath = AssetDatabase.GUIDToAssetPath(guid);
            objects[objCounter++] = AssetDatabase.LoadAssetAtPath(guidPath, typeof(GameObject)) as GameObject;
        }
        return objects;
    }

    public void Generate()
    {
        System.Random rnd = new System.Random();
        int doorWallNumber = rnd.Next(length * width);
        int findDoorCounter = 0;

        floors = new Floor[numberOfFloors];
        int floorCount = 0;

        int[] bounds = new int[] {0, width - 1, 0, length - 1};

        foreach (Floor floor in floors)
        {
            Room[,] rooms = new Room[width, length];
            for(int i = bounds[0]; i <= bounds[1]; i++)
            {
                for(int j = bounds[2]; j <= bounds[3]; j++)
                {
                    Wall[] walls = new Wall[4];
                    for(int k = 0; k < 4; k++)
                    {
                        if (floorCount == 0)
                        {
                            bool sideWall = false;
                            if (i == 0 && k == 0)
                            {
                                sideWall = true;
                            }
                            else if (i == width - 1 && k == 2)
                            {
                                sideWall = true;
                            }
                            else if (j == 0 && k == 3)
                            {
                                sideWall = true;
                            }
                            else if (j == length - 1 && k == 1)
                            {
                                sideWall = true;
                            }
                            if(sideWall)
                            {
                                if(findDoorCounter == doorWallNumber)
                                {
                                    findDoorCounter++;
                                    walls[k] = new Wall(Wall.WallType.Door);
                                    continue;
                                }
                                else
                                {
                                    findDoorCounter++;
                                }
                            }
                        }
                        if ((float)rnd.NextDouble() < windowChance)
                        {
                            walls[k] = new Wall(Wall.WallType.Window);
                        }
                        else
                        {
                            walls[k] = new Wall();
                        }
                    }
                    rooms[i, j] = new Room(new Vector2(i, j), walls, floorCount == numberOfFloors - 1);
                }
            }
            floors[floorCount] = new Floor(floorCount++, rooms);
        }
    }

    public void Render()
    {
        System.Random rnd = new System.Random();
        foreach (Floor floor in floors)
        {
            for(int i = 0; i < width; ++i)
            {
                for (int j = 0; j < length; ++j)
                {
                    Room room = floor.rooms[i, j];

                    Wall[] walls = room.walls;
                    for (int k = 0;k < 4; k++)
                    {
                        GameObject gameObject;
                        switch (walls[k].walType)
                        {
                            case Wall.WallType.Door:
                                gameObject = doorPrefabs[rnd.Next(doorPrefabs.Length)];
                                break;
                            case Wall.WallType.Window:
                                gameObject = windowPrefabs[rnd.Next(windowPrefabs.Length)];
                                break;
                            default:
                                gameObject = wallPrefabs[rnd.Next(wallPrefabs.Length)];
                                break;
                        }
                        var wall = Instantiate(gameObject, new Vector3(room.position.x * cellUnitSize, floor.FloorNumber * cellUnitSize, room.position.y * cellUnitSize), Quaternion.Euler(0, 90 * k, 0));
                        wall.transform.parent = transform;
                    }

                    if(room.hasRoof)
                    {
                        var roof = Instantiate(roofPrefabs[rnd.Next(roofPrefabs.Length)], new Vector3(room.position.x * cellUnitSize, floor.FloorNumber + 1.5f * cellUnitSize, room.position.y * cellUnitSize), Quaternion.Euler(-90, 270, 0));
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
        Normal,
        Door,
        Window
    }

    public WallType walType;

    public Wall(WallType walType = WallType.Normal)
    {
        this.walType = walType;
    }
}

public class Room
{
    public Vector2 position;
    public Wall[] walls;
    public bool hasRoof;

    public Room(Vector2 position, Wall[] walls, bool hasRoof = false)
    {
        this.position = position;
        this.walls = walls;
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