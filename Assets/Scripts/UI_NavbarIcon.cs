using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_NavbarIcon : MonoBehaviour
{
    private Image childImage;
    public float riseDistance = 50f;
    public float animationDuration = 0.5f;
    public Color targetColor = Color.orange;

    public delegate void OnIconClick();
    public event OnIconClick IconClicked;

    public Button button;

    void Start()
    {
        button = GetComponentInChildren<Button>();
        // Get the Image component in the children
        childImage = GetComponentInChildren<Image>();
        if (childImage == null)
        {
            Debug.LogError("No Image component found in children of UI_NavbarIcon.");
        }

        button.onClick.AddListener(() =>
        {
            StartCoroutine(AnimateIcon());
            IconClicked?.Invoke();
            Debug.Log("Button clicked!");
        });
    }

    void OnMouseDown()
    {
        if (childImage != null)
        {
            StartCoroutine(AnimateIcon());
            IconClicked?.Invoke();
        }
    }

    private IEnumerator AnimateIcon()
    {
        Vector3 originalPosition = childImage.rectTransform.localPosition;
        Vector3 targetPosition = originalPosition + Vector3.up * riseDistance;

        Color originalColor = childImage.color;

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            childImage.rectTransform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
            childImage.color = Color.Lerp(originalColor, targetColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are set
        childImage.rectTransform.localPosition = targetPosition;
        childImage.color = targetColor;
    }
}
