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

    public void ButtonPressed(int index)
    {
        CardPanel cardPanel = FindObjectOfType<CardPanel>();

        cardPanel.PressButtonCardData(index);
    }
}
