using UnityEditor;
using UnityEngine;

public class PivotToEmptyParent : EditorWindow
{
    [MenuItem("Tools/Pivot → Empty Parent (Zero Local Pos)")]
    private static void ShowWindow()
    {
        GetWindow<PivotToEmptyParent>("Pivot via Parent");
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "Creates a new empty parent at the selected object's position,\n" +
            "parents the object under it, then resets localPosition to zero.\n" +
            "Visual position stays the same — pivot moves to parent.",
            MessageType.Info);

        if (GUILayout.Button("Apply to Selected"))
        {
            if (Selection.gameObjects.Length == 0)
            {
                Debug.LogWarning("Nothing selected.");
                return;
            }

            Undo.SetCurrentGroupName("Pivot via Empty Parent");

            int count = 0;

            foreach (var go in Selection.gameObjects)
            {
                Transform originalParent = go.transform.parent;

                // Create empty parent
                GameObject pivotParent = new GameObject(go.name + "_Pivot");
                Undo.RegisterCreatedObjectUndo(pivotParent, "Create Pivot Parent");

                Transform pivotTransform = pivotParent.transform;

                // Match world transform
                pivotTransform.position = go.transform.position;
                pivotTransform.rotation = go.transform.rotation;
                pivotTransform.localScale = go.transform.lossyScale;

                // Parent it correctly in hierarchy
                pivotTransform.SetParent(originalParent, true);

                // Reparent selected object
                Undo.SetTransformParent(go.transform, pivotTransform, "Reparent Object");

                // Zero local transform
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                // Rename child to "Body"
                Undo.RecordObject(go, "Rename Body");
                go.name = "Body";

                count++;
            }

            Debug.Log($"Created pivot parents for {count} object{(count > 1 ? "s" : "")}");
        }
    }
}
