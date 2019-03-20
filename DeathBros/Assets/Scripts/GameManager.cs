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
    public static Vector2 PlayerLevelPosition { get { return Player.transform.position - CurrentLevel.transform.position; } }

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

        //PauseMenu.Instance.gameObject.SetActive(true);
        PauseMenu.Open();
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

    public static bool CheckIfDamageApplies(EAttackType attackType, EAttackClass eAttackClass)
    {
        switch (eAttackClass)
        {

            case EAttackClass.Tilts:
                {
                    if (attackType == EAttackType.DTilt || attackType == EAttackType.UTilt || attackType == EAttackType.FTilt ||
                        attackType == EAttackType.Jab1 || attackType == EAttackType.DashAtk)
                        return true;
                    break;
                }

            case EAttackClass.Aerials:
                {
                    if (attackType == EAttackType.NAir || attackType == EAttackType.DAir || attackType == EAttackType.UAir || 
                        attackType == EAttackType.FAir || attackType == EAttackType.BAir)
                        return true;
                    break;
                }

            case EAttackClass.Strongs:
                {
                    if (attackType == EAttackType.USoul || attackType == EAttackType.DSoul || attackType == EAttackType.FSoul)
                        return true;
                    break;
                }

            case EAttackClass.Specials:
                {
                    if (attackType == EAttackType.NSpec || attackType == EAttackType.FSpec || attackType == EAttackType.DSpec || attackType == EAttackType.USpec)
                        return true;
                    break;
                }
            default: break;
        }

        return false;
    }
}
