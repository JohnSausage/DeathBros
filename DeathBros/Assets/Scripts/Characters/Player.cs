using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    public Stat wallSlideSpeed = new Stat("WallslideSpeed", 5);

    public CStates_AdvancedMovement advancedMovementStates;
    public CStates_Attack attackStates;

    public float soulCharge = 0;

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
        DirectionalInput = InputManager.Direction;
        StrongInputs = InputManager.Smash;
        TiltInput = InputManager.CStick;

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

        if (InputManager.BufferdDown("Jump") || StrongInputs.y > 0)
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (soulMeter > 50)
        {
            soulMeter -= soulMeterBalanceRate;

            if (soulMeter < 50) soulMeter = 50;
        }
        else
        {
            soulMeter += soulMeterBalanceRate;

            if (soulMeter > 50) soulMeter = 50;
        }

        soulMeter = Mathf.Clamp(soulMeter, 0, 100);
    }

    public override bool CheckForTiltAttacks()
    {
        if (Attack)
        {
            if (DirectionalInput == Vector2.zero)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.Jab));
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FTilt));
            }
            else if (DirectionalInput.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.UTilt));
            }
            else if (DirectionalInput.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DTilt));
            }

            return true;
        }

        if (TiltInput != Vector2.zero)
        {
            if (Mathf.Abs(TiltInput.x) > 0.5f)
            {
                Direction = TiltInput.x;

                CSMachine.ChangeState(GetAttackState(EAttackType.FTilt));
            }
            else if (TiltInput.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.UTilt));
            }
            else if (TiltInput.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DTilt));
            }

            return true;
        }


        return false;
    }

    public override bool CheckForSoulAttacks()
    {
        Vector2 smash = InputManager.Smash;

        if (Attack && smash != Vector2.zero)
        {
            if (Mathf.Abs(smash.x) > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FSoul));
            }
            else if (smash.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.USoul));
            }
            else if (smash.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DSoul));
            }

            return true;
        }
        else return false;
    }

    public override bool CheckForAerialAttacks()
    {
        if (Attack)
        {
            if (DirectionalInput == Vector2.zero)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.NAir));
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f && Mathf.Sign(DirectionalInput.x) == Direction)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FAir));
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f && Mathf.Sign(DirectionalInput.x) != Direction)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.BAir));
            }
            else if (DirectionalInput.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.UAir));
            }
            else if (DirectionalInput.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DAir));
            }

            return true;
        }

        if (TiltInput != Vector2.zero)
        {
            if (Mathf.Abs(TiltInput.x) > 0.5f && Mathf.Sign(TiltInput.x) == Direction)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FAir));
            }
            else if (Mathf.Abs(TiltInput.x) > 0.5f && Mathf.Sign(TiltInput.x) != Direction)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.BAir));
            }
            else if (TiltInput.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.UAir));
            }
            else if (TiltInput.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DAir));
            }

            return true;
        }

        return false;
    }
}
