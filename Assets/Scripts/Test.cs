#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class Test
{
    [MenuItem("Tools/Replace With Prefab (Keep World Transform)")]
    static void Replace()
    {
        GameObject prefab = null;

        foreach (var obj in Selection.objects)
        {
            if (obj is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go))
            {
                prefab = go;
                break;
            }
        }

        if (prefab == null)
        {
            Debug.LogError("Select a prefab asset (Project) and one or more scene objects.");
            return;
        }

        foreach (var go in Selection.gameObjects)
        {
            Transform t = go.transform;

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, go.scene);
            instance.transform.SetParent(t.parent, true); // keep world
            instance.transform.SetPositionAndRotation(t.position, t.rotation);
            instance.transform.localScale = t.lossyScale;

            Undo.DestroyObjectImmediate(go);
        }
    }
}
#endif
