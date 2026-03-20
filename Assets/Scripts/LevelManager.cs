using System;
using System.Collections;
using Unity.Cinemachine;
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
    [SerializeField] private PauseMenuManager pauseMenu;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private CinemachineCamera followCam;
    [SerializeField] private StageData[] stages;
    [Range(0, 2)]
    [SerializeField] private int initialStageIndex = 0;
    private int currentStageIndex = 0;

    private int score = 0;
    private float progress = 0f;
    [SerializeField] private float directorTotalDuration = 75.29578f;

    private PlayerMovement player;
    private InputManager inputManager;

    private bool canOpenMenu = true;
    private bool isMenuOpen = false;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        GameEventManager.Instance.generalEvent.onGatePass.AddListener(PassGate);
        GameEventManager.Instance.generalEvent.onPlayerDead.AddListener(PlayerDie);
        GameEventManager.Instance.generalEvent.onDiamondCollected.AddListener(() => score++);
        GameEventManager.Instance.generalEvent.onLevelReset.AddListener(ResetLevel);
        GameEventManager.Instance.generalEvent.onCrownCollected.AddListener(SetCurrentStageIndex);
        GameEventManager.Instance.generalEvent.onLevelContinue.AddListener(ContiueLevel);
    }

    void Start()
    {
        inputManager = InputManager.Instance;
        currentStageIndex = initialStageIndex;
        LoadStage(currentStageIndex);

    }

    void Update()
    {
        // director.time = 30;
        if (canOpenMenu && inputManager.input.UI.Escape.WasPerformedThisFrame())
        {
            Debug.Log("Escape key pressed. Toggling pause menu...");
            if (isMenuOpen)
            {
                pauseMenu.ClosePauseMenu();
                isMenuOpen = false;
            }
            else
            {
                pauseMenu.OpenPauseMenu(0f);
                UpdateUIStat();
                isMenuOpen = true;
            }
        }
    }

    private void ContiueLevel()
    {
        if (progress >= 100)
        {
            LoadStage(0);
            return;
        }
        LoadStage(currentStageIndex);
    }

    private void ResetLevel()
    {
        LoadStage(0);
    }

    private void SetCurrentStageIndex(int index)
    {
        currentStageIndex = index;
    }

    public void LoadStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= 3) return;

        StopAllCoroutines();
        director.Stop();
        player.enableMoving = false;
        score = 0;

        StageData stage = stages[stageIndex];
        player.transform.position = stage.playerStartPosition;

        followCam.transform.position = player.transform.position; // Snap camera to player position
        followCam.PreviousStateIsValid = false; // Force Cinemachine to update camera position immediately

        player.gameObject.name = $"Player (Stage {stageIndex})";
        player.Reset();

        director.time = stage.directorStartTime;

        progress = (int)(director.time / directorTotalDuration * 100);


        director.Evaluate();
        isMenuOpen = false;
        StartCoroutine(StartPlayCo());
        GameEventManager.Instance.generalEvent.LoadStage(stageIndex);
        Debug.Log($"Loaded Stage {stageIndex}");
        Debug.Log("Waiting for tap to start...");
    }

    private IEnumerator StartPlayCo()
    {
        canOpenMenu = true;
        yield return new WaitUntil(() => !isMenuOpen && inputManager.ConsumeTap());
        GameEventManager.Instance.generalEvent.PlayerStart();
        yield return new WaitForEndOfFrame();
        canOpenMenu = false;
        player.enableMoving = true;
        player.enableInput = true;
        director.Play();
    }

    private void PassGate()
    {
        Debug.Log("Win!");

        progress = 100;
        player.enableInput = false;
        pauseMenu.OpenPauseMenu(5f);
        UpdateUIStat();
        SaveProgress();
        StartCoroutine(StopPlayerCo());
    }

    private IEnumerator StopPlayerCo()
    {
        yield return new WaitForSeconds(10f);
        player.enableMoving = false;
    }

    private void PlayerDie()
    {
        progress = (int)(director.time / directorTotalDuration * 100);
        progress = Mathf.Clamp(progress, 0, 99); // Cap progress at 99% on death
        SaveProgress();
        pauseMenu.OpenPauseMenu(1f);
        UpdateUIStat();
    }

    private void UpdateUIStat()
    {
        pauseMenu.UpdateProgress(progress, score);
    }

    private void SaveProgress()
    {
        int bestScore = PlayerPrefs.GetInt("Score", 0);
        float bestProgress = PlayerPrefs.GetFloat("Progress", 0f);

        if (score > bestScore)
        {
            PlayerPrefs.SetInt("Score", score);
            Debug.Log($"New best score: {score}");
        }
        if (progress > bestProgress)
        {
            PlayerPrefs.SetFloat("Progress", progress);
            Debug.Log($"New best progress: {progress}%");
        }
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
