using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPanel : MonoBehaviour, IState
{
    [SerializeField]
    protected Selectable firstSelection;

    private void Awake()
    {
        Exit();
    }

    BaseEventData eventData = null;

    public virtual void Enter()
    {
        gameObject.SetActive(true);
        firstSelection.Select();
        firstSelection.OnSelect(eventData);
    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
        gameObject.SetActive(false);

    }
}
