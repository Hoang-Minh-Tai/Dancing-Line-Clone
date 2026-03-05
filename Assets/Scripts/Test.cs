using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        Debug.Log("Running update");
    }

    void FixedUpdate()
    {
        Debug.Log("Running fixed update");
    }
}