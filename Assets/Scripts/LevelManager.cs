using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class StageData
{
    public Vector3 playerStartPosition;
    public bool playerTurnLeft;
    public float directorStartTime;
}

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private StageData[] stages;
    [Range(0, 2)]
    [SerializeField] private int initialStageIndex = 0;

    private Player player;
    private InputManager input;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Start()
    {
        input = InputManager.Instance;
        LoadStage(initialStageIndex);
    }

    void Update()
    {
        // director.time = 30;
    }

    public void LoadStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= 3) return;

        StopCoroutine(StartPlayCo());
        director.Stop();
        player.enableMoving = false;

        StageData stage = stages[stageIndex];
        player.transform.position = stage.playerStartPosition;
        player.gameObject.name = $"Player (Stage {stageIndex})";
        player.turnLeft = stage.playerTurnLeft;
        director.time = stage.directorStartTime;

        StartCoroutine(StartPlayCo());
        Debug.Log($"Loaded Stage {stageIndex}");
        Debug.Log("Waiting for tap to start...");
    }

    private IEnumerator StartPlayCo()
    {

        yield return new WaitUntil(() => input.ConsumeTap());
        yield return new WaitForEndOfFrame();

        player.enableMoving = true;
        director.Play();
    }

    //---------------------------------------------
    // Test
    //---------------------------------------------
    [ContextMenu("Test Load Stage 0")]
    private void TestLoadStage0()
    {
        LoadStage(0);
    }

    [ContextMenu("Test Load Stage 1")]
    private void TestLoadStage1()
    {
        LoadStage(1);
    }

    [ContextMenu("Test Load Stage 2")]
    private void TestLoadStage2()
    {
        LoadStage(2);
    }
}
