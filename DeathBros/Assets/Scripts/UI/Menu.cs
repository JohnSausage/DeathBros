using UnityEngine;

public class Menu : MonoBehaviour 
{
    public StateMachine MenuSM { get; protected set; }

    [SerializeField]
    protected bool openOnStart = true;

    [SerializeField]
    protected MenuPanel mainMenuPanel;

    void Awake()
    {
        MenuSM = new StateMachine();
        MenuSM.ChangeState(mainMenuPanel);
    }

    private void Start()
    {
        if (openOnStart)
        {
            Open();
        }
    }

    public  void Open()
    {
        gameObject.SetActive(true);
        MenuSM.ChangeState(mainMenuPanel);
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
