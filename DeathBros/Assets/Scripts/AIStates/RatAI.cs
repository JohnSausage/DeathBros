using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAI : MonoBehaviour
{
    public string followTag = "Player";

    public float attackRange = 1f;

    public Transform target;

    public int randomChangeStateTime = 60;
    private int randomizedTime;
    private int timer = 0;
    protected bool attacking = false;

    public StateMachine aiMachine;// { get; protected set; }

    public Vector2 TargetDirection;// { get; protected set; }
    public Vector2 TargetVector;// { get; protected set; }

    private AI_Follow aiFollowPlayer;
    private AI_Flee aiFlee;
    private AI_Attack aiAttack;

    public float DistanceToTarget
    {
        get
        {
            return TargetVector.magnitude;
        }
    }

    public bool InAttackRange
    {
        get
        {
            return DistanceToTarget <= attackRange;
        }
    }

    public Enemy enemy { get; protected set; }

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        target = GameObject.FindGameObjectWithTag(followTag).transform;

        //enemy.HitM.EnemyHit += EnemyHit;
        enemy.currentAttack.AttackOver += AttackOver;

        aiFollowPlayer = new AI_Follow(this);
        aiFlee = new AI_Flee(this);
        aiAttack = new AI_Attack(this);

        aiMachine = new StateMachine();
        aiMachine.ChangeState(aiFollowPlayer);

        randomizedTime = randomChangeStateTime + Random.Range(-30, 30);
    }

    private void FixedUpdate()
    {
        float targetX = Mathf.Clamp(target.position.x - transform.position.x, -1f, 1f);
        float targetY = Mathf.Clamp(target.position.y - transform.position.y, -1f, 1f);

        TargetDirection = new Vector2(targetX, targetY);

        TargetVector = target.position - transform.position;

        if (!attacking)
        {
            timer++;
            if (timer > randomizedTime)
            {
                timer = 0;
                randomizedTime = randomChangeStateTime + Random.Range(-30, 30);

                if (Random.Range(0, 100) > 30)
                {
                    aiMachine.ChangeState(aiFollowPlayer);
                }
                else
                {
                    aiMachine.ChangeState(aiFlee);
                }
            }
        }

        if (TargetVector.magnitude <= 1)
        {
            aiMachine.ChangeState(aiAttack);
            attacking = true;
        }

        aiMachine.Update();
    }

    private void EnemyHit(Character hitChr)
    {
        //Debug.Log(hitChr.name + " was hit by " + enemy.name);
    }

    private void AttackOver(CS_Attack cs_attack)
    {
        attacking = false;
        aiMachine.ChangeState(aiFollowPlayer);
    }
}
