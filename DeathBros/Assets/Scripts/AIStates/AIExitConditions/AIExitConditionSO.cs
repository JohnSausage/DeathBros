using UnityEngine;

public class AIExitConditionSO : ScriptableObject
{
    public string exitState;

    public virtual void CheckForExit(AIController aiCtr)
    {
    }
}