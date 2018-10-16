using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    protected CStates_Movement movementStates;

    [SerializeField]
    protected CS_Attack attackState;

    //public EnemyAI AI { get; protected set; }

    public override void Init()
    {
        base.Init();

        //AI = GetComponent<EnemyAI>();

        movementStates.Init(this);
        attackState.Init(this);

        CStates_InitExitStates();
    }

    private void Update()
    {
        //DirectionalInput = new Vector2(1, 0);

        /*
        DirectionalInput = AI.TargetDirection;

        if(AI.InAttackRange)
        {
            Attack = true;
        }
        else
        {
            Attack = false;
        }
        */
    }
}
