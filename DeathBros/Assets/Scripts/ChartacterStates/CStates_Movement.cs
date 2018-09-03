using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CStates_Movement
{
    public void Init(Character chr)
    {
        idle.Init(chr);
        walking.Init(chr);
        jumpsquat.Init(chr);
    }

    public CS_Idle idle;
    public CS_Walking walking;
    public CS_Jumpsquat jumpsquat;
}

[System.Serializable]
public class CS_Idle : CState
{
    public override void Execute()
    {
        base.Execute();

        if (Mathf.Abs(chr.Movement.x) >= 0.1f)
        {
            ChangeState(chr.movementStates.walking);
        }
    }
}

[System.Serializable]
public class CS_Walking : CState
{
    public override void Execute()
    {
        base.Execute();

        if (Mathf.Abs(chr.Movement.x) <= 0.1f)
        {
            ChangeState(chr.movementStates.idle);
        }
    }
}

[System.Serializable]
public class CS_Jumpsquat : CState
{
}
