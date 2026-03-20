using UnityEngine;

public class Diamond : MonoBehaviour
{
    [SerializeField] private GameObject fracturePrefab;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float explosionForce = 300f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float upwardModifier = 0.5f;

    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void OnEnable()
    {
        GameEventManager.Instance.generalEvent.onStageLoad.AddListener(Reset);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Break();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && meshRenderer.enabled)
        {
            Break();
        }
    }

    private void Break()
    {
        GameObject fractureInstance = Instantiate(fracturePrefab, transform.position, transform.rotation);

        Rigidbody[] bodies = fractureInstance.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in bodies)
        {
            rb.AddExplosionForce(
                explosionForce,
                transform.position,
                explosionRadius,
                upwardModifier,
                ForceMode.Impulse
            );
        }

        Destroy(fractureInstance, 3f);

        GameObject explosionEffectInstance = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        Destroy(explosionEffectInstance, 2f);

        meshRenderer.enabled = false;

        GameEventManager.Instance.generalEvent.CollectDiamond();
    }

    private void Reset(int stageIndex)
    {
        meshRenderer.enabled = true;
    }
}