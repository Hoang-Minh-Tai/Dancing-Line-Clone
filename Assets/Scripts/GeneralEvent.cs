using UnityEngine.Events;

public class GeneralEvent
{
    public UnityEvent onLevelReset = new UnityEvent();
    public UnityEvent onLevelContinue = new UnityEvent();
    public UnityEvent onDiamondCollected = new UnityEvent();
    public UnityEvent<int> onCrownCollected = new UnityEvent<int>();
    public UnityEvent onPlayerDead = new UnityEvent();
    public UnityEvent onGatePass = new UnityEvent();
    public UnityEvent onPlayerStart = new UnityEvent();
    public UnityEvent<int> onStageLoad = new UnityEvent<int>();

    public void ResetLevel()
    {
        onLevelReset.Invoke();
    }

    public void ContinueLevel()
    {
        onLevelContinue.Invoke();
    }

    public void CollectDiamond()
    {
        onDiamondCollected.Invoke();
    }

    public void CollectCrown(int index)
    {
        onCrownCollected.Invoke(index);
    }

    public void LoadStage(int index)
    {
        onStageLoad.Invoke(index);
    }

    public void PlayerDied()
    {
        onPlayerDead.Invoke();
    }

    public void PassGate()
    {
        onGatePass.Invoke();
    }

    public void PlayerStart()
    {
        onPlayerStart.Invoke();
    }
}