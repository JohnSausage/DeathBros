using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NES_Player : NES_Character
{
    protected override void GetInputs()
    {
        base.GetInputs();

        DirectionalInput = InputManager.Direction;
        StrongInputs = InputManager.Smash;
        TiltInput = InputManager.CStick;

        if (Mathf.Abs(DirectionalInput.x) < 0.25f) DirectionalInput = new Vector2(0, DirectionalInput.y);

        if (InputManager.BufferdDown("Attack")) Attack = true;
        else Attack = false;


        if (InputManager.Attack.GetButton()) HoldAttack = true;
        else HoldAttack = false;

        if (InputManager.BufferdDown("Special")) Special = true;
        else Special = false;


        if (InputManager.Special.GetButton()) HoldSpecial = true;
        else HoldSpecial = false;

        if (InputManager.BufferdDown("Jump") || InputManager.BufferdDown("Jump2") || (StrongInputs.y > 0) && InputManager.TapJump == true) Jump = true;
        else
        {
            //reset at the end of FixedUpdate to not miss any inputs
            Jump = false;
        }

        if (InputManager.Jump.GetButton() || InputManager.Jump2.GetButton()) HoldJump = true;
        else HoldJump = false;


        if ((InputManager.BufferdDown("Shield"))) Shield = true;
        else Shield = false;

        if (InputManager.Shield.GetButton()) HoldShield = true;
        else HoldShield = false;

        if (InputManager.BufferdDown("Grab")) Grab = true;
        else Grab = false;
    }
}
