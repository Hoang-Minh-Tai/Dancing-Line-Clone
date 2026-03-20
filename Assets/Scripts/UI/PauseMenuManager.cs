using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private float openDelay = 1f;

    [SerializeField] private TMPro.TextMeshProUGUI progressText;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    [SerializeField] private Button[] buttons;
    private int selectedButtonIndex = 0;

    void OnEnable()
    {

    }

    public void ResetLevel()
    {
        GameEventManager.Instance.generalEvent.ResetLevel();
        pauseMenuUI.SetActive(false);
    }

    public void ContinueLevel()
    {
        GameEventManager.Instance.generalEvent.ContinueLevel();
        pauseMenuUI.SetActive(false);
    }



    public void QuitLevel()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ClosePauseMenu()
    {
        pauseMenuUI.SetActive(false);
    }

    public void OpenPauseMenu(float delay)
    {
        Debug.Log("Opening pause menu...");
        StartCoroutine(OpenUICoroutine(delay));
    }

    private IEnumerator OpenUICoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        pauseMenuUI.SetActive(true);
        buttons[selectedButtonIndex].Select();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void UpdateProgress(float progress, int score)
    {
        progressText.text = $"<size=120%>{progress}</size>%";
        scoreText.text = $"{score}/10";
    }
}
