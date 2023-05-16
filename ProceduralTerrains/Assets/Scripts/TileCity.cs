using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class TileCity : Tile
{
    public Vector2 position;
    Building building;

    static public TileCity GenerateTile(Vector2 coordinates, int widthOfRegion, int lengthOfRegion, Transform transform)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);

        TileCity chunk = new TileCity(viewedChunkCoord, 240, transform, 100f);
        chunk.placeRoadItems();
        return chunk;
    }

    private TileCity(Vector2 coord, int size, Transform parent, float sizeScale)
    {
        Vector3 position3 = new Vector3(coord.x, 0, coord.y) * sizeScale + new Vector3(parent.transform.position.x, 0, parent.transform.position.z);

        meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        meshObject.name = "City Tile";

        meshObject.transform.parent = parent;

        meshObject.transform.localPosition = position3;
        meshObject.transform.localRotation = Quaternion.identity;

        // Scale the plane
        meshObject.transform.localScale = Vector3.one * sizeScale / 10f;

        GameObject buildingObj = new GameObject("Building");

        building = buildingObj.AddComponent<Building>();
        buildingObj.transform.parent = meshObject.transform;
        buildingObj.transform.localPosition = new Vector3(0, 0, 0);

        building.Initialize(Building.FloorSizePolicy.Constant, "Assets/Prefabs", 3, 3, 5, 0.75f, 1f);
        building.ReadPrefabs();
        building.Generate();
        building.Render();

        meshObject.SetActive(true);
    }

    private void placeRoadItems()
    {
        RoadItem[,] roadItems = GenerateRoadMap(10, 10);
        float offset = -4.5f;
        for (int i = 0; i < roadItems.GetLength(0); ++i)
        {
            for (int j = 0; j < roadItems.GetLength(1); ++j)
            {
                GameObject road = RoadItem.InstantiateRoadItem(roadItems[j, i], "Assets\\Prefabs\\Roads", meshObject.transform);
                if (road != null)
                {
                    road.transform.localPosition = new Vector3(i + offset, 0, j + offset);
                    road.transform.localScale = Vector3.one * 0.5f;
                }
            }
        }
    }

    private RoadItem[,] GenerateRoadMap(int width, int length)
    {
        RoadItem[,] roadMap = new RoadItem[width, length];
        int x = width / 2;
        int y = length / 2;
        roadMap[x, y] = RoadItem.CreateStartRoadItem();

        Queue<Vector2Int> queueToProcess = new Queue<Vector2Int>();
        queueToProcess.Enqueue(new Vector2Int(x + 1, y));
        queueToProcess.Enqueue(new Vector2Int(x - 1, y));
        queueToProcess.Enqueue(new Vector2Int(x, y + 1));
        queueToProcess.Enqueue(new Vector2Int(x, y - 1));

        while (queueToProcess.Count != 0)
        {
            Queue<Vector2Int> queueToProcessNext = new Queue<Vector2Int>();
            while (queueToProcess.Count != 0)
            {
                Vector2Int v = queueToProcess.Dequeue();
                RoadItem roadItem = roadMap[v.x, v.y];
                if(roadItem.IsProcessed())
                {
                    continue;
                }
                RoadItem up = v.x + 1 != width ? roadMap[v.x + 1, v.y] : new RoadItem();
                RoadItem right = v.y + 1 != length ? roadMap[v.x, v.y + 1] : new RoadItem();
                RoadItem down = v.x != 0 ? roadMap[v.x - 1, v.y] : new RoadItem();
                RoadItem left = v.y != 0 ? roadMap[v.x, v.y - 1] : new RoadItem();
                roadItem.Process(up, right, down, left);
                roadMap[v.x, v.y] = roadItem;
                if (roadItem.IsUpRoad() && v.x + 1 != width && !up.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x + 1, v.y));
                }
                if (roadItem.IsRightRoad() && v.y + 1 != length && !right.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x, v.y + 1));
                }
                if (roadItem.IsDownRoad() && v.x != 0 && !down.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x - 1, v.y));
                }
                if (roadItem.IsLeftRoad() && v.y != 0 && !left.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x, v.y - 1));
                }
            }
            queueToProcess = queueToProcessNext;
        }
        return roadMap;
    }
}

