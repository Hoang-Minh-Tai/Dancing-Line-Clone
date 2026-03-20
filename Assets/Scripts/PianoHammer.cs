using UnityEngine;

public class PianoHammer : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        GameEventManager.Instance.generalEvent.onStageLoad.AddListener(Reset);
    }

    public void Strike()
    {
        if (animator == null) return;
        animator.SetTrigger("Strike");
    }

    void Reset(int index)
    {
        if (index < 2)
        {
            animator.Play("New State", 1, 0f);
        }
    }
}
