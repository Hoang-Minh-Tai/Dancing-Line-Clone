using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Transform meshObject;
    private bool destroyTail;
    private ObjectPool<GameObject> segmentPool;
    private List<GameObject> activeSegments = new List<GameObject>();

    [Header("Movement")]
    public float speed = 5f;
    public float segmentSpacing = 0.5f;
    private Vector3 direction = Vector3.back;
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private float groundThickness = 0.1f;     // thin slice downward
    [SerializeField] private float edgeInset = 0.05f;          // inset from edges to avoid self-hit
    [SerializeField] private LayerMask groundLayer = 1 << 8;   // "Ground" layer
    [SerializeField] private Transform groundCheck;            // child at feet
    [SerializeField] private LayerMask wallLayer;

    [Header("Physics")]
    [SerializeField] private float gravity = -1f;

    public bool turnLeft = true;

    private Vector3 lastSegmentPosition;
    private Vector3 velocity;           // y-component only used for gravity
    public bool isGrounded;
    public bool enableMoving { get; set; }
    public bool enableInput { get; set; }
    private Rigidbody rb;


    private void Awake()
    {
        segmentPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(segmentPrefab),
            actionOnGet: obj =>
            {
                obj.SetActive(true);
                activeSegments.Add(obj);
            },
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Auto-create ground check transform if missing
        if (groundCheck == null)
        {
            var obj = new GameObject("GroundCheck");
            obj.transform.SetParent(transform, false);
            obj.transform.localPosition = Vector3.down * 0.5f;
            groundCheck = obj.transform;
        }

        Reset();
    }


    private void FixedUpdate()
    {
        if (!enableMoving) return;

        if (enableInput && InputManager.Instance.ConsumeTap())
        {
            Turn();
        }
        CheckGrounded();
        ApplyGravity();
        Move();
        if (CheckWall())
        {
            Die();
            return;
        }
        HandleSegmentSpawn();
    }


    public void Reset()
    {
        enableMoving = false;
        rb.isKinematic = true;
        rb.useGravity = false;
        destroyTail = true;
        turnLeft = false;
        direction = Vector3.back;
        meshObject.rotation = Quaternion.LookRotation(direction, Vector3.up);

        lastSegmentPosition = transform.position;
        for (int i = activeSegments.Count - 1; i >= 0; i--)
        {
            try
            {
                segmentPool.Release(activeSegments[i]);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error occurred while releasing segment: {e.Message}");
            }
        }
    }

    private void CheckGrounded()
    {
        Vector3 origin = groundCheck.position;

        // Dynamic half-extents based on cube scale
        Vector3 fullExtents = transform.localScale * 0.5f;
        Vector3 halfExtents = new Vector3(
            fullExtents.x - edgeInset,
            groundThickness,
            fullExtents.z - edgeInset
        );

        bool hitSomething = Physics.BoxCast(
            origin,
            halfExtents,
            -transform.up,
            out RaycastHit hit,
            transform.rotation,
            groundCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        // Optional: reject steep slopes
        isGrounded = hitSomething;
    }

    private bool CheckWall()
    {
        // Use player center (or slightly forward if you prefer)
        Vector3 center = transform.position;

        // Same size logic as GroundCheck
        Vector3 fullExtents = transform.localScale * 0.5f;
        Vector3 halfExtents = new Vector3(
            fullExtents.x - edgeInset,
            groundThickness,
            fullExtents.z - edgeInset
        );

        Collider[] hits = Physics.OverlapBox(
            center,
            halfExtents,
            transform.rotation,
            wallLayer, // or use a separate wallLayer if you prefer
            QueryTriggerInteraction.Ignore
        );

        return hits.Length > 0;
    }

    private void ApplyGravity()
    {
        if (isGrounded)
        {
            velocity.y = 0f;
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
    }

    private void Die()
    {
        enableMoving = false;
        StopAllCoroutines();
        destroyTail = false;
        GameEventManager.Instance.generalEvent.PlayerDied();
    }

    private void Move()
    {
        if (!enableMoving) return;
        Vector3 horizontal = direction * speed * Time.fixedDeltaTime;
        Vector3 vertical = Vector3.up * velocity.y * Time.fixedDeltaTime;

        rb.MovePosition(transform.position + horizontal + vertical);
    }

    private void HandleSegmentSpawn()
    {
        if (!isGrounded) return;

        if (Vector3.Distance(transform.position, lastSegmentPosition) >= segmentSpacing)
        {
            SpawnSegment();
            lastSegmentPosition = transform.position;
        }
    }

    private void Turn()
    {
        if (!isGrounded) return;
        SpawnSegment();

        float angle = turnLeft ? 90f : -90f;
        direction = Quaternion.Euler(0, angle, 0) * direction;

        turnLeft = !turnLeft;
    }

    private void SpawnSegment()
    {
        GameObject segment = segmentPool.Get();
        segment.transform.SetPositionAndRotation(transform.position, meshObject.rotation);
        if (destroyTail)
        {
            StartCoroutine(ReturnSegmentToPool(segment));
        }
    }

    private IEnumerator ReturnSegmentToPool(GameObject segment)
    {
        yield return new WaitForSeconds(5f);
        if (destroyTail) segmentPool.Release(segment);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
        {
            StopCoroutine(ReturnSegmentToPool(null)); // Stop all segment return coroutines
            Die();
            Debug.Log("Player collided with deadly object. Game Over.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate"))
        {
            // Turn 45 degrees
            float angle = turnLeft ? 45f : -45f;
            direction = Quaternion.Euler(0, angle, 0) * direction;
            meshObject.rotation = Quaternion.LookRotation(direction, Vector3.up);

            StopAllCoroutines();
            destroyTail = false;
            GameEventManager.Instance.generalEvent.PassGate();
            Debug.Log("Passed through gate, new direction: " + direction);
        }
    }

    private void OnDrawGizmos()
    {
        // Movement direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(direction).normalized * 2f);

        if (groundCheck == null) return;

        Vector3 origin = groundCheck.position;
        Vector3 down = -transform.up;

        // Dynamic half-extents (same as in CheckGrounded)
        Vector3 fullExtents = transform.localScale * 0.5f;
        Vector3 halfExtents = new Vector3(
            fullExtents.x - edgeInset,
            groundThickness,
            fullExtents.z - edgeInset
        );

        Gizmos.color = isGrounded ? Color.green : Color.red;

        // Starting box
        Gizmos.matrix = Matrix4x4.TRS(origin, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);

        // End box (max distance)
        Vector3 endPos = origin + down * groundCheckDistance;
        Gizmos.matrix = Matrix4x4.TRS(endPos, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);

        // Reset matrix
        Gizmos.matrix = Matrix4x4.identity;

        // Center line
        Gizmos.DrawRay(origin, down * groundCheckDistance);

        // Corner guide lines (helps see 3D shape)
        Vector3 offsetX = transform.right * halfExtents.x;
        Vector3 offsetZ = transform.forward * halfExtents.z;

        Gizmos.DrawLine(origin + offsetX + offsetZ, endPos + offsetX + offsetZ);
        Gizmos.DrawLine(origin - offsetX + offsetZ, endPos - offsetX + offsetZ);
        Gizmos.DrawLine(origin + offsetX - offsetZ, endPos + offsetX - offsetZ);
        Gizmos.DrawLine(origin - offsetX - offsetZ, endPos - offsetX - offsetZ);

        // Wall check
        Gizmos.color = Color.blue;
        Vector3 center = transform.position;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
        Gizmos.matrix = Matrix4x4.identity;
    }
}