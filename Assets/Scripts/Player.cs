using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private bool destroyTail;

    public float speed = 5f;
    public float segmentSpacing = 0.5f;
    public Vector3 direction = Vector3.forward;

    private InputSystem_Actions input;
    public bool turnLeft = true; // Track the turn direction
    private Vector3 lastSegmentPosition;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
    }

    private void Start()
    {
        lastSegmentPosition = transform.position;
        input.Player.Tap.performed += ctx => Turn();
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, lastSegmentPosition) >= segmentSpacing)
        {
            SpawnSegment();
            lastSegmentPosition = transform.position;
        }
    }

    void Turn()
    {
        SpawnSegment(); // Spawn a segment at the turn point
        if (turnLeft)
        {
            direction = Quaternion.Euler(0, 90, 0) * direction;
        }
        else
        {
            direction = Quaternion.Euler(0, -90, 0) * direction;
        }

        turnLeft = !turnLeft; // Toggle the turn direction
    }

    void SpawnSegment()
    {
        GameObject segment = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
        if (destroyTail)
        {
            Destroy(segment, 15f); // Destroy the segment after 5 seconds
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a ray to visualize the direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, direction * 2);
    }
}
