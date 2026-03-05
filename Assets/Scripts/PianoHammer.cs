using UnityEngine;

public class PianoHammer : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Strike()
    {
        if (animator == null) return;
        animator.SetTrigger("Strike");
    }
}
