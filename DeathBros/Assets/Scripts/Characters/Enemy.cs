using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    protected CStates_Movement movementStates;

    public override void Init()
    {
        base.Init();

        movementStates.Init(this);

        CStates_InitExitStates();
    }

    private void Update()
    {
        //DirectionalInput = new Vector2(1, 0);
    }
}
