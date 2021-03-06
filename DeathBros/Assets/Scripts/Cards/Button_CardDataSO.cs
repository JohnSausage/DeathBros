﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_CardDataSO : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public int specialIndex;
    public CardDataSO cardDataSO { get; protected set; }

    protected SpriteFontText text;

    public Vector2 OriginalLocalPosition { get; protected set; }

    private void Awake()
    {
        OriginalLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        transform.localPosition = OriginalLocalPosition;
    }

    public void SetCardData(CardDataSO cardDataSO)
    {
        if (cardDataSO == null)
        {
            text = GetComponentInChildren<SpriteFontText>();
            text.SetText("NOT SET");
        }
        else
        {
            this.cardDataSO = cardDataSO;

            text = GetComponentInChildren<SpriteFontText>();
            text.SetText(cardDataSO.title);
        }
    }

    public void ButtonPressed()
    {
        Panel_AvlSkills panel_AvlSkills = FindObjectOfType<Panel_AvlSkills>();

        if (panel_AvlSkills == null)
        {
            Debug.Log("panel_AvlSkills not found");
            return;
        }

        panel_AvlSkills.Press_AvlSkillButton(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.localPosition += Vector3.right * 16;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localPosition += Vector3.left * 16;
    }
}
