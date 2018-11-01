using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : _MB
{
    public Level startingLevel;
    public World startWorld;

    public float knTestFactor = 1;
    public static bool IsPaused { get; private set; }

    public static Player Player { get; private set; }
    public StateMachine LevelSM { get; private set; }
    public static Level CurrentLevel { get { return (Level)Instance.LevelSM.CurrentState; } }
    public static Vector2 PlayerLevelPosition {  get { return Player.transform.position - CurrentLevel.transform.position; } }

    public static GameManager Instance { get; private set; }

    private bool levelStarted = false;

    public override void Init()
    {
        base.Init();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }

        Player = FindObjectOfType<Player>();

        if (!Player)
            Debug.Log("Player not found");

        LevelSM = new StateMachine();

    }

    void Update()
    {
        if (levelStarted == false)
        {
            LevelSM.ChangeState(startingLevel);
            levelStarted = true;
        }

        LevelSM.Update();

        if (Input.GetKeyDown(KeyCode.Escape) || InputManager.Pause.GetButtonDown())
        {

            if (!IsPaused)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        IsPaused = true;

        PauseMenu.Instance.gameObject.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        IsPaused = false;

        PauseMenu.Instance.gameObject.SetActive(false);
    }

    public static void ChangeLevel(Level newLevel)
    {
        if (newLevel != null)
            Instance.LevelSM.ChangeState(newLevel);
    }
}