public struct RoadItem
{
    static public RoadItem CreateStartRoadItem()
    {
        RoadItem roadItem = new RoadItem();
        roadItem.roadUp = true;
        roadItem.roadDown = true;
        roadItem.roadRight = true;
        roadItem.roadLeft = true;
        roadItem.isProcessed = true;
        return roadItem;
    }

    private bool roadUp;
    private bool roadRight;
    private bool roadDown;
    private bool roadLeft;
    private bool isProcessed;
    static private float newRoadChance = 0.5f;

    static public GameObject InstantiateRoadItem(RoadItem roadItem, String pathToRoads, Transform parent)
    {
        GameObject road = null;
        int numberOfRoads = Convert.ToInt32(roadItem.roadUp) + Convert.ToInt32(roadItem.roadDown) + Convert.ToInt32(roadItem.roadRight) + Convert.ToInt32(roadItem.roadLeft);
        if(numberOfRoads == 1)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadend.prefab");
            road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
            if (roadItem.roadUp) { 
                road.transform.localRotation = Quaternion.identity;
            }
            else if (roadItem.roadRight)
            {
                road.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (roadItem.roadDown)
            {
                road.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (roadItem.roadLeft)
            {
                road.transform.localRotation = Quaternion.Euler(0, 270, 0);
            }
        }
        else if (numberOfRoads == 2)
        {
            if (roadItem.roadUp && roadItem.roadDown)
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadstraight.prefab");
                road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
            }
            else if (roadItem.roadLeft && roadItem.roadRight)
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadstraight.prefab");
                road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
                road.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadturn.prefab");
                road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
                if (roadItem.roadUp && roadItem.roadRight)
                {
                    road.transform.localRotation = Quaternion.Euler(0, 270, 0);
                }
                else if (roadItem.roadUp && roadItem.roadLeft)
                {
                    road.transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                else if (roadItem.roadDown && roadItem.roadRight)
                {
                    road.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if (roadItem.roadDown && roadItem.roadLeft)
                {
                    road.transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
            }
        }
        else if (numberOfRoads == 3)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadtcross.prefab");
            road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
            if (!roadItem.roadUp)
            {
                road.transform.localRotation = Quaternion.identity;
            }
            else if (!roadItem.roadRight)
            {
                road.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (!roadItem.roadDown)
            {
                road.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (!roadItem.roadLeft)
            {
                road.transform.localRotation = Quaternion.Euler(0, 270, 0);
            }
        }
        else if (numberOfRoads == 4)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadcross.prefab");
            road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
        }
        return road;
    }

    public RoadItem(bool roadUp = false, bool roadRight = false, bool roadDown = false, bool roadLeft = false, bool isProcessed = false)
    {
        this.roadUp = roadUp;
        this.roadRight = roadRight;
        this.roadDown = roadDown;
        this.roadLeft = roadLeft;
        this.isProcessed = isProcessed;
    }

    public void Process(RoadItem up, RoadItem right, RoadItem down, RoadItem left)
    {
        if (!up.IsProcessed())
        {
            roadUp = UnityEngine.Random.Range(0f, 1f) < newRoadChance;
        }
        else if (up.roadDown)
        {
            roadUp = true;
        }
        if (!down.IsProcessed())
        {
            roadDown = UnityEngine.Random.Range(0f, 1f) < newRoadChance;
        }
        else if (down.roadUp)
        {
            roadDown = true;
        }
        if (!left.IsProcessed())
        {
            roadLeft = UnityEngine.Random.Range(0f, 1f) < newRoadChance;
        }
        else if (left.roadRight)
        {
            roadLeft = true;
        }
        if (!right.IsProcessed())
        {
            roadRight = UnityEngine.Random.Range(0f, 1f) < newRoadChance;
        }
        else if (right.roadLeft)
        {
            roadRight = true;
        }
        isProcessed = true;
    }

    public bool IsProcessed()
    {
        return isProcessed;
    }

    public bool IsUpRoad()
    {
        return roadUp;
    }

    public bool IsDownRoad()
    {
        return roadDown;
    }

    public bool IsRightRoad()
    {
        return roadRight;
    }

    public bool IsLeftRoad()
    {
        return roadLeft;
    }
}