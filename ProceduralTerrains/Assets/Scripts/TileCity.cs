using System;
using UnityEngine;

public class TileCity : Tile
{
    public Vector2 position;
    Building building;

    public TileCity(Vector2 coord, int size, Transform parent, float sizeScale)
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
}