using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TileCity;

public class TileCity : Tile
{
    public Vector2 position;

    private GameObject[] nonBuildingPrefabs;
    private GameObject nonBuildingsObj;
    private GameObject buildingsObj;
    private GameObject roadsObj;

    private Vector2Int coord;
    private CityData cityData;

    public const int UNITS_PER_ROAD_ITEM = 5;
    public const int DEFAULT_CITY_FREQUENCY = 50;

    static public TileCity GenerateTile(Vector2Int coordinates, int widthOfRegion, int lengthOfRegion, Transform parent, RoadItem[,] roadItems, CityItem[,] cityItems, CityData cityData)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);

        TileCity chunk = new TileCity(coordinates, viewedChunkCoord, parent, 100f, cityData);
        chunk.PlaceRoadItems(roadItems);
        chunk.PlaceNonRoads(cityItems);

        return chunk;
    }

    static public RoadItem[,] GenerateRoadMap(int width, int length)
    {
        RoadItem[,] roadMap = new RoadItem[width, length];
        int x = width / 2;
        int y = length / 2;
        roadMap[x, y] = RoadItem.CreateStartRoadItem();

        Queue<Vector2Int> queueToProcess = new Queue<Vector2Int>();
        if (x + 1 < width)
        {
            queueToProcess.Enqueue(new Vector2Int(x + 1, y));
        }
        if (x - 1 >= 0)
        {
            queueToProcess.Enqueue(new Vector2Int(x - 1, y));
        }
        if(y + 1 < length)
        {
            queueToProcess.Enqueue(new Vector2Int(x, y + 1));
        }
        if (y - 1 >= 0)
        {
            queueToProcess.Enqueue(new Vector2Int(x, y - 1));
        }

        while (queueToProcess.Count != 0)
        {
            Queue<Vector2Int> queueToProcessNext = new Queue<Vector2Int>();
            while (queueToProcess.Count != 0)
            {
                Vector2Int v = queueToProcess.Dequeue();
                RoadItem roadItem = roadMap[v.x, v.y];
                if (roadItem.IsProcessed())
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

    static public CityItem[,] GenerateCityItemsMap(RoadItem[,] roadItems, CityData cityData)
    {
        CityItem[,] cityItems = new CityItem[roadItems.GetLength(0) * UNITS_PER_ROAD_ITEM, roadItems.GetLength(1) * UNITS_PER_ROAD_ITEM];
        for (int i = 0; i < roadItems.GetLength(1); ++i)
        {
            for (int j = 0; j < roadItems.GetLength(0); ++j)
            {
                RoadItem roadItem = roadItems[j, i];
                if (roadItem.IsUpRoad())
                {
                    for (int k = 1; k < 5; ++k)
                    {
                        cityItems[j * 5 + k, i * 5 + 1] = CityItem.CreateRoad();
                        cityItems[j * 5 + k, i * 5 + 2] = CityItem.CreateRoad();
                        cityItems[j * 5 + k, i * 5 + 3] = CityItem.CreateRoad();
                    }
                }
                if (roadItem.IsDownRoad())
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        cityItems[j * 5 + k, i * 5 + 1] = CityItem.CreateRoad();
                        cityItems[j * 5 + k, i * 5 + 2] = CityItem.CreateRoad();
                        cityItems[j * 5 + k, i * 5 + 3] = CityItem.CreateRoad();
                    }
                }
                if (roadItem.IsRightRoad())
                {
                    for (int k = 1; k < 5; ++k)
                    {
                        cityItems[j * 5 + 1, i * 5 + k] = CityItem.CreateRoad();
                        cityItems[j * 5 + 2, i * 5 + k] = CityItem.CreateRoad();
                        cityItems[j * 5 + 3, i * 5 + k] = CityItem.CreateRoad();
                    }
                }
                if (roadItem.IsLeftRoad())
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        cityItems[j * 5 + 1, i * 5 + k] = CityItem.CreateRoad();
                        cityItems[j * 5 + 2, i * 5 + k] = CityItem.CreateRoad();
                        cityItems[j * 5 + 3, i * 5 + k] = CityItem.CreateRoad();
                    }
                }
            }
        }
        for (int i = 0; i < cityItems.GetLength(1); ++i)
        {
            for (int j = 0; j < cityItems.GetLength(0); ++j)
            {
                if (cityItems[j, i].type == CityItemType.Road)
                {
                    continue;
                }
                if (i > 0 && cityItems[j, i - 1].type == CityItemType.Building)
                {
                    if (j > 0 && cityItems[j - 1, i - 1].type == CityItemType.Building)
                    {
                        cityItems[j, i] = cityItems[j - 1, i].type == CityItemType.Building ? CityItem.CreateBuilding(cityItems[j - 1, i].height) : CityItem.CreateNone();
                    }
                    else // j > 0 && cityItems[j - 1, i - 1] != CityItem.Building || j == 0
                    {
                        int lengthIt = j;
                        while (lengthIt < cityItems.GetLength(0) && cityItems[lengthIt, i - 1].type == CityItemType.Building)
                        {
                            ++lengthIt;
                        }
                        bool foundRoad = false;
                        for (int k = j; k < lengthIt; ++k)
                        {
                            if (cityItems[k, i].type == CityItemType.Road)
                            {
                                foundRoad = true;
                                break;
                            }
                        }
                        if (foundRoad)
                        {
                            cityItems[j, i] = CityItem.CreateNone();
                        }
                        else
                        {
                            cityItems[j, i] = UnityEngine.Random.Range(0f, 1f) < cityData.buildingChance ? CityItem.CreateBuilding(cityItems[j, i - 1].height) : CityItem.CreateNone();
                        }
                    }
                }
                else if (i > 0 && cityItems[j, i - 1].type != CityItemType.Building)
                {
                    if (j < cityItems.GetLength(0) - 1 && cityItems[j + 1, i - 1].type == CityItemType.Building || j > 0 && cityItems[j - 1, i - 1].type == CityItemType.Building)
                    {
                        cityItems[j, i] = CityItem.CreateNone();
                    }
                    else
                    {
                        int height = j > 1 && cityItems[j - 1, i].type == CityItemType.Building ? cityItems[j - 1, i].height : UnityEngine.Random.Range(1, cityData.maxNumberOfFloors + 1);
                        cityItems[j, i] = UnityEngine.Random.Range(0f, 1f) < cityData.buildingChance ? CityItem.CreateBuilding(height) : CityItem.CreateNone();
                    }
                }
                else // i == 0
                {
                    int height = j > 1 && cityItems[j - 1, i].type == CityItemType.Building ? cityItems[j - 1, i].height : UnityEngine.Random.Range(1, cityData.maxNumberOfFloors + 1);
                    cityItems[j, i] = UnityEngine.Random.Range(0f, 1f) < cityData.buildingChance ? CityItem.CreateBuilding(height) : CityItem.CreateNone();
                }
                if (cityItems[j, i].type == CityItemType.None)
                {
                    cityItems[j, i] = UnityEngine.Random.Range(0f, 1f) < cityData.nonBuildingChance ? CityItem.CreateNonBuilding() : CityItem.CreateNone();
                }
            }
        }
        return cityItems;
    }

    private TileCity(Vector2Int coord, Vector2 viewedCoord, Transform parent, float sizeScale, CityData cityData)
    {
        this.cityData = cityData;

        heightMap = new float[Noise.NOISE_MAP_WIDTH, Noise.NOISE_MAP_WIDTH];
        this.coord = coord;

        Vector3 position3 = new Vector3(viewedCoord.x, 0, viewedCoord.y) * sizeScale + new Vector3(parent.transform.position.x, 0, parent.transform.position.z);

        meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        meshObject.name = "City Tile";
        MeshRenderer planeMeshRenderer = meshObject.GetComponent<MeshRenderer>();
        Material planeMaterial = new Material(Shader.Find("Standard"));
        planeMaterial.color = cityData.sidewalkColor;
        planeMeshRenderer.sharedMaterial = planeMaterial;

        meshObject.transform.parent = parent;

        meshObject.transform.localPosition = position3;
        meshObject.transform.localRotation = Quaternion.identity;
        meshObject.transform.localScale = Vector3.one;

        meshObject.transform.localScale = Vector3.one * sizeScale / 10f;

        meshObject.SetActive(true);

        nonBuildingPrefabs = Utils.ReadPrefabs("Assets\\Prefabs\\NonBuildings");

        nonBuildingsObj = new GameObject("Non-buildings object");
        nonBuildingsObj.transform.parent = meshObject.transform;
        nonBuildingsObj.transform.localPosition = Vector3.zero;

        buildingsObj = new GameObject("Buildings object");
        buildingsObj.transform.parent = meshObject.transform;
        buildingsObj.transform.localPosition = Vector3.zero;

        roadsObj = new GameObject("Roads object");
        roadsObj.transform.parent = meshObject.transform;
        roadsObj.transform.localPosition = Vector3.zero;
    }

    private void PlaceRoadItems(RoadItem[,] roadItems)
    {
        int offsetI = coord.x * cityData.cityFrequency / UNITS_PER_ROAD_ITEM;
        int offsetJ = coord.y * cityData.cityFrequency / UNITS_PER_ROAD_ITEM;
        for (int i = 0; i < cityData.cityFrequency / (int)UNITS_PER_ROAD_ITEM; ++i)
        {
            for (int j = 0; j < cityData.cityFrequency / (int)UNITS_PER_ROAD_ITEM; ++j)
            {
                GameObject road = InstantiateRoadItem(roadItems[j + offsetJ, i + offsetI], "Assets\\Prefabs\\Roads", roadsObj.transform);
                if (road != null)
                {
                    float scale = (float)DEFAULT_CITY_FREQUENCY / cityData.cityFrequency;
                    road.transform.localPosition = new Vector3(scale * (2f * UNITS_PER_ROAD_ITEM * i - (cityData.cityFrequency - UNITS_PER_ROAD_ITEM)), 0, scale * (2f * UNITS_PER_ROAD_ITEM * j - (cityData.cityFrequency - UNITS_PER_ROAD_ITEM)));
                    road.transform.localScale = Vector3.one * UNITS_PER_ROAD_ITEM * scale;
                }
            }
        }
        Utils.MergeChildMeshesByMaterialColor(roadsObj);
    }

    private CityItem getCityItemWithOffset(CityItem[,] cityItems, int iOffset, int jOffset, int i, int j)
    {
        return cityItems[jOffset + j, iOffset + i];
    }

    private void PlaceNonRoads(CityItem[,] cityItems)
    {
        int offsetI = coord.x * cityData.cityFrequency;
        int offsetJ = coord.y * cityData.cityFrequency;
        for (int i = 0; i < cityData.cityFrequency; ++i)
        {
            for (int j = 0; j < cityData.cityFrequency; ++j)
            {
                if (getCityItemWithOffset(cityItems, offsetI, offsetJ, i, j).type == CityItemType.Building)
                {
                    if (j > 0 && getCityItemWithOffset(cityItems, offsetI, offsetJ, i, j - 1).type == CityItemType.Building || i > 0 && getCityItemWithOffset(cityItems, offsetI, offsetJ, i - 1, j).type == CityItemType.Building) {
                        continue;
                    }
                    int endJ = j;
                    int endI = i;
                    while(endJ + 1 < cityData.cityFrequency && getCityItemWithOffset(cityItems, offsetI, offsetJ, i, endJ + 1).type == CityItemType.Building)
                    {
                        ++endJ;
                    }
                    while (endI + 1 < cityData.cityFrequency && getCityItemWithOffset(cityItems, offsetI, offsetJ, endI + 1, j).type == CityItemType.Building)
                    {
                        ++endI;
                    }
                    GameObject buildingObj = new GameObject();
                    Building building = buildingObj.AddComponent<Building>();
                    float scale = (float)DEFAULT_CITY_FREQUENCY / cityData.cityFrequency;
                    building.Initialize(cityData.floorSizePolicy, "Assets/Prefabs/MiddleDetailed", endI - i + 1, endJ - j + 1, getCityItemWithOffset(cityItems, offsetI, offsetJ, i, j).height, 0.75f, 2f);
                    building.ReadPrefabs();
                    building.Generate();
                    building.Render();
                    buildingObj.transform.parent = buildingsObj.transform;
                    buildingObj.transform.localPosition = new Vector3(scale * (2f * (endI + i) / 2f - (cityData.cityFrequency - 1)), 0, scale * (2f * (endJ + j) / 2f - (cityData.cityFrequency - 1)));
                    buildingObj.transform.localScale = Vector3.one * scale;
                }
                else if (getCityItemWithOffset(cityItems, offsetI, offsetJ, i, j).type == CityItemType.NonBuilding) {
                    GameObject gameObject = nonBuildingPrefabs[UnityEngine.Random.Range(0, nonBuildingPrefabs.Length)];
                    var obj = GameObject.Instantiate(gameObject, Vector3.zero, Quaternion.identity, nonBuildingsObj.transform);
                    float scale = (float)DEFAULT_CITY_FREQUENCY / cityData.cityFrequency;
                    obj.transform.localPosition = new Vector3(scale * (2f * i - (cityData.cityFrequency - 1)), 0, scale * (2f * j - (cityData.cityFrequency - 1)));
                    obj.transform.localScale = obj.transform.localScale * scale;
                }
            }
        }
        Utils.MergeChildMeshesByMaterialColor(buildingsObj);
        Utils.MergeChildMeshesByMaterialColor(nonBuildingsObj);
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

public enum CityItemType
{
    None,
    Building,
    Road,
    NonBuilding,
}

public struct CityItem
{
    public CityItemType type;
    public int height;

    static public CityItem CreateRoad()
    {
        return new CityItem() { type = CityItemType.Road, height = 0 };
    }

    static public CityItem CreateNonBuilding()
    {
        return new CityItem() { type = CityItemType.NonBuilding, height = 0 };
    }

    static public CityItem CreateNone()
    {
        return new CityItem() { type = CityItemType.None, height = 0 };
    }

    static public CityItem CreateBuilding(int buildingHeight)
    {
        return new CityItem() { type = CityItemType.Building, height = buildingHeight };
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