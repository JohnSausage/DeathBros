using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public string followTag = "Player";

    public float attackRange = 1f;
    public float aggroRange = 10f;

    public bool aggroed = false;

    public Transform target;

    public List<AIState> RandomStates { get; protected set; }
    public AI_Follow aiFollowPlayer;

    public StateMachine aiMachine;// { get; protected set; }

    public Vector2 TargetDirection;// { get; protected set; }
    public Vector2 TargetVector;// { get; protected set; }

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

    public bool InAggroRange
    {
        get
        {
            if(DistanceToTarget <= aggroRange)
            {
                aggroed = true;
                return true;
            }
            return false;
        }

    }

    public Enemy chr { get; protected set; }

    private void Awake()
    {
        chr = GetComponent<Enemy>();
        target = GameObject.FindGameObjectWithTag(followTag).transform;

        RandomStates = new List<AIState>();
    }

    protected void FixedUpdate()
    {
        float targetX = Mathf.Clamp(target.position.x - transform.position.x, -1f, 1f);
        float targetY = Mathf.Clamp(target.position.y - transform.position.y, -1f, 1f);

        TargetDirection = new Vector2(targetX, targetY);

        TargetVector = target.position - transform.position;

        aiMachine.Update();
    }

    public AIState GetRandomState()
    {
        AIState aIState = null;

        if (RandomStates.Count > 0)
        {
            while (aIState == null)
            {
                int nr = Random.Range(0, RandomStates.Count);

                if (RandomStates[nr].RandomEnterChance > Random.Range(0, 100))
                {
                    aIState = RandomStates[nr];
                }
            }
        }

        return aIState;
    }

    public virtual AIState GetAttackState()
    {
        return null;
    }
}
