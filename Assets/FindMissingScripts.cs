using System.Linq;
using UnityEngine;
using UnityEditor;
public class FindMissingScripts : Editor
{
    [MenuItem("Component/FindMissingScripts")]
    public static void FindGameObjects()
    {
        string[] prefabPaths = AssetDatabase.GetAllAssetPaths()
            .Where(path => path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();

        GameObject parent = null;
        foreach (var path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Component[] components = prefab.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null)
                {
                    parent = new GameObject("Missing Component Objects");
                }

                GameObject instance = Instantiate(prefab, parent.transform);
                break;
            }
        }
    }
}