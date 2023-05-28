using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Building building = (Building)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            building.Clear();
            building.Generate();
            building.Render();
        }
        if (GUI.changed)
        {
            building.Clear();
            building.Generate();
            building.Render();
        }
        if (GUILayout.Button("Clear"))
        {
            building.Clear();
        }
    }
}
