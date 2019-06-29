using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectAutoScroll : MonoBehaviour
{
    protected ScrollRect scrollRect;

    public List<Selectable> selectables;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

        foreach (Selectable s in scrollRect.content.GetComponentsInChildren<Selectable>())
        {
            selectables.Add(s);
        }
    }

    void Update()
    {
        for (int i = 0; i < selectables.Count; i++)
        {
            if (selectables[i].gameObject == EventSystem.current.currentSelectedGameObject)
            {
                scrollRect.verticalNormalizedPosition = 1f - (float)((float)i / (float)(selectables.Count - 1f));
            }
        }
    }
}
