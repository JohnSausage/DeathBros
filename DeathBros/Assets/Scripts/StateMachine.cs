[System.Serializable]
public class StateMachine
{
    public string currentState;

    public IState CurrentState { get; protected set; }
    protected IState previousState;

    public StateMachine()
    {
        CurrentState = new EmptyState();
        previousState = new EmptyState();
    }

    public void Update()
    {
        CurrentState.Execute();

        currentState = CurrentState.ToString();
    }

    public void ChangeState(IState newState)
    {
        CurrentState.Exit();
        previousState = CurrentState;
        CurrentState = newState;
        newState.Enter();
    }

    public void GoToPreviousState()
    {
        ChangeState(previousState);
    }
}

public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

public class EmptyState : IState
{
    public void Enter()
    {
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}
