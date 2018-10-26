using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAI : EnemyAI
{
    public AI_Flee aiFlee;
    public AI_Attack aiAttack;
    public AI_Walk aiWalk;

    private void Start()
    {
        chr = GetComponent<Enemy>();
        target = GameObject.FindGameObjectWithTag(followTag).transform;

        //enemy.HitM.EnemyHit += EnemyHit;
        chr.currentAttack.AttackOver += AttackOver;

        aiFollowPlayer.Init(this);
        aiFlee.Init(this);
        aiAttack.Init(this);
        aiWalk.Init(this);

        aiMachine = new StateMachine();
        aiMachine.ChangeState(aiWalk);
    }

    private void EnemyHit(Character hitChr)
    {
        //Debug.Log(hitChr.name + " was hit by " + enemy.name);
    }

    private void AttackOver(CS_Attack cs_attack)
    {
        aiMachine.ChangeState(aiWalk);
    }

    public override AIState GetAttackState()
    {
        return aiAttack;
    }
}
