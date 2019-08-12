using UnityEngine;

public class AIExitConditionSO : ScriptableObjectWithDrawer
{
    public int priority;

    public string exitState;

    public virtual bool CheckForExit(AIController aiCtr)
    {
        return false;
    }
}