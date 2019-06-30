using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectAutoScroll : MonoBehaviour
{
    [SerializeField]
    protected float buttonHeight;

    [SerializeField]
    protected float scrollRectHeight;

    [SerializeField]
    protected float spacing;

    protected ScrollRect scrollRect;

    public List<Selectable> selectables;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

        foreach (Selectable s in scrollRect.content.GetComponentsInChildren<Selectable>())
        {
            selectables.Add(s);
        }

        buttonHeight = selectables[0].GetComponent<RectTransform>().rect.height + spacing;
        scrollRectHeight = GetComponent<RectTransform>().rect.height;
    }

    void Update()
    {
        for (int i = 0; i < selectables.Count; i++)
        {
            if (selectables[i].gameObject == EventSystem.current.currentSelectedGameObject)
            {
                float relPos = (float)((float)i / (float)(selectables.Count - 1f));

                scrollRect.verticalNormalizedPosition = 1f - relPos;
            }
        }
    }
}
