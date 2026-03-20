using UnityEngine;

public class UI_Guide : MonoBehaviour
{
    private InputManager inputManager;

    void OnEnable()
    {
        GameEventManager.Instance.generalEvent.onPlayerStart.AddListener(Hide);
    }

    void Start()
    {

        inputManager = InputManager.Instance;
    }

    void Update()
    {
        if (inputManager.input.UI.Escape.WasPerformedThisFrame())
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        Destroy(gameObject);
        // gameObject.SetActive(false);
    }
}
