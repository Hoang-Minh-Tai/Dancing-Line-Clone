using System.Collections;
using UnityEngine;

public class Crown : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private Sprite crownEmpty;
    [SerializeField] private Sprite crownFull;
    [SerializeField] private int crownIndex;

    private MeshRenderer meshRenderer;


    void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void OnEnable()
    {
        GameEventManager.Instance.generalEvent.onLevelReset.AddListener(Reset);
        GameEventManager.Instance.generalEvent.onStageLoad.AddListener((index) =>
        {
            if (index < crownIndex)
            {
                Reset();
            }
        });
    }

    private void Update()
    {
        meshRenderer.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
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

        meshRenderer.enabled = false;

        StartCoroutine(PlayExplosionEffectCo());

        GameEventManager.Instance.generalEvent.CollectCrown(crownIndex);
    }

    private void Reset()
    {
        meshRenderer.enabled = true;
        icon.sprite = crownEmpty;
    }


    private IEnumerator PlayExplosionEffectCo()
    {
        // Instantiate the explosion effect at tranform. Then slowly move it towards the target. Then destroy it after 1 second.
        GameObject explosionInstance = Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
        float elapsedTime = 0f;
        Vector3 startPosition = explosionInstance.transform.position;
        while (elapsedTime < 1f)
        {
            explosionInstance.transform.position = Vector3.Lerp(startPosition, icon.transform.position, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        icon.sprite = crownFull;
        Destroy(explosionInstance);
    }
}