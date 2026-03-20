using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Firework : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        GameEventManager.Instance.generalEvent.onGatePass.AddListener(Explode);
        GameEventManager.Instance.generalEvent.onLevelReset.AddListener(Reset);
    }

    void Start()
    {
        Reset();
    }

    public void Explode()
    {
        if (ps == null) return;
        ps.Play();
    }

    void Reset()
    {
        if (ps == null) return;
        ps.Stop();
    }
}