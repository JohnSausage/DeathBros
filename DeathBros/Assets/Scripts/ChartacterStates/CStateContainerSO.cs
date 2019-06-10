using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CStateContainerSO")]
public class CStateContainerSO : ScriptableObject
{
    public List<CState> cStates;
}
