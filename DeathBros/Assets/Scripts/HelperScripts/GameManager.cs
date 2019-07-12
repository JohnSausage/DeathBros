using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : _MB
{
    public bool startGameHere = false;
    public bool gameHasStarted = false;
    [Space]
    public Level startingLevel;
    public World startWorld;

    public float knTestFactor = 1;
    public static bool IsPaused { get; private set; }

    public static Player Player { get; private set; }
    public static CameraController MainCamera { get; private set; }
    public StateMachine LevelSM { get; private set; }
    public static Level CurrentLevel { get { return (Level)Instance.LevelSM.CurrentState; } }
    public static Vector2 PlayerLevelPosition { get { return Player.transform.position - CurrentLevel.transform.position; } }

    public AudioManager audioManager { get; protected set; }
    public DialogueManager dialogueManager { get; protected set; }
    public Canvas canvas;

    public SaveData saveData;// { get; protected set; }



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
            Destroy(gameObject);
        }

        //Player = FindObjectOfType<Player>();
        //MainCamera = FindObjectOfType<CameraController>();

        //if (!Player)
        //    Debug.Log("Player not found");

        //LevelSM = new StateMachine();

        //StartGame();

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    //protected override void Start()
    //{
    //    base.Start();

    //    if (startGameHere == true)
    //    {
    //        StartGame();
    //    }
    //}

    void Update()
    {
        if (gameHasStarted == false)
        {
            return;
        }

        LevelSM.Update();

        if (Input.GetKeyDown(KeyCode.Escape) || InputManager.Pause.GetButtonDown())
        {

            if (!IsPaused)
                Pause();
            else
                Resume();
        }

        //@@@ For Debugging
        if (Input.GetKeyDown(KeyCode.P))
        {
            Player.Respawn();
        }

    }

    public void Pause()
    {
        Time.timeScale = 0;
        IsPaused = true;

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

    public static void StartGame()
    {
        Instance.saveData.gameStarted = true;

        Instance.InitLevel();

        Instance.UpdateManagers();

        Instance.LevelSM.ChangeState(Instance.startingLevel);
    }

    protected void UpdateManagers()
    {
        audioManager = FindObjectOfType<AudioManager>();

        //AudioManager.Instance.FindAndLoadSounds();

        dialogueManager = FindObjectOfType<DialogueManager>();
        dialogueManager.Setup();
    }


    protected void InitLevel()
    {
        gameHasStarted = true;


        Player = FindObjectOfType<Player>();
        MainCamera = FindObjectOfType<CameraController>();

        CameraController.Instance.target = Player.transform;

        if (!Player)
            Debug.Log("Player not found");

        LevelSM = new StateMachine();

        if (startingLevel == null)
        {
            World world = FindObjectOfType<World>();

            startingLevel = world.startingLevel;
        }
    }

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    protected void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name != "NES_StartMenu")
        {
            StartGame();
            Resume();

            canvas = FindObjectOfType<Canvas>();
            canvas.worldCamera = CameraController.Instance.GetComponent<Camera>();
        }
    }

    public void ExitToMainMenu()
    {
        LevelSM.ChangeState(new EmptyState());

        gameHasStarted = false;

        LoadScene("NES_StartMenu");
    }
}
