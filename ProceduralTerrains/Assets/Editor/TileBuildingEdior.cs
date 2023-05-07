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
            // mapGen.Clear();
        }
        if (GUILayout.Button("Generate"))
        {
            building.Generate();
            building.Render();
        }
    }
}
