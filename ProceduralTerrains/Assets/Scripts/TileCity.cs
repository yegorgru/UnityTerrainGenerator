using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static TileCity;

public class TileCity : Tile
{
    public enum CityDataPolicy
    {
        Global,
        Local
    }

    private CityDataPolicy cityDataPolicy;

    public Vector2 position;

    private GameObject[] nonBuildingPrefabs;

    private GameObject[] roadCrossPrefabs;
    private GameObject[] roadEndPrefabs;
    private GameObject[] roadStraightPrefabs;
    private GameObject[] roadTCrossPrefabs;
    private GameObject[] roadTurnPrefabs;

    private GameObject nonBuildingsObj;
    private GameObject buildingsObj;
    private GameObject roadsObj;

    private Vector2Int coord;
    private CityData cityData;
    private RoadItem[,] roadItemsLocal;

    public const int UNITS_PER_ROAD_ITEM = 5;
    public const int DEFAULT_CITY_FREQUENCY = 50;

    static public TileCity GenerateTile(Vector2Int coordinates, int widthOfRegion, int lengthOfRegion, Transform parent, RoadItem[,] roadItems, CityItem[,] cityItems, CityData cityData, CityData localCityData)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);

        TileCity chunk = new TileCity(coordinates, viewedChunkCoord, parent, 100f, cityData, localCityData);
        chunk.PlaceRoadItems(roadItems);
        chunk.PlaceNonRoads(cityItems);

        return chunk;
    }

    static public RoadItem[,] GenerateRoadMap(int width, int length, CityData cityData, bool useBounds)
    {
        RoadItem[,] roadMap = new RoadItem[width, length];

        int[] widthBounds = { useBounds ? 0 : 1, useBounds ? width : width - 1 };
        int[] lengthBounds = { useBounds ? 0 : 1, useBounds ? length : length - 1 };

        Queue<Vector2Int> queueToProcess = new Queue<Vector2Int>();
        if (cityData.startRoadItemsNumber == 1)
        {
            int x = width / 2;
            int y = length / 2;
            roadMap[x, y] = new RoadItem();

            queueToProcess.Enqueue(new Vector2Int(x, y));
        }
        else
        {
            for (int i = 0; i < cityData.startRoadItemsNumber; ++i)
            {
                int x = UnityEngine.Random.Range(widthBounds[0], widthBounds[1]);
                int y = UnityEngine.Random.Range(lengthBounds[0], lengthBounds[1]);
                
                roadMap[x, y] = new RoadItem();
                queueToProcess.Enqueue(new Vector2Int(x, y));
            }
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
                RoadItem up = v.x + 1 != widthBounds[1] ? roadMap[v.x + 1, v.y] : new RoadItem();
                RoadItem right = v.y + 1 != lengthBounds[1] ? roadMap[v.x, v.y + 1] : new RoadItem();
                RoadItem down = v.x != widthBounds[0] ? roadMap[v.x - 1, v.y] : new RoadItem();
                RoadItem left = v.y != lengthBounds[0] ? roadMap[v.x, v.y - 1] : new RoadItem();
                roadItem.Process(up, right, down, left, cityData.roadChance);
                roadMap[v.x, v.y] = roadItem;
                if (roadItem.IsUpRoad() && v.x + 1 != widthBounds[1] && !up.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x + 1, v.y));
                }
                if (roadItem.IsRightRoad() && v.y + 1 != lengthBounds[1] && !right.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x, v.y + 1));
                }
                if (roadItem.IsDownRoad() && v.x != widthBounds[0] && !down.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x - 1, v.y));
                }
                if (roadItem.IsLeftRoad() && v.y != lengthBounds[0] && !left.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x, v.y - 1));
                }
            }
            queueToProcess = queueToProcessNext;
        }
        return roadMap;
    }

    static public CityItem[,] GenerateCityItemsMap(RoadItem[,] roadItems, CityData cityData, bool useBounds)
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

        int[] widthBounds = { useBounds ? 0 : 1, useBounds ? cityItems.GetLength(1) : cityItems.GetLength(1) - 1 };
        int[] lengthBounds = { useBounds ? 0 : 1, useBounds ? cityItems.GetLength(0) : cityItems.GetLength(0) - 1 };

        for (int i = widthBounds[0]; i < widthBounds[1]; ++i)
        {
            for (int j = lengthBounds[0]; j < lengthBounds[1]; ++j)
            {
                if (cityItems[j, i].type == CityItemType.Road)
                {
                    continue;
                }
                if (i > widthBounds[0] && cityItems[j, i - 1].type == CityItemType.Building)
                {
                    if (j > lengthBounds[0] && cityItems[j - 1, i - 1].type == CityItemType.Building)
                    {
                        cityItems[j, i] = cityItems[j - 1, i].type == CityItemType.Building ? CityItem.CreateBuilding(cityItems[j - 1, i].height) : CityItem.CreateNone();
                    }
                    else
                    {
                        int lengthIt = j;
                        while (lengthIt < lengthBounds[1] && cityItems[lengthIt, i - 1].type == CityItemType.Building)
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
                else if (i > widthBounds[0] && cityItems[j, i - 1].type != CityItemType.Building)
                {
                    if (j < lengthBounds[1] - 1 && cityItems[j + 1, i - 1].type == CityItemType.Building || j > lengthBounds[0] && cityItems[j - 1, i - 1].type == CityItemType.Building)
                    {
                        cityItems[j, i] = CityItem.CreateNone();
                    }
                    else
                    {
                        int height = j > 1 && cityItems[j - 1, i].type == CityItemType.Building ? cityItems[j - 1, i].height : UnityEngine.Random.Range(1, cityData.maxNumberOfFloors + 1);
                        cityItems[j, i] = UnityEngine.Random.Range(0f, 1f) < cityData.buildingChance ? CityItem.CreateBuilding(height) : CityItem.CreateNone();
                    }
                }
                else
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

    private TileCity(Vector2Int coord, Vector2 viewedCoord, Transform parent, float sizeScale, CityData cityData, CityData cityDataLocal)
    {
        if(cityDataLocal != null) {
            this.cityDataPolicy = CityDataPolicy.Local;
            this.cityData = cityDataLocal;
        }
        else
        {
            this.cityData = cityData;
            this.cityDataPolicy = CityDataPolicy.Global;
        }

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

        nonBuildingPrefabs = Utils.ReadPrefabs(cityData.pathToNonBuildings);

        roadCrossPrefabs = Utils.ReadPrefabs(cityData.pathToRoads + "/roadcross");
        roadEndPrefabs = Utils.ReadPrefabs(cityData.pathToRoads + "/roadend");
        roadStraightPrefabs = Utils.ReadPrefabs(cityData.pathToRoads + "/roadstraight");
        roadTCrossPrefabs = Utils.ReadPrefabs(cityData.pathToRoads + "/roadtcross");
        roadTurnPrefabs = Utils.ReadPrefabs(cityData.pathToRoads + "/roadturn");

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
        RoadItem[,] roadItemsToUse = roadItems;
        int offsetI = coord.x * cityData.cityFrequency / UNITS_PER_ROAD_ITEM;
        int offsetJ = coord.y * cityData.cityFrequency / UNITS_PER_ROAD_ITEM;
        if (cityDataPolicy == CityDataPolicy.Local)
        {
            roadItemsToUse = GenerateRoadMap(cityData.cityFrequency / UNITS_PER_ROAD_ITEM, cityData.cityFrequency / UNITS_PER_ROAD_ITEM, cityData, false);
            this.roadItemsLocal = roadItemsToUse;
            offsetI = 0;
            offsetJ = 0;
        }
        for (int i = 0; i < cityData.cityFrequency / (int)UNITS_PER_ROAD_ITEM; ++i)
        {
            for (int j = 0; j < cityData.cityFrequency / (int)UNITS_PER_ROAD_ITEM; ++j)
            {
                GameObject road = InstantiateRoadItem(roadItemsToUse[j + offsetJ, i + offsetI], roadsObj.transform);
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
        CityItem[,] cityItemsToUse = cityItems;
        int offsetI = coord.x * cityData.cityFrequency;
        int offsetJ = coord.y * cityData.cityFrequency;
        if (cityDataPolicy == CityDataPolicy.Local)
        {
            cityItemsToUse = GenerateCityItemsMap(this.roadItemsLocal, cityData, false);
            offsetI = 0;
            offsetJ = 0;
        }
        for (int i = 0; i < cityData.cityFrequency; ++i)
        {
            for (int j = 0; j < cityData.cityFrequency; ++j)
            {
                if (getCityItemWithOffset(cityItemsToUse, offsetI, offsetJ, i, j).type == CityItemType.Building)
                {
                    if (j > 0 && getCityItemWithOffset(cityItemsToUse, offsetI, offsetJ, i, j - 1).type == CityItemType.Building || i > 0 && getCityItemWithOffset(cityItemsToUse, offsetI, offsetJ, i - 1, j).type == CityItemType.Building) {
                        continue;
                    }
                    int endJ = j;
                    int endI = i;
                    while(endJ + 1 < cityData.cityFrequency && getCityItemWithOffset(cityItemsToUse, offsetI, offsetJ, i, endJ + 1).type == CityItemType.Building)
                    {
                        ++endJ;
                    }
                    while (endI + 1 < cityData.cityFrequency && getCityItemWithOffset(cityItemsToUse, offsetI, offsetJ, endI + 1, j).type == CityItemType.Building)
                    {
                        ++endI;
                    }
                    GameObject buildingObj = new GameObject();
                    Building building = buildingObj.AddComponent<Building>();
                    float scale = (float)DEFAULT_CITY_FREQUENCY / cityData.cityFrequency;
                    building.Initialize(cityData.floorSizePolicy, cityData.pathsToBuildings[UnityEngine.Random.Range(0, cityData.pathsToBuildings.Length)], endI - i + 1, endJ - j + 1, getCityItemWithOffset(cityItemsToUse, offsetI, offsetJ, i, j).height, 0.75f, 2f);
                    building.Generate();
                    building.Render();
                    buildingObj.transform.parent = buildingsObj.transform;
                    buildingObj.transform.localPosition = new Vector3(scale * (2f * (endI + i) / 2f - (cityData.cityFrequency - 1)), 0, scale * (2f * (endJ + j) / 2f - (cityData.cityFrequency - 1)));
                    buildingObj.transform.localScale = Vector3.one * scale;
                }
                else if (getCityItemWithOffset(cityItemsToUse, offsetI, offsetJ, i, j).type == CityItemType.NonBuilding) {
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

    public GameObject InstantiateRoadItem(RoadItem roadItem, Transform parent)
    {
        GameObject road = null;
        int numberOfRoads = Convert.ToInt32(roadItem.IsUpRoad()) + Convert.ToInt32(roadItem.IsDownRoad()) + Convert.ToInt32(roadItem.IsRightRoad()) + Convert.ToInt32(roadItem.IsLeftRoad());
        if (numberOfRoads == 1)
        {
            GameObject asset = roadEndPrefabs[UnityEngine.Random.Range(0, roadEndPrefabs.Length)];
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
                GameObject asset = roadStraightPrefabs[UnityEngine.Random.Range(0, roadStraightPrefabs.Length)];
                road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
            }
            else if (roadItem.IsLeftRoad() && roadItem.IsRightRoad())
            {
                GameObject asset = roadStraightPrefabs[UnityEngine.Random.Range(0, roadStraightPrefabs.Length)];
                road = GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity, parent);
                road.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                GameObject asset = roadTurnPrefabs[UnityEngine.Random.Range(0, roadTurnPrefabs.Length)];
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
            GameObject asset = roadTCrossPrefabs[UnityEngine.Random.Range(0, roadTCrossPrefabs.Length)];
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
            GameObject asset = roadCrossPrefabs[UnityEngine.Random.Range(0, roadCrossPrefabs.Length)];
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
    private bool roadUp;
    private bool roadRight;
    private bool roadDown;
    private bool roadLeft;
    private bool isProcessed;

    public RoadItem(bool roadUp = false, bool roadRight = false, bool roadDown = false, bool roadLeft = false, bool isProcessed = false)
    {
        this.roadUp = roadUp;
        this.roadRight = roadRight;
        this.roadDown = roadDown;
        this.roadLeft = roadLeft;
        this.isProcessed = isProcessed;
    }

    public void Process(RoadItem up, RoadItem right, RoadItem down, RoadItem left, float newRoadChance)
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