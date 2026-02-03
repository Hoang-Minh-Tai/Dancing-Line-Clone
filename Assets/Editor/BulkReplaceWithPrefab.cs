using UnityEditor;
using UnityEngine;

public class BulkReplaceWithPrefab : EditorWindow
{
    private GameObject prefab;

    [MenuItem("Tools/Bulk Replace With Prefab")]
    private static void ShowWindow()
    {
        GetWindow<BulkReplaceWithPrefab>("Bulk Prefab Replace");
    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Target Prefab", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace Selected Objects") && prefab != null)
        {
            Undo.SetCurrentGroupName("Bulk Replace With Prefab");
            int group = Undo.GetCurrentGroup();

            var selected = Selection.gameObjects;
            foreach (var go in selected)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(go)) continue; // skip already instances

                var newInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, go.transform.parent);
                newInstance.transform.SetPositionAndRotation(go.transform.position, go.transform.rotation);
                newInstance.transform.localScale = go.transform.localScale;

                Undo.RegisterCreatedObjectUndo(newInstance, "");
                Undo.DestroyObjectImmediate(go);
            }

            Undo.CollapseUndoOperations(group);
            Debug.Log($"Replaced {selected.Length} objects with {prefab.name}");
        }
    }
}