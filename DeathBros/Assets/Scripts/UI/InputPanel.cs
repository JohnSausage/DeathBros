using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanel : MenuPanel
{
    public override void Enter()
    {
        base.Enter();
        InputManager.Instance.settingInput = true;
    }

    public override void Exit()
    {
        base.Exit();
        if(InputManager.Instance != null)
        InputManager.Instance.settingInput = false;
    }
}