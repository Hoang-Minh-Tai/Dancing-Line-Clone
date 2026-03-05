using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_FallingSquare : MonoBehaviour
{
    [Header("Size")]
    [SerializeField] float minSize = 24f;
    [SerializeField] float maxSize = 120f;

    [Header("Movement")]
    [SerializeField] Vector2 fallSpeedRange = new Vector2(40f, 180f);
    [SerializeField] Vector2 rotationSpeedRange = new Vector2(-90f, 90f);

    [Header("Fade / Respawn")]
    [SerializeField] float fadeDuration = 0.6f;
    [SerializeField] float respawnDelay = 0.05f;

    [Header("Spawn/Despawn Control")]
    [SerializeField] float spawnLineY = 500f;
    [SerializeField] float despawnLineY = -500f;

    RectTransform _rect;
    RectTransform _parentRect;
    Image _image;

    float _fallSpeed;
    float _rotationSpeed;
    float _rotation;
    Coroutine _fadeCoroutine;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _image = GetComponent<Image>() ?? GetComponentInChildren<Image>();

        Canvas c = GetComponentInParent<Canvas>();
        if (c != null) _parentRect = c.GetComponent<RectTransform>();
        if (_parentRect == null && transform.parent != null)
            _parentRect = transform.parent as RectTransform;
    }

    void Start()
    {
        Respawn();

        // Randomize initial Y position to simulate falling
        if (_rect != null && _parentRect != null)
        {
            float parentH = _parentRect.rect.height;
            float randomYOffset = Random.Range(-parentH * 0.5f, parentH * 0.5f);
            _rect.anchoredPosition += new Vector2(0, randomYOffset);
        }
    }

    void Update()
    {
        if (_parentRect == null) return;

        // Move downward
        _rect.anchoredPosition += Vector2.down * _fallSpeed * Time.deltaTime;

        // Rotate
        _rotation += _rotationSpeed * Time.deltaTime;
        _rect.localEulerAngles = new Vector3(0f, 0f, _rotation);

        // When reaches despawn line, start fade+respawn
        float halfHeight = _rect.rect.height * 0.5f;
        if (_rect.anchoredPosition.y <= despawnLineY - halfHeight && _fadeCoroutine == null)
        {
            _fadeCoroutine = StartCoroutine(FadeAndRespawn());
        }
    }

    IEnumerator FadeAndRespawn()
    {
        if (_image == null)
        {
            Respawn();
            _fadeCoroutine = null;
            yield break;
        }

        float t = 0f;
        Color col = _image.color;
        float startA = col.a;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            col.a = Mathf.Lerp(startA, 0f, t / fadeDuration);
            _image.color = col;
            yield return null;
        }

        col.a = 0f;
        _image.color = col;

        yield return new WaitForSeconds(respawnDelay);

        Respawn();
        _fadeCoroutine = null;
    }

    void Respawn()
    {
        if (_rect == null) return;
        if (_parentRect == null)
        {
            Canvas c = GetComponentInParent<Canvas>();
            if (c != null) _parentRect = c.GetComponent<RectTransform>();
            if (_parentRect == null && transform.parent != null)
                _parentRect = transform.parent as RectTransform;
            if (_parentRect == null) return;
        }

        // Ensure center anchors/pivot for predictable positioning
        _rect.anchorMin = _rect.anchorMax = new Vector2(0.5f, 0.5f);
        _rect.pivot = new Vector2(0.5f, 0.5f);

        // Random size
        float size = Random.Range(minSize, maxSize);
        _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

        // Random rotation and rotation speed
        _rotation = Random.Range(0f, 360f);
        _rect.localEulerAngles = new Vector3(0f, 0f, _rotation);
        _rotationSpeed = Random.Range(rotationSpeedRange.x, rotationSpeedRange.y);

        // Random fall speed
        _fallSpeed = Random.Range(fallSpeedRange.x, fallSpeedRange.y);

        // Place at spawn line with random X
        float parentW = _parentRect.rect.width;
        float halfSize = size * 0.5f;
        float x = Random.Range(-parentW * 0.5f + halfSize, parentW * 0.5f - halfSize);
        _rect.anchoredPosition = new Vector2(x, spawnLineY);

        // Reset alpha
        if (_image != null)
        {
            Color c = _image.color;
            c.a = 0.5f;
            _image.color = c;
        }
    }

    void OnDrawGizmos()
    {
        if (_parentRect == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-_parentRect.rect.width * 0.5f, spawnLineY, 0),
                        new Vector3(_parentRect.rect.width * 0.5f, spawnLineY, 0));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-_parentRect.rect.width * 0.5f, despawnLineY, 0),
                        new Vector3(_parentRect.rect.width * 0.5f, despawnLineY, 0));
    }
}
