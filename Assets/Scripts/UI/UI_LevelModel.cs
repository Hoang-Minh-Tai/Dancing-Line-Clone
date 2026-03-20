using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelModel : MonoBehaviour, IMouseClickHandler
{
    Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float speed = 8f;

    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI progressText;

    bool hovering = false;

    void Start()
    {
        originalScale = transform.localScale;

        int bestScore = PlayerPrefs.GetInt("Score", 0);
        float bestProgress = PlayerPrefs.GetFloat("Progress", 0f);

        scoreText.text = $"{bestScore}/10";
        progressText.text = $"{bestProgress}%";
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

    [ContextMenu("Clear History")]
    public void ClearHistory()
    {
        PlayerPrefs.DeleteAll();
        scoreText.text = $"0/10";
        progressText.text = $"0%";
    }
}
