using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileBuilding))]
public class TileBuildingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TileBuilding building = (TileBuilding)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Clear"))
        {
            while (building.transform.childCount > 0)
            {
                Transform child = building.transform.GetChild(0);
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
        if (GUILayout.Button("Generate"))
        {
            building.ReadPrefabs();
            building.Generate();
            building.Render();
        }
    }
}
