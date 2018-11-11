using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public string followTag = "Player";

    //public float attackRange = 1f;
    //public float aggroRange = 10f;
    //public bool aggroed = false;

    public StateMachine aiMachine;// { get; protected set; }

    public List<AI_State> aiStates;

    public Transform Target { get; protected set; }
    public Vector2 TargetDirection { get; protected set; }
    public Vector2 TargetVector { get; protected set; }

    public float DistanceToTarget { get { return TargetVector.magnitude; } }
    //public bool InAttackRange { get { return DistanceToTarget <= attackRange; } }
    //public bool InAggroRange { get { if (DistanceToTarget <= aggroRange) { aggroed = true; return true; } return false; } }

    public Enemy Enemy { get; protected set; }
    public Vector2 inputDirection;

    private void Awake()
    {
        Enemy = GetComponent<Enemy>();
        Target = GameObject.FindGameObjectWithTag(followTag).transform;

        for (int i = 0; i < aiStates.Count; i++)
        {
            aiStates[i].Init(this);
        }

        aiMachine = new StateMachine();
        if (aiStates.Count > 0) aiMachine.ChangeState(aiStates[0]);
    }

    protected void FixedUpdate()
    {
        float targetX = Mathf.Clamp(Target.position.x - transform.position.x, -1f, 1f);
        float targetY = Mathf.Clamp(Target.position.y - transform.position.y, -1f, 1f);

        TargetDirection = new Vector2(targetX, targetY);

        TargetVector = Target.position - transform.position;

        aiMachine.Update();
    }

    public void ChangeState(string actionName)
    {
        AI_State changeState = aiStates.Find(x => x.aiAction.actionName == actionName);

        if (changeState != null) aiMachine.ChangeState(changeState);
    }
}

[System.Serializable]
public class AI_State : IState
{
    public AIActionSO aiAction;

    public List<AIExitConditionSO> aiExitConditions;

    public Vector2 setInputDirection { get; protected set; }

    protected AIController aiCtr;

    public void Init(AIController aiCtr)
    {
        this.aiCtr = aiCtr;
    }

    public void Enter()
    {
        aiAction.Enter(aiCtr);
    }

    public void Execute()
    {
        aiAction.Execute(aiCtr);

        for (int i = 0; i < aiExitConditions.Count; i++)
        {
            aiExitConditions[i].CheckForExit(aiCtr);
        }
    }

    public void Exit()
    {
    }
}
