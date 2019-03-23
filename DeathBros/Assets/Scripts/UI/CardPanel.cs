using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPanel : MenuPanel
{
    [SerializeField]
    private CardListControl cardListControl;

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
}
