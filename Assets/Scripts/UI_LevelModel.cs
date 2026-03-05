using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelModel : MonoBehaviour, IMouseClickHandler
{
    Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float speed = 8f;

    bool hovering = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        Vector3 target = hovering ? originalScale * hoverScale : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * speed);
    }

    public void OnMouseDown()
    {
        Debug.Log($"Clicked level: {gameObject.name}");
        SceneManager.LoadScene("Piano Level");
    }

    public void OnMouseEnter()
    {
        hovering = true;
    }

    public void OnMouseExit()
    {
        hovering = false;
    }
}
