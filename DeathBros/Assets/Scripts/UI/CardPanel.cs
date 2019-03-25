using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPanel : MenuPanel
{
    [SerializeField]
    private CardListControl cardListControl;

    public CardButton selectedCard;

    public override void Enter()
    {
        base.Enter();

        cardListControl.LoadCards();
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public void SelectCard(CardData cardData)
    {
        selectedCard.GetComponent<CardButton>().SetCard(cardData);
    }
}
