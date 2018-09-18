using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    public Stat wallSlideSpeed = new Stat("WallslideSpeed", 5);

    public CStates_AdvancedMovement advancedMovementStates;
    public CStates_Attack attackStates;

    public override void Init()
    {
        base.Init();
        soundFolderName = "Sounds/Player/";

        advancedMovementStates.Init(this);
        attackStates.Init(this);

        stats.AddStat(wallSlideSpeed);

        CStates_InitExitStates();
    }

    void Update()
    {
        //DirectionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));
        DirectionalInput = InputManager.Direction;
        if (Mathf.Abs(DirectionalInput.x) < 0.2f) DirectionalInput = new Vector2(0, DirectionalInput.y);

        if (InputManager.BufferdDown("Attack"))
        {
            Attack = true;
        }
        else
        {
            Attack = false;
        }

        if (InputManager.Attack.GetButton())
        {
            HoldAttack = true;
        }
        else
        {
            HoldAttack = false;
        }

        if (InputManager.BufferdDown("Jump"))
        {
            Jump = true;
        }

        else
        {
            //reset at the end of FixedUpdate to not miss any inputs
            //Jump = false;
        }


        if (InputManager.Jump.GetButton())
        {
            HoldJump = true;
        }
        else
        {
            HoldJump = false;
        }
    }

    public override bool CheckForTiltAttacks()
    {
        if (Attack)
        {
            if (DirectionalInput == Vector2.zero)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.Jab));
                Debug.Log("Jab");
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FTilt));
                Debug.Log("FTilt");
            }
            else if (DirectionalInput.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.UTilt));
                Debug.Log("UTilt");

            }
            else if (DirectionalInput.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DTilt));
                Debug.Log("DTilt");
            }

            return true;
        }
        else return false;
    }

    public override bool CheckForSoulAttacks()
    {
        Vector2 smash = InputManager.Smash;

        if (Attack && smash != Vector2.zero)
        {
            if (Mathf.Abs(smash.x) > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FSoul));
                Debug.Log("FSmash");
            }
            else if (smash.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.USoul));
                Debug.Log("USmash");
            }
            else if (smash.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DSoul));
                Debug.Log("DSmash");
            }

            return true;
        }
        else return false;
    }
}
