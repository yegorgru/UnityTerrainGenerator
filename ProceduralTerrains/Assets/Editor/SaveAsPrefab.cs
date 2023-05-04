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
        Object prefab = PrefabUtility.SaveAsPrefabAsset(selectedGameObject, prefabPath);

        // Destroy the selected game object, since it is now stored as a prefab asset
        // DestroyImmediate(selectedGameObject);

        // Load the new prefab asset
        AssetDatabase.Refresh();
        Object loadedPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

        // Select the new prefab asset in the project window
        Selection.activeObject = loadedPrefab;
    }
}