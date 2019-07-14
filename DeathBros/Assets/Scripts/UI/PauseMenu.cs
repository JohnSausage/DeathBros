using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; protected set; }
    public StateMachine MenuSM { get; protected set; }

    [SerializeField]
    protected MenuPanel mainMenuPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);

            gameObject.SetActive(false);
        }
        else
        {
            Destroy(this);
            return;
        }

        MenuSM = new StateMachine();
        MenuSM.ChangeState(mainMenuPanel);
    }

    private void Update()
    {
        MenuSM.Update();
    }

    public static void Open()
    {
        Instance.gameObject.SetActive(true);
        Instance.MenuSM.ChangeState(Instance.mainMenuPanel);
    }

    public void ChangeMenuPanel(MenuPanel newPanel)
    {
        MenuSM.ChangeState(newPanel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
