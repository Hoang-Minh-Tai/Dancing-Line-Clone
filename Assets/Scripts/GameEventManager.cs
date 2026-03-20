using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    public GeneralEvent generalEvent;

    private void Awake()
    {
        Instance = this;

        generalEvent = new GeneralEvent();
        Debug.Log("GameEventManager Awake: Instance set.");


    }
}
