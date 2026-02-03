using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;  // ← add this

public static class SortChildrenByName
{
    [MenuItem("GameObject/Sort Children By Name (Natural)", false, 10)]
    private static void SortSelectedNatural()
    {
        if (Selection.activeTransform == null)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        Transform parent = Selection.activeTransform;
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < parent.childCount; i++)
        {
            children.Add(parent.GetChild(i));
        }

        // Natural sort: pad numbers with leading zeros so "11" > "2"
        children = children
            .OrderBy(t => Regex.Replace(t.name, @"\d+", m => m.Value.PadLeft(20, '0')))
            .ToList();

        // Re-apply order (same as before)
        Undo.SetCurrentGroupName("Sort Children By Name (Natural)");
        int group = Undo.GetCurrentGroup();

        foreach (Transform child in children)
        {
            Undo.SetTransformParent(child, null, "Detach for sort");
        }

        for (int i = 0; i < children.Count; i++)
        {
            Undo.SetTransformParent(children[i], parent, "Re-attach sorted");
            children[i].SetSiblingIndex(i);
        }

        Undo.CollapseUndoOperations(group);
        Debug.Log($"Natural-sorted {children.Count} children of {parent.name}.");
    }

    [MenuItem("GameObject/Sort Children By Name (Natural)", true)]
    private static bool ValidateSortNatural()
    {
        return Selection.activeTransform != null;
    }
}