using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : _MB
{
    public Level startingLevel;

    public static bool IsPaused { get; private set; }

    public static Player Player { get; private set; }
    public StateMachine LevelSM { get; private set; }

    public static GameManager Instance { get; private set; }

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
        LevelSM.ChangeState(startingLevel);
    }

    void Update()
    {
        LevelSM.Update();

        if(Input.GetKeyDown(KeyCode.Escape))
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
    }

    public void Resume()
    {
        Time.timeScale = 1;
        IsPaused = false;
    }
}
