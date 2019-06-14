using UnityEngine;
using System.Collections.Generic;

public class AIActionSO : ScriptableObjectWithDrawer
{
    public string actionName;

    public List<AIExitConditionSO> aiExitConditions;

    public virtual void Enter(AIController aiCtr)
    {
        aiCtr.Timer = 0;
    }

    public virtual void Execute(AIController aiCtr)
    {
        aiCtr.Timer++;

        for (int i = 0; i < aiExitConditions.Count; i++)
        {
            aiExitConditions[i].CheckForExit(aiCtr);
        }
    }

    public virtual void Exit(AIController aiCtr)
    {
    }
}


public class ScriptableObjectWithDrawer : ScriptableObject
{

}