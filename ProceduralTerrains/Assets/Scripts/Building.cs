using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Building : MonoBehaviour
{
    private GameObject[] wallPrefabs;

    private GameObject[] windowPrefabs;

    private GameObject[] roofPrefabs;

    private GameObject[] doorPrefabs;

    private GameObject defaultRoofPrefab;

    public enum FloorSizePolicy
    {
        Constant,
        Decreasing
    }

    [SerializeField]
    private FloorSizePolicy floorSizePolicy = FloorSizePolicy.Constant;

    [SerializeField]
    private string prefabsPath;

    [SerializeField]
    private int width = 3;

    [SerializeField]
    private int length = 3;

    [SerializeField]
    private float cellUnitSize = 2f;

    [SerializeField]
    private int numberOfFloors;

    [SerializeField]
    private float windowChance;

    private Floor[] floors;

    public void Initialize(FloorSizePolicy floorSizePolicy, string prefabsPath, int width, int length, int numberOfFloors, float windowChance, float cellUnitSize)
    {
        this.floorSizePolicy = floorSizePolicy;
        this.prefabsPath = prefabsPath;
        this.width = width;
        this.length = length;
        this.numberOfFloors = numberOfFloors;
        this.windowChance = windowChance;
        this.cellUnitSize = cellUnitSize;
    }

    public void ReadPrefabs()
    {
        wallPrefabs = ReadPrefabs(prefabsPath + "\\Walls");
        windowPrefabs = ReadPrefabs(prefabsPath + "\\Windows");
        roofPrefabs = ReadPrefabs(prefabsPath + "\\Roofs");
        doorPrefabs = ReadPrefabs(prefabsPath + "\\Doors");
        defaultRoofPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "\\Roofs\\Default.prefab");
        if(defaultRoofPrefab == null)
        {
            throw new Exception("Roofs folder must contain Default.prefab");
        }
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
        int doorWallNumber = UnityEngine.Random.Range(0, length * 2 + width * 2);
        int findDoorCounter = 0;

        floors = new Floor[numberOfFloors];
        int floorCount = 0;

        int[] bounds = new int[] {0, width - 1, 0, length - 1};

        foreach (Floor floor in floors)
        {
            int[] nextBounds = new int[4];
            Array.Copy(bounds, nextBounds, nextBounds.Length);

            if (floorSizePolicy == FloorSizePolicy.Decreasing)
            {
                if (UnityEngine.Random.Range(0f, 1f) > 0.75 && bounds[1] - bounds[0] > 1)
                {
                    ++nextBounds[0];
                }
                if (UnityEngine.Random.Range(0f, 1f) > 0.75 && bounds[1] - bounds[0] > 1)
                {
                    --nextBounds[1];
                }
                if (UnityEngine.Random.Range(0f, 1f) > 0.75 && bounds[3] - bounds[2] > 1)
                {
                    ++nextBounds[2];
                }
                if (UnityEngine.Random.Range(0f, 1f) > 0.75 && bounds[3] - bounds[2] > 1)
                {
                    --nextBounds[3];
                }
            }

            Room[,] rooms = new Room[width, length];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (i < bounds[0] || i > bounds[1] || j < bounds[2] || j > bounds[3])
                    {
                        rooms[i, j] = new Room(new Vector2(i, j), null, Room.RoofType.None, Room.RoomType.Blank);
                        continue;
                    }
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
                        if (UnityEngine.Random.Range(0f, 1f) < windowChance)
                        {
                            walls[k] = new Wall(Wall.WallType.Window);
                        }
                        else
                        {
                            walls[k] = new Wall();
                        }
                    }
                    Room.RoofType roofType = Room.RoofType.None;
                    if(floorCount == numberOfFloors - 1 ||
                        (i < nextBounds[0] || i > nextBounds[1] || j < nextBounds[2] || j > nextBounds[3]))
                    {
                        roofType = floorCount == floors.Length - 1 ? Room.RoofType.Random : Room.RoofType.Default;
                    }
                    rooms[i, j] = new Room(new Vector2(i, j), walls, roofType);
                }
            }
            floors[floorCount] = new Floor(floorCount++, rooms);

            bounds = nextBounds;
        }
        Optimize();
    }

    private void Optimize()
    {
        for (int f = 0; f < floors.Length - 1; ++f)
        {
            Floor floor = floors[f];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < length; ++j)
                {
                    if(i == 0 || j == 0 || i == width - 1 || j == length - 1)
                    {
                        if (i > 0 && floor.rooms[i - 1, j].roomType != Room.RoomType.Blank)
                        {
                            floor.rooms[i, j].walls[0].walType = Wall.WallType.Blank;
                        }
                        if (i < width - 1 && floor.rooms[i + 1, j].roomType != Room.RoomType.Blank)
                        {
                            floor.rooms[i, j].walls[2].walType = Wall.WallType.Blank;
                        }
                        if (j > 0 && floor.rooms[i, j - 1].roomType != Room.RoomType.Blank)
                        {
                            floor.rooms[i, j].walls[3].walType = Wall.WallType.Blank;
                        }
                        if (j < length - 1 && floor.rooms[i, j + 1].roomType != Room.RoomType.Blank)
                        {
                            floor.rooms[i, j].walls[1].walType = Wall.WallType.Blank;
                        }
                    }
                    else if (!(floor.rooms[i - 1, j].roomType == Room.RoomType.Blank || floor.rooms[i + 1, j].roomType == Room.RoomType.Blank
                        || floor.rooms[i, j - 1].roomType == Room.RoomType.Blank || floor.rooms[i, j + 1].roomType == Room.RoomType.Blank
                        || floors[f].rooms[i, j].roomType == Room.RoomType.Blank))
                    {
                        floor.rooms[i, j].roomType = Room.RoomType.Internal;
                        continue;
                    }
                }
            }
        }
    }

    public void Render()
    {
        float offsetX = 0.5f * cellUnitSize * (1f - width);
        float offsetY = 0.5f * cellUnitSize * (1f - length);
        foreach (Floor floor in floors)
        {
            for(int i = 0; i < width; ++i)
            {
                for (int j = 0; j < length; ++j)
                {
                    Room room = floor.rooms[i, j];
                    if (room.roomType == Room.RoomType.Blank || room.roomType == Room.RoomType.Internal)
                    {
                        continue;
                    }

                    Wall[] walls = room.walls;
                    for (int k = 0; k < 4; k++)
                    {
                        GameObject gameObject;
                        switch (walls[k].walType)
                        {
                            case Wall.WallType.Door:
                                gameObject = doorPrefabs[UnityEngine.Random.Range(0, doorPrefabs.Length)];
                                break;
                            case Wall.WallType.Window:
                                gameObject = windowPrefabs[UnityEngine.Random.Range(0, windowPrefabs.Length)];
                                break;
                            case Wall.WallType.Normal:
                                gameObject = wallPrefabs[UnityEngine.Random.Range(0, wallPrefabs.Length)];
                                break;
                            default:
                                continue;
                        }
                        var wall = Instantiate(gameObject, Vector3.zero, Quaternion.identity, transform);
                        wall.transform.localPosition = new Vector3(offsetX + room.position.x * cellUnitSize, (floor.FloorNumber + 0.5f) * cellUnitSize, offsetY + room.position.y * cellUnitSize);
                        wall.transform.localRotation = Quaternion.Euler(0, 90 * k, 0);
                        wall.transform.localScale = wall.transform.localScale * cellUnitSize / 2f;
                    }

                    // var roomFloor = Instantiate(floorPrefabs[UnityEngine.Random.Range(0, floorPrefabs.Length)], new Vector3(room.position.x * cellUnitSize, (floor.FloorNumber) * cellUnitSize, room.position.y * cellUnitSize), Quaternion.Euler(-90, 270, 0), transform);
                    // roomFloor.transform.localScale = roomFloor.transform.localScale * cellUnitSize / 2f;

                    if (room.roofType != Room.RoofType.None)
                    {
                        GameObject roofObj = room.roofType == Room.RoofType.Random ? roofPrefabs[UnityEngine.Random.Range(0, roofPrefabs.Length)] : defaultRoofPrefab;
                        var roof = Instantiate(roofObj, Vector3.zero, Quaternion.identity, transform);
                        roof.transform.localPosition = new Vector3(offsetX + room.position.x * cellUnitSize, (floor.FloorNumber + 1f) * cellUnitSize, offsetY + room.position.y * cellUnitSize);
                        // roof.transform.localRotation = Quaternion.Euler(0, 270, 0);
                        roof.transform.localScale = roof.transform.localScale * cellUnitSize / 2f;
                    }
                }
            }
        }
        MergeChildMeshesByMaterialColor();
    }

    public void Clear()
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    public int maxVerticesPerObject = 65536;
    public void MergeChildMeshesByMaterialColor()
    {
        MeshFilter[] childMeshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true);

        // Group child meshes by material color
        Dictionary<Color, List<MeshFilter>> meshGroups = new Dictionary<Color, List<MeshFilter>>();
        foreach (MeshFilter childMeshFilter in childMeshFilters)
        {
            Material meshMaterial = childMeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
            Color materialColor = meshMaterial.color;

            if (!meshGroups.ContainsKey(materialColor))
            {
                meshGroups[materialColor] = new List<MeshFilter>();
            }
            meshGroups[materialColor].Add(childMeshFilter);
        }

        // Merge meshes for each group
        foreach (var group in meshGroups)
        {
            int totalVertices = 0;
            List<MeshFilter> meshes = group.Value;

            List<CombineInstance> combineInstances = new List<CombineInstance>();
            for (int i = 0; i < meshes.Count; i++)
            {
                MeshFilter meshFilter = meshes[i];
                int meshVertices = meshFilter.sharedMesh.vertexCount;

                if (totalVertices + meshVertices > maxVerticesPerObject)
                {
                    CreateMergedObject(combineInstances, meshes[0].GetComponent<MeshRenderer>().sharedMaterial);
                    combineInstances.Clear();
                    totalVertices = 0;
                }

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = meshFilter.sharedMesh;
                combineInstance.transform = meshFilter.transform.localToWorldMatrix;
                combineInstances.Add(combineInstance);
                meshFilter.gameObject.SetActive(false);

                totalVertices += meshVertices;
            }

            CreateMergedObject(combineInstances, meshes[0].GetComponent<MeshRenderer>().sharedMaterial);
        }
    }

    private void CreateMergedObject(List<CombineInstance> combineInstances, Material material)
    {
        if(combineInstances.Count == 0)
        {
            return;
        }
        GameObject mergedObject = new GameObject("MergedObject");
        mergedObject.transform.parent = gameObject.transform;
        //mergedObject.transform.localPosition = Vector3.zero;
        mergedObject.transform.localRotation = Quaternion.identity;
        mergedObject.transform.localScale = Vector3.one;

        MeshFilter mergedMeshFilter = mergedObject.AddComponent<MeshFilter>();
        MeshRenderer mergedMeshRenderer = mergedObject.AddComponent<MeshRenderer>();
        mergedMeshRenderer.material = material;

        mergedMeshFilter.sharedMesh = new Mesh();
        mergedMeshFilter.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        mergedObject.SetActive(true);
    }
}

public class Wall
{
    public enum WallType
    {
        Normal,
        Door,
        Window,
        Blank
    }

    public WallType walType;

    public Wall(WallType walType = WallType.Normal)
    {
        this.walType = walType;
    }
}

public class Room
{
    public enum RoofType
    {
        None,
        Default,
        Random
    }

    public Vector2 position;
    public Wall[] walls;
    public RoofType roofType;
    public RoomType roomType;

    public enum RoomType
    {
        Normal,
        Internal,
        Blank
    }

    public Room(Vector2 position, Wall[] walls, RoofType roofType = RoofType.None, RoomType roomType = RoomType.Normal)
    {
        this.position = position;
        this.walls = walls;
        this.roofType = roofType;
        this.roomType = roomType;
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