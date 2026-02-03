using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Road : MonoBehaviour
{

    void OnApplicationQuit()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }
}
