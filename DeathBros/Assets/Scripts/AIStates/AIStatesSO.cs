using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI/AIStateSO")]
public class AIStatesSO : ScriptableObjectWithDrawer
{
    public List<AIActionSO> aiActions;
}
