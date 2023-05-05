using UnityEngine;
using UnityEditor;

public class SaveAsPrefab : MonoBehaviour
{
    [MenuItem("GameObject/Save as Prefab")]
    static void SaveGameObjectAsPrefab()
    {
        GameObject selectedGameObject = Selection.activeGameObject;

        // Create a new empty prefab asset
        string prefabPath = "Assets/NewPrefab.prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(selectedGameObject, prefabPath, InteractionMode.UserAction);

        // Load the new prefab asset
        AssetDatabase.Refresh();
        Object loadedPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

        // Select the new prefab asset in the project window
        Selection.activeObject = loadedPrefab;
    }
}