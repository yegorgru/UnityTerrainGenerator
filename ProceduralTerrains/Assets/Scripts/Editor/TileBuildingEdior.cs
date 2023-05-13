using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Building))]
public class TileBuildingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Building building = (Building)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            building.Clear();
            building.ReadPrefabs();
            building.Generate();
            building.Render();
        }
        if (GUI.changed)
        {
            building.Clear();
            building.ReadPrefabs();
            building.Generate();
            building.Render();
        }
        if (GUILayout.Button("Clear"))
        {
            building.Clear();
        }
    }
}
