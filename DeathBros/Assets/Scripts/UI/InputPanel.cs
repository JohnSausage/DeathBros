using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputPanel : MenuPanel
{
    [SerializeField]
    private Toggle tapJumpToggle;

    public override void Enter()
    {
        base.Enter();
        InputManager.Instance.settingInput = true;

        tapJumpToggle.isOn = InputManager.TapJump;
    }

    public override void Exit()
    {
        base.Exit();
        if(InputManager.Instance != null)
        InputManager.Instance.settingInput = false;
    }
}