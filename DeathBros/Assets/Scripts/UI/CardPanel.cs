using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPanel : MenuPanel
{
    [SerializeField]
    private CardListControl cardListControl;

    public CardButton selectedCard;

    [SerializeField]
    private GameObject[] comboCards;

    public override void Enter()
    {
        base.Enter();

        cardListControl.LoadCards();


        for (int i = 0; i < comboCards.Length; i++)
        {
            comboCards[i].GetComponent<CardButton>().SetCard(InventoryManager.comboCards[i]);
        }
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void Exit()
    {
        base.Exit();
        selectedCard.SetCard(null);
    }

    public void SelectCard(CardData cardData)
    {
        selectedCard.GetComponent<CardButton>().SetCard(cardData);
    }

    public void AddComboCard()
    {
        if(selectedCard.GetComponent<CardButton>().cardData != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if(comboCards[i].GetComponent<CardButton>().cardData == null)
                {
                    comboCards[i].GetComponent<CardButton>().SetCard(selectedCard.cardData);
                    break;
                }
            }
        }
    }
}
