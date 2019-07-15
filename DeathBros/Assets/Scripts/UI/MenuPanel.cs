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

    protected BaseEventData eventData = null;

    public virtual void Enter()
    {
        gameObject.SetActive(true);
        firstSelection.Select();
        firstSelection.OnSelect(eventData);

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        //rectTransform.offsetMax = new Vector2(-16,-16);
        //rectTransform.offsetMin = new Vector2(16, 16);

        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;

    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
        gameObject.SetActive(false);

    }
}
