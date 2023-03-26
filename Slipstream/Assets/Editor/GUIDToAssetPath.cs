using UnityEditor;
using UnityEngine;

public class GUIDToAssetPathExample : MonoBehaviour
{
    [MenuItem("Tools/GUIDToAssetPath")]
    static void MaterialPathsInProject()
    {
        var allMaterials = AssetDatabase.FindAssets("t: Material");

        foreach (var guid in allMaterials)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(path + " : " + guid);
        }
    }
}