using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileCity : Tile
{
    public Vector2 position;
    RoadItem[,] roadItems;
    CityItem[,] cityItems;
    float buildingChance;
    float nonBuildingChance;
    int maxFloor;
    Building.FloorSizePolicy floorSizePolicy;

    private GameObject[] nonBuildingPrefabs;

    static public TileCity GenerateTile(Vector2 coordinates, int widthOfRegion, int lengthOfRegion, Transform transform, Color sidewalkColor, float buildingChance, float nonBuildingChance, int maxFloor, Building.FloorSizePolicy floorSizePolicy)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);

        TileCity chunk = new TileCity(viewedChunkCoord, 240, transform, 100f, sidewalkColor, buildingChance, nonBuildingChance, maxFloor, floorSizePolicy);
        chunk.PlaceRoadItems();
        chunk.GenerateMap();
        chunk.Render();

        return chunk;
    }

    private TileCity(Vector2 coord, int size, Transform parent, float sizeScale, Color sidewalkColor, float buildingChance, float nonBuildingChance, int maxFloor, Building.FloorSizePolicy floorSizePolicy)
    {
        this.buildingChance = buildingChance;
        this.nonBuildingChance = nonBuildingChance;
        this.maxFloor = maxFloor;
        this.floorSizePolicy = floorSizePolicy;

        Vector3 position3 = new Vector3(coord.x, 0, coord.y) * sizeScale + new Vector3(parent.transform.position.x, 0, parent.transform.position.z);

        meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        meshObject.name = "City Tile";
        MeshRenderer planeMeshRenderer = meshObject.GetComponent<MeshRenderer>();
        Material planeMaterial = new Material(Shader.Find("Standard"));
        planeMaterial.color = sidewalkColor;
        planeMeshRenderer.sharedMaterial = planeMaterial;

        meshObject.transform.parent = parent;

        meshObject.transform.localPosition = position3;
        meshObject.transform.localRotation = Quaternion.identity;

        meshObject.transform.localScale = Vector3.one * sizeScale / 10f;

        meshObject.SetActive(true);

        this.nonBuildingPrefabs = Utils.ReadPrefabs("Assets\\Prefabs\\NonBuildings");
    }

    private void PlaceRoadItems()
    {
        roadItems = GenerateRoadMap(10, 10);
        float offset = -4.5f;
        for (int i = 0; i < roadItems.GetLength(0); ++i)
        {
            for (int j = 0; j < roadItems.GetLength(1); ++j)
            {
                GameObject road = InstantiateRoadItem(roadItems[j, i], "Assets\\Prefabs\\Roads", meshObject.transform);
                if (road != null)
                {
                    road.transform.localPosition = new Vector3(i + offset, 0, j + offset);
                    road.transform.localScale = Vector3.one * 0.5f;
                }
            }
        }
    }

    public enum CityItem
    {
        None,
        Building,
        Road,
        NonBuilding,
    }

    private void GenerateMap()
    {
        cityItems = new CityItem[50, 50];
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                RoadItem roadItem = roadItems[j, i];
                if(roadItem.IsUpRoad())
                {
                    for(int k = 1; k < 5; ++k)
                    {
                        cityItems[j * 5 + k, i * 5 + 1] = CityItem.Road;
                        cityItems[j * 5 + k, i * 5 + 2] = CityItem.Road;
                        cityItems[j * 5 + k, i * 5 + 3] = CityItem.Road;
                    }
                }
                if (roadItem.IsDownRoad())
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        cityItems[j * 5 + k, i * 5 + 1] = CityItem.Road;
                        cityItems[j * 5 + k, i * 5 + 2] = CityItem.Road;
                        cityItems[j * 5 + k, i * 5 + 3] = CityItem.Road;
                    }
                }
                if(roadItem.IsRightRoad()) { 
                    for (int k = 1; k < 5; ++k)
                    {
                        cityItems[j * 5 + 1, i * 5 + k] = CityItem.Road;
                        cityItems[j * 5 + 2, i * 5 + k] = CityItem.Road;
                        cityItems[j * 5 + 3, i * 5 + k] = CityItem.Road;
                    }
                }
                if (roadItem.IsLeftRoad())
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        cityItems[j * 5 + 1, i * 5 + k] = CityItem.Road;
                        cityItems[j * 5 + 2, i * 5 + k] = CityItem.Road;
                        cityItems[j * 5 + 3, i * 5 + k] = CityItem.Road;
                    }
                }
            }
        }
        for(int i = 0; i < cityItems.GetLength(0); ++i)
        {
            for (int j = 0; j < cityItems.GetLength(1); ++j)
            {
                if (cityItems[j, i] == CityItem.Road)
                {
                    continue;
                }
                if (j > 0 && i > 0 && cityItems[j - 1, i - 1] == CityItem.Building || j > 0 && i < cityItems.GetLength(0) - 1 && cityItems[j - 1, i + 1] == CityItem.Building ||
                    j < cityItems.GetLength(1) - 1 && i > 0 && cityItems[j + 1, i - 1] == CityItem.Building || j < cityItems.GetLength(1) - 1 && i < cityItems.GetLength(0) - 1 && cityItems[j + 1, i + 1] == CityItem.Building)
                {
                    cityItems[j, i] = CityItem.NonBuilding;
                }
                else
                {
                    cityItems[j, i] = UnityEngine.Random.Range(0f, 1f) < buildingChance ? CityItem.Building : CityItem.NonBuilding;
                }
                if (cityItems[j, i] == CityItem.NonBuilding)
                {
                    cityItems[j, i] = UnityEngine.Random.Range(0f, 1f) < nonBuildingChance ? CityItem.NonBuilding : CityItem.None;
                }
            }
        }
    }

    private void Render()
    {
        for (int i = 0; i < 50; ++i)
        {
            for (int j = 0; j < 50; ++j)
            {
                if (cityItems[j, i] == CityItem.Building)
                {
                    if (j > 0 && cityItems[j - 1, i] == CityItem.Building || i > 0 && cityItems[j, i - 1] == CityItem.Building) {
                        continue;
                    }
                    int endJ = j;
                    int endI = i;
                    while(endJ + 1 < 50 && cityItems[endJ + 1, i] == CityItem.Building)
                    {
                        ++endJ;
                    }
                    while (endI + 1 < 50 && cityItems[j, endI + 1] == CityItem.Building)
                    {
                        ++endI;
                    }
                    GameObject buildingObj = new GameObject();
                    Building building = buildingObj.AddComponent<Building>();
                    building.Initialize(floorSizePolicy, "Assets/Prefabs/MiddleDetailed", endI - i + 1, endJ - j + 1, UnityEngine.Random.Range(1, maxFloor + 1), 0.75f, 2f);
                    building.ReadPrefabs();
                    building.Generate();
                    building.Render();
                    buildingObj.transform.parent = meshObject.transform;
                    buildingObj.transform.localPosition = new Vector3(0.2f * (endI + i) / 2f - 4.9f, 0, 0.2f * (endJ + j) / 2f - 4.9f);
                }
                else if (cityItems[j, i] == CityItem.NonBuilding) {
                    GameObject gameObject = nonBuildingPrefabs[UnityEngine.Random.Range(0, nonBuildingPrefabs.Length)];
                    var obj = GameObject.Instantiate(gameObject, Vector3.zero, Quaternion.identity, meshObject.transform);
                    obj.transform.localPosition = new Vector3(-4.9f + 0.2f * i, 0, -4.9f + 0.2f * j);
                    obj.transform.localScale = Vector3.one * 0.125f;
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

    public GameObject InstantiateRoadItem(RoadItem roadItem, String pathToRoads, Transform parent)
    {
        GameObject road = null;
        int numberOfRoads = Convert.ToInt32(roadItem.IsUpRoad()) + Convert.ToInt32(roadItem.IsDownRoad()) + Convert.ToInt32(roadItem.IsRightRoad()) + Convert.ToInt32(roadItem.IsLeftRoad());
        if (numberOfRoads == 1)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadend.prefab");
            road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
            if (roadItem.IsUpRoad())
            {
                road.transform.localRotation = Quaternion.identity;
            }
            else if (roadItem.IsRightRoad())
            {
                road.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (roadItem.IsDownRoad())
            {
                road.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (roadItem.IsLeftRoad())
            {
                road.transform.localRotation = Quaternion.Euler(0, 270, 0);
            }
        }
        else if (numberOfRoads == 2)
        {
            if (roadItem.IsUpRoad() && roadItem.IsDownRoad())
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadstraight.prefab");
                road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
            }
            else if (roadItem.IsLeftRoad() && roadItem.IsRightRoad())
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadstraight.prefab");
                road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
                road.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadturn.prefab");
                road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
                if (roadItem.IsUpRoad() && roadItem.IsRightRoad())
                {
                    road.transform.localRotation = Quaternion.Euler(0, 270, 0);
                }
                else if (roadItem.IsUpRoad() && roadItem.IsLeftRoad())
                {
                    road.transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                else if (roadItem.IsDownRoad() && roadItem.IsRightRoad())
                {
                    road.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if (roadItem.IsDownRoad() && roadItem.IsLeftRoad())
                {
                    road.transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
            }
        }
        else if (numberOfRoads == 3)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(pathToRoads + "\\roadtcross.prefab");
            road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
            if (!roadItem.IsUpRoad())
            {
                road.transform.localRotation = Quaternion.identity;
            }
            else if (!roadItem.IsRightRoad())
            {
                road.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (!roadItem.IsDownRoad())
            {
                road.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (!roadItem.IsLeftRoad())
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