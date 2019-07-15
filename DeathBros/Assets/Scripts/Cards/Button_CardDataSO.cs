using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_CardDataSO : MonoBehaviour
{
    public int specialIndex;
    public CardDataSO cardDataSO { get; protected set; }

    protected SpriteFontText text;


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
}
