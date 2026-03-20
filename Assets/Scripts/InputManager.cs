using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public InputSystem_Actions input;

    [SerializeField] private float tapBuffer = 0.2f;
    private float bufferTimer = 0f;
    private Camera mainCamera;

    // Hover detection variables
    private GameObject currentHovered = null;
    [SerializeField] private LayerMask hoverLayerMask;
    [SerializeField] private float hoverCheckInterval = 0.05f;
    private float hoverTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        input = new InputSystem_Actions();
        input.Player.Enable();
        input.UI.Enable();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (input.Player.Tap.WasPerformedThisFrame())
        {
            bufferTimer = tapBuffer;
        }

        bufferTimer -= Time.deltaTime;

        if (Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                // ─── Hit something ───────────────────────────────
                GameObject clicked = hit.collider.gameObject;

                if (clicked.TryGetComponent<IMouseClickHandler>(out var obj))
                {
                    obj.OnMouseDown();
                }
            }
        }

        // ─── Hover detection ───────────────────────────────────────
        hoverTimer -= Time.deltaTime;
        if (hoverTimer <= 0f)
        {
            hoverTimer = hoverCheckInterval;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            GameObject newHovered = null;

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hoverLayerMask))
            {
                newHovered = hit.collider.gameObject;
            }

            // Has hover changed?
            if (newHovered != currentHovered)
            {
                // Exit previous
                if (currentHovered != null &&
                    currentHovered.TryGetComponent<IMouseClickHandler>(out var prev))
                {
                    prev.OnMouseExit();
                }

                // Enter new
                if (newHovered != null &&
                    newHovered.TryGetComponent<IMouseClickHandler>(out var next))
                {
                    next.OnMouseEnter();
                }

                currentHovered = newHovered;
            }
        }
    }

    public bool ConsumeTap()
    {
        if (bufferTimer > 0f)
        {
            bufferTimer = 0f;
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        input.Player.Disable();
    }
}