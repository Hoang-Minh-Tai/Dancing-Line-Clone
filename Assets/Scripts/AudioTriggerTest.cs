using UnityEngine;
using UnityEngine.Playables;

public class AudioTriggerTest : MonoBehaviour
{
    public AudioSource audioSource;
    public PlayableDirector playableDirector;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        // audioSource.Play();
        playableDirector.Play();
    }
}
