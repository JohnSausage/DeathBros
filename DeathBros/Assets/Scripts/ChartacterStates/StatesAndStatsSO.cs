using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatesAndStats")]
public class StatesAndStatsSO : ScriptableObject
{
    public string idle_anim = "idle";
    public string walking_anim = "walking";
    public string running_anim = "running";
    public string jumpsquat_anim = "jumpsquat";
    public int jumpsquat_duration = 4;

    public List<CState> GenerateStates()
    {
        List<CState> cstates = new List<CState>();



        return cstates;
    }
}