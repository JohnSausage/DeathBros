using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CStates_Movement
{
    [Space]

    public CS_Idle idle;
    public CS_Walking walking;
    public CS_Jumpsquat jumpsquat;
    public CS_Jumping jumping;
    public CS_Landing landing;
    public CS_Hitstun hitstun;
    public CS_Hitfreeze hitfreeze;
    public CS_HitLand hitLand;
    public CS_Shield shield;
    public CS_Die die;
    public CS_Dead dead;
    public CS_StandUp standUp;
    public CS_HitLanded hitLanded;

    public virtual void Init(Character chr)
    {
        idle.Init(chr);
        walking.Init(chr);
        jumpsquat.Init(chr);
        jumping.Init(chr);
        landing.Init(chr);
        hitstun.Init(chr);
        hitfreeze.Init(chr);
        hitLand.Init(chr);
        shield.Init(chr);
        die.Init(chr);
        dead.Init(chr);
        standUp.Init(chr);
        hitLanded.Init(chr);

        chr.CSMachine.ChangeState(idle);
    }
}

[System.Serializable]
public class CStates_AdvancedMovement : CStates_Movement
{
    [Space]

    public CS_DoubleJumpsquat doubleJumpsquat;
    public CS_Wallsliding wallsliding;
    public CS_WalljumpStart walljumpStart;
    public CS_Walljumping walljumping;
    public CS_Skid skid;
    public CS_Dash dash;
    public CS_Crouch crouch;
    public CS_Roll roll;
    public CS_ThrowItem throwItem;
    public CS_AirDodge airDodge;
    public CS_ShieldHit shieldHit;

    public override void Init(Character chr)
    {
        base.Init(chr);

        doubleJumpsquat.Init(chr);
        wallsliding.Init(chr);
        walljumpStart.Init(chr);
        walljumping.Init(chr);
        skid.Init(chr);
        dash.Init(chr);
        crouch.Init(chr);
        roll.Init(chr);
        throwItem.Init(chr);
        airDodge.Init(chr);
        shieldHit.Init(chr);
    }
}

[System.Serializable]
public class CS_Idle : CState
{
    CS_Walking walking;
    CS_Jumpsquat jumpsquat;
    CS_Crouch crouch;

    public override void Init(Character chr)
    {
        base.Init(chr);
        if (animationName == "") animationName = "idle";
    }

    public override void InitExitStates()
    {
        base.InitExitStates();

        walking = (CS_Walking)chr.GetState(typeof(CS_Walking));
        jumpsquat = (CS_Jumpsquat)chr.GetState(typeof(CS_Jumpsquat));
        crouch = (CS_Crouch)chr.GetState(typeof(CS_Crouch));
    }

    public override void Enter()
    {
        base.Enter();

        chr.jumpsUsed = 0;

        chr.Anim.animationSpeed = 1.5f;
    }

    public override void Execute()
    {
        base.Execute();

        chr.GetInputs();

        if (chr.DirectionalInput.y < -0.5f)
            ChangeState(crouch);

        if (Mathf.Abs(chr.DirectionalInput.x) != 0)
            ChangeState(walking);

        if (chr.StrongInputs.x != 0)
            ChangeState(typeof(CS_Dash));

        if (chr.HoldShield)
            ChangeState(typeof(CS_Shield));

        if (chr.Jump)
            ChangeState(jumpsquat);

        chr.CS_CheckIfStillGrounded();

        if (chr.CheckForSpecialAttacks() == false)
        {
            if (chr.CheckForSoulAttacks() == false)
            {
                chr.CheckForTiltAttacks();
            }
        }
    }
}

[System.Serializable]
public class CS_Walking : CState
{
    [SerializeField]
    string animationSlow;
    [SerializeField]
    float direction = 0;

    protected FrameAnimation fAnimSlow;

    public override void Init(Character chr)
    {
        base.Init(chr);

        if (animationSlow != "")
        {
            fAnimSlow = chr.Anim.GetAnimation(animationSlow);
        }
    }
    public override void Enter()
    {
        base.Enter();

        direction = Mathf.Sign(chr.DirectionalInput.x);

        if (chr.DirectionalInput.x < 0) chr.Spr.flipX = true;
        if (chr.DirectionalInput.x > 0) chr.Spr.flipX = false;
    }

    public override void Execute()
    {
        base.Execute();

        chr.Anim.animationSpeed = Mathf.Abs(chr.DirectionalInput.x);

        if (Mathf.Abs(chr.DirectionalInput.x) < 0.5f && fAnimSlow != null)
            chr.Anim.ChangeAnimation(fAnimSlow);


        if (Mathf.Abs(chr.DirectionalInput.x) >= 0.7f)
            chr.Anim.ChangeAnimation(animation);


        chr.GetInputs();

        if (Mathf.Abs(chr.DirectionalInput.x) == 0f || Mathf.Sign(chr.DirectionalInput.x) != direction)
        {
            CState exit = (CS_Skid)chr.GetState(typeof(CS_Skid));

            if (exit != null)
            {
                CS_Skid skid = (CS_Skid)exit;
                skid.direction = direction;
            }
            else
            {
                exit = (CS_Idle)chr.GetState(typeof(CS_Idle));
            }
            ChangeState(exit);

        }

        if (chr.HoldShield)
            ChangeState(typeof(CS_Shield));

        if (chr.Jump)
            ChangeState(typeof(CS_Jumpsquat));


        if (chr.DirectionalInput.y < -0.5f)
            ChangeState(typeof(CS_Crouch));


        if (chr.CheckForSpecialAttacks() == false)
        {
            if (chr.CheckForSoulAttacks() == false)
            {
                chr.CheckForTiltAttacks();
            }
        }

        chr.CS_CheckIfStillGrounded();
    }
}

[System.Serializable]
public class CS_Dash : CState
{
    [SerializeField]
    public int duration = 10;
    private int timer = 0;

    private float dirX;

    public override void Enter()
    {
        base.Enter();

        timer = 0;
        dirX = Mathf.Sign(chr.DirectionalInput.x);

        chr.Direction = dirX;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.GetInputs();

        if (Mathf.Sign(chr.DirectionalInput.x) != dirX && Mathf.Abs(chr.DirectionalInput.x) > 0.5f)
        {
            //ChangeState(typeof(CS_Dash));
            ChangeState(typeof(CS_Idle));
        }


        if (timer >= duration)
        {
            if (chr.DirectionalInput.x == 0)
            {
                ChangeState(typeof(CS_Idle));
            }
            else
            {
                ChangeState(typeof(CS_Walking));
            }
        }

        if (timer <= 2)
            chr.SetInputs(new Vector2(dirX * 0.05f, 0));
        else
            chr.SetInputs(new Vector2(dirX * 1.2f, 0));

        if (chr.Jump)
        {
            ChangeState(typeof(CS_Jumpsquat));
        }

        if (timer <= 5)
        {
            if (chr.Attack)
            {
                ChangeState(EAttackType.FSoul);
            }
        }

        if (chr.CheckForSpecialAttacks() == false)
        {
            if (chr.CheckForSoulAttacks() == false)
            {
                chr.CheckForTiltAttacks();
            }
        }
    }
}

[System.Serializable]
public class CS_Skid : CState
{
    public float direction { get; set; }

    [SerializeField]
    public int duration = 6;

    [SerializeField]
    public int idleOutDuration = 3;
    private int timer;

    private int idleTimer;

    private bool changedDirection;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        idleTimer = 0;

        changedDirection = false;

        chr.Anim.animationSpeed = 0.5f;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.GetInputs();


        if (!changedDirection)
        {
            if (Mathf.Sign(chr.DirectionalInput.x) != direction && chr.DirectionalInput.x != 0) //don't change direction if there is no input
            {
                chr.Spr.flipX = !chr.Spr.flipX;
                changedDirection = true;
            }
        }


        if (chr.DirectionalInput.x == 0)
        {
            idleTimer++;
        }

        chr.SetInputs(new Vector2(direction * 0.9f, 0));

        if (timer >= duration)
        {
            //ChangeState(chr.advancedMovementStates.idle);
            ChangeState(typeof(CS_Idle));

            chr.GetInputs();
        }
        else if (idleTimer >= idleOutDuration)
        {
            ChangeState(typeof(CS_Idle));

            chr.GetInputs();
        }
        else if (chr.Jump)
        {
            //ChangeState(chr.advancedMovementStates.jumpsquat);
            ChangeState(typeof(CS_Jumpsquat));
        }
    }
}

[System.Serializable]
public class CS_Crouch : CState
{
    public override void Execute()
    {
        base.Execute();

        chr.GetInputs();
        chr.SetInputs(new Vector2(0, chr.DirectionalInput.y));

        if (chr.StrongInputs.y < 0)
        {
            chr.Ctr.fallThroughPlatform = true;
        }

        if (chr.DirectionalInput.y >= -0.25f)
        {
            chr.CS_SetIdle();
        }

        if (chr.Jump)
        {
            ChangeState(typeof(CS_Jumpsquat));
        }

        chr.CS_CheckIfStillGrounded();

        if (chr.CheckForSpecialAttacks() == false)
        {
            if (chr.CheckForSoulAttacks() == false)
            {
                chr.CheckForTiltAttacks();
            }
        }
    }
}

[System.Serializable]
public class CS_Jumpsquat : CState
{
    [SerializeField]
    public int duration;
    private int timer;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
    }

    public override void Execute()
    {
        base.Execute();

        //chr.ModInputs(0.5f);

        timer++;

        if (chr.Attack && chr.StrongInputs.y > 0)
        {
            ChangeState(EAttackType.USoul);
        }

        if (timer >= duration)
        {
            if (chr.TiltInput != Vector2.zero)
            {
                chr.CheckForAerialAttacks();
                chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength") * 0.75f;
            }
            else
            {
                ChangeState(typeof(CS_Jumping));

                chr.GetInputs();
                if (chr.HoldJump || chr.DirectionalInput.y > 0.75f)
                {
                    chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength");
                }
                else
                {
                    chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength") * 0.75f;
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        chr.jumpsUsed++;
    }
}

[System.Serializable]
public class CS_DoubleJumpsquat : CState
{
    [SerializeField]
    public int duration = 3;
    private int timer;

    public override void Enter()
    {
        base.Enter();

        timer = 0;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        if (timer >= duration)
        {
            //ChangeState(chr.advancedMovementStates.jumping);
            ChangeState(typeof(CS_Jumping));
            chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength");
        }
    }

    public override void Exit()
    {
        base.Exit();
        chr.jumpsUsed++;

        chr.Ctr.fastFall = false;
    }
}

[System.Serializable]
public class CS_Landing : CState
{
    [SerializeField]
    public int duration = 3;

    public int addDuration { get; set; } //additional landing lag from eg aerials

    private int timer;

    private float dirX;

    public override void Enter()
    {
        base.Enter();

        timer = 0;

        dirX = 0;
        if (chr.DirectionalInput.x != 0)
            dirX = chr.DirectionalInput.x;
    }

    public override void Execute()
    {
        base.Execute();

        dirX *= 0.8f;
        chr.SetInputs(new Vector2(dirX, 0));


        timer++;

        if (timer >= (duration + addDuration))
        {
            addDuration = 0; //clear additional landing lag when landing

            chr.GetInputs();
            if (chr.DirectionalInput.y < 0)
            {
                ChangeState(typeof(CS_Crouch));
            }
            else
            {
                chr.CS_SetIdle();
            }
        }
    }
}

[System.Serializable]
public class CS_Jumping : CState
{
    [SerializeField]
    public string jumpRisingAnimation;
    FrameAnimation jumpRisingFA;

    public override void Init(Character chr)
    {
        base.Init(chr);

        jumpRisingFA = chr.Anim.GetAnimation(jumpRisingAnimation);
    }

    public override void Execute()
    {
        base.Execute();

        if (chr.canChangeDirctionInAir)
            chr.Direction = Mathf.Sign(chr.DirectionalInput.x);

        if (chr.Ctr.velocity.y > 0)
            chr.Anim.ChangeAnimation(jumpRisingFA);
        else
            chr.Anim.ChangeAnimation(animation);


        chr.GetInputs();

        //if (chr.Ctr.velocity.y < 0 && chr.DirectionalInput.y < -0.5f)
        if (chr.Ctr.velocity.y < 0 && chr.StrongInputs.y == -1)
        {
            chr.Ctr.fastFall = true;
        }


        chr.CS_CheckLanding();

        if (chr.Jump)
        {

            if (chr.jumpsUsed < chr.GetCurrentStatValue("Jumps"))
            {
                ChangeState(typeof(CS_DoubleJumpsquat));
            }
        }

        if (chr.Ctr.onWall)
        {
            ChangeState(typeof(CS_Wallsliding));
        }

        if (chr.Shield)
        {
            ChangeState(typeof(CS_AirDodge));
        }

        chr.CheckForAerialAttacks();
    }
}

[System.Serializable]
public class CS_Wallsliding : CState
{
    [SerializeField]
    public string wallUpAnimation;

    private FrameAnimation wallUpFA;
    private CS_WalljumpStart walljumpStart;
    private CS_Jumping jumping;

    private float dirBeforeWallslide;

    public override void Init(Character chr)
    {
        base.Init(chr);

        wallUpFA = chr.Anim.GetAnimation(wallUpAnimation);

        if (chr.Ctr.velocity.y > 0)
        {
            chr.Anim.ChangeAnimation(wallUpFA);
        }
        else
        {
            chr.Anim.ChangeAnimation(animation);
        }
    }

    public override void InitExitStates()
    {
        base.InitExitStates();

        walljumpStart = (CS_WalljumpStart)chr.GetState(typeof(CS_WalljumpStart));
        jumping = (CS_Jumping)chr.GetState(typeof(CS_Jumping));
    }

    public override void Enter()
    {
        base.Enter(); //Dont automatically change animation to slideDwon, check vel.y first!

        dirBeforeWallslide = chr.Direction;

        if (chr.Ctr.velocity.y > 0)
        {
            chr.Anim.ChangeAnimation(wallUpFA);
        }
        else
        {
            chr.Anim.ChangeAnimation(animation);
        }
    }

    public override void Execute()
    {
        base.Execute();

        if (chr.Ctr.velocity.y > 0)
        {
            chr.Anim.ChangeAnimation(wallUpFA);
        }
        else
        {
            chr.Anim.ChangeAnimation(animation);
        }

        chr.Spr.flipX = chr.Ctr.wallDirection == -1;

        chr.GetInputs();

        chr.CS_CheckLanding();

        if (!chr.Ctr.onWall)
        {
            ChangeState(jumping);
        }

        chr.CheckForAerialAttacks();

        if (chr.Jump)// || (chr.StrongInputs.x != 0 && Mathf.Sign(chr.StrongInputs.x) != chr.Ctr.wallDirection))
        {
            walljumpStart.walljumpDirection = -chr.Ctr.wallDirection;
            ChangeState(walljumpStart);
            /*
            if (chr.Ctr.wallDirection != Mathf.Sign(chr.DirectionalInput.x))
            {
                walljumpStart.walljumpDirection = -chr.Ctr.wallDirection;
                //ChangeState(chr.advancedMovementStates.walljumpStart);
                ChangeState(walljumpStart);
            }
            else if (chr.jumpsUsed < chr.stats.jumps.CurrentValue)
            {
                //ChangeState(chr.advancedMovementStates.doubleJumpsquat);
                ChangeState(typeof(CS_DoubleJumpsquat));
            }
            */
        }
    }

    public override void Exit()
    {
        base.Exit();

        //jumping.AllowWallJump = true;
        chr.Direction = dirBeforeWallslide;
    }
}

[System.Serializable]
public class CS_WalljumpStart : CState
{
    public int walljumpDirection { get; set; }

    [SerializeField]
    public float jumpHeightReductionFactor = 0.75f;
    [SerializeField]
    public int duration = 3;

    private CS_Walljumping walljumping;
    private int timer;

    public override void InitExitStates()
    {
        base.InitExitStates();

        walljumping = (CS_Walljumping)chr.GetState(typeof(CS_Walljumping));
    }
    public override void Enter()
    {
        base.Enter();
        chr.Spr.flipX = chr.Ctr.wallDirection == 1;
        timer = 0;
        chr.SetInputs(Vector2.zero);
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        if (timer >= duration)
        {
            chr.SetInputs(new Vector2(walljumpDirection, 0));
            chr.Ctr.jumpVelocity = chr.GetCurrentStatValue("JumpStrength") * jumpHeightReductionFactor;

            walljumping.walljumpDirection = walljumpDirection;
            //ChangeState(chr.advancedMovementStates.walljumping);
            ChangeState(walljumping);
        }
    }
}

[System.Serializable]
public class CS_Walljumping : CState
{
    public int walljumpDirection { get; set; }

    [SerializeField]
    public int duration = 20;
    private int timer;

    private CS_Jumping jumping;

    public override void InitExitStates()
    {
        base.InitExitStates();

        jumping = (CS_Jumping)chr.GetState(typeof(CS_Jumping));
    }

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        //jumping.AllowWallJump = false;

    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.GetInputs();

        chr.CheckForAerialAttacks();

        chr.SetInputs(new Vector2(walljumpDirection, 0));

        chr.CS_CheckLanding();

        if (chr.Ctr.onWall)
        {
            //ChangeState(chr.advancedMovementStates.wallsliding);
            ChangeState(typeof(CS_Wallsliding));
        }

        /*
        if (chr.Jump && chr.jumpsUsed < chr.jumps)
        {
            ChangeState(chr.movementStates.doubleJumpsquat);
        }
        */

        if (timer >= duration)
        {
            //ChangeState(chr.advancedMovementStates.jumping);
            ChangeState(jumping);
        }
    }
}

[System.Serializable]
public class CS_Hitstun : CState
{
    [SerializeField]
    public string hitstunFallAnimation;
    FrameAnimation hitstunFallFA;

    //public int freezeStart = 15;
    //public int freezeEnd = 45;
    public int minDuration = 3;

    //public float knockbackX { get; set; }
    private int timer;

    private float spawnCloudVelocity = 10;

    public int HitStunDuration { get; set; }

    public override void Init(Character chr)
    {
        base.Init(chr);

        hitstunFallFA = chr.Anim.GetAnimation(hitstunFallAnimation);
    }

    public override void Enter()
    {
        base.Enter();

        timer = 0;
        chr.Ctr.inControl = false;

        chr.Direction = Mathf.Sign(chr.Ctr.velocity.x);

        EffectManager.SpawnEffect("Cloud1", chr.transform.position);
    }

    public override void Execute()
    {
        base.Execute();

        if (chr.Ctr.velocity.y > 0)
            chr.Anim.ChangeAnimation(animation);
        else
            chr.Anim.ChangeAnimation(hitstunFallFA);


        chr.GetInputs();

        timer++;

        if (timer <= 60)
        {
            if (chr.Ctr.velocity.magnitude * 60 >= spawnCloudVelocity)
            {
                if (timer <= 30)
                {
                    if (timer % 4 == 0) EffectManager.SpawnEffect("Cloud1", chr.transform.position);
                }

                if (timer % 10 == 0) EffectManager.SpawnEffect("Cloud1", chr.transform.position);


            }
        }

        if (timer > HitStunDuration)
        {
            chr.Ctr.inControl = true;
            ChangeState(typeof(CS_Jumping));

            chr.RaiseComboOverEvent();
        }

        if (chr.Ctr.collision)
        {
            ChangeState(typeof(CS_HitLand));
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.Ctr.freeze = false;
        //HitStunDuration = 0;
    }
}

[System.Serializable]
public class CS_Hitfreeze : CState
{
    [SerializeField]
    public int duration = 5;
    private int timer;

    private void SetDuration(Damage damage)
    {
        duration = (int)damage.damageNumber;
        duration = Mathf.Clamp(duration, 4, 10);

        CS_Hitstun hitstun = (CS_Hitstun)chr.GetState(typeof(CS_Hitstun));
        hitstun.HitStunDuration = CalculateHitstun(damage, chr);
    }

    private int CalculateHitstun(Damage damage, Character chr)
    {
        int baseHitstun = 10;
        return baseHitstun + (int)(damage.damageNumber * (5 - 2 * chr.HealthPercent)); //range 3- 5
    }

    public override void Init(Character chr)
    {
        base.Init(chr);

        chr.ATakesDamage += SetDuration;
    }

    public override void Enter()
    {
        base.Enter();

        chr.Ctr.inControl = false;
        chr.Ctr.freeze = true;


        timer = 0;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        if (timer > duration)
        {
            ChangeState(typeof(CS_Hitstun));
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.Ctr.inControl = false;
        chr.Ctr.freeze = false;

        chr.Ctr.forceMovement = chr.currentKnockback;
    }
}

[System.Serializable]
public class CS_Shield : CState
{
    public override void Enter()
    {
        base.Enter();
        chr.shielding = true;
        //chr.Flash(Color.yellow, 5);
    }

    public override void Execute()
    {
        base.Execute();

        chr.GetInputs();

        //if (chr.DirectionalInput.x != 0)
        if (chr.StrongInputs.x != 0)
            ChangeState(typeof(CS_Roll));

        chr.SetInputs(Vector2.zero);

        if (!chr.HoldShield) ChangeState(typeof(CS_Idle));


        if (chr.Jump) ChangeState(typeof(CS_Jumpsquat));
    }

    public override void Exit()
    {
        base.Exit();
        chr.shielding = false;
    }
}

[System.Serializable]
public class CS_Roll : CState
{
    private float dirX;

    public override void Enter()
    {
        base.Enter();

        dirX = 0;
        if (chr.DirectionalInput.x != 0) dirX = Mathf.Sign(chr.DirectionalInput.x);

        chr.SetInputs(new Vector2(dirX, 0));
    }

    public override void Execute()
    {
        base.Execute();

        chr.SetInputs(new Vector2(dirX * 0.8f, 0));

        if (chr.Ctr.onLedge)
            chr.SetInputs(Vector2.zero);

        if (chr.Anim.animationOver)
            ChangeState(typeof(CS_Idle));
    }

    public override void Exit()
    {
        base.Exit();

        chr.Direction = -dirX;
    }
}

[System.Serializable]
public class CS_HitLand : CState
{
    [SerializeField]
    public int duration = 10;
    private int timer;

    private Vector2 collisionReflect;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        chr.Ctr.freeze = true;
        chr.Ctr.inControl = false;
        collisionReflect = chr.Ctr.collisionReflect;
        chr.Flash(Color.white, 5);
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.GetInputs();

        if (chr.HoldShield)
        {
            chr.RaiseComboOverEvent();

            if (chr.Ctr.lastCollisionAngle <= 45)
            {
                if (Mathf.Abs(chr.DirectionalInput.x) > 0.5f)
                {
                    if (!ChangeState(typeof(CS_Roll)))
                    {
                        ChangeState(typeof(CS_StandUp));
                    }
                }
                else ChangeState(typeof(CS_StandUp));
            }
            else if (chr.Ctr.lastCollisionAngle == 90)
            {
                if (chr.HoldJump)
                {
                    ChangeState(typeof(CS_WalljumpStart));
                }
                else
                {
                    ChangeState(typeof(CS_Jumping));
                }
            }
            else
            {
                ChangeState(typeof(CS_Jumping));
            }

            chr.Ctr.inControl = true;
            collisionReflect = Vector2.zero;
        }

        if (timer > duration)
        {
            if (collisionReflect.magnitude * 60 > 20f)
            {
                chr.Ctr.forceMovement = collisionReflect * 60 * 0.8f; //80% reduction

                ChangeState(typeof(CS_Hitstun));
            }
            else
            {
                if (chr.Ctr.grounded)
                {
                    chr.RaiseComboOverEvent();

                    ChangeState(typeof(CS_HitLanded));
                    chr.Ctr.inControl = true;
                }
                else
                {
                    chr.Ctr.forceMovement = collisionReflect * 60 * 0.8f; //80% reduction

                    ChangeState(typeof(CS_Hitstun));
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        chr.Ctr.freeze = false;
    }
}
[System.Serializable]
public class CS_HitLanded : CState
{
    [SerializeField]
    public int minDuration = 20;

    protected int timer;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        chr.GetInputs();
        Vector2 input = chr.DirectionalInput;

        if (timer > minDuration)
        {
            if (input.y > 0.5f)
            {
                ChangeState(typeof(CS_StandUp));

            }
            else if (Mathf.Abs(input.x) > 0.5f)
            {
                if (!ChangeState(typeof(CS_Roll)))
                    ChangeState(typeof(CS_StandUp));
            }
        }

        chr.SetInputs(Vector2.zero);
    }
}

[System.Serializable]
public class CS_StandUp : CState
{
    public override void Execute()
    {
        base.Execute();

        chr.SetInputs(Vector2.zero);

        if (chr.Anim.animationOver)
        {
            ChangeState(typeof(CS_Idle));
        }
    }
}

[System.Serializable]
public class CS_Dead : CState
{
    public override void Execute()
    {
        base.Execute();

        chr.Dead();
    }
}

[System.Serializable]
public class CS_Die : CState
{
    public override void Execute()
    {
        base.Execute();

        if (chr.Anim.animationOver)
            ChangeState(typeof(CS_Dead));
    }
}

[System.Serializable]
public class CS_ThrowItem : CState
{
    [SerializeField]
    public string aerialAnimationName;
    protected FrameAnimation aerialANimation;

    public override void Init(Character chr)
    {
        base.Init(chr);
        aerialANimation = chr.Anim.GetAnimation(aerialAnimationName);
    }

    public override void Enter()
    {
        base.Enter();

        if (!chr.Ctr.IsGrounded)
        {
            chr.Anim.ChangeAnimation(aerialANimation);
        }
    }

    public override void Execute()
    {
        base.Execute();

        chr.SetInputs(Vector2.zero);

        if (chr.Anim.animationOver)
        {
            chr.GetInputs();

            if (chr.Ctr.IsGrounded)
                ChangeState(typeof(CS_Idle));
            else ChangeState(typeof(CS_Jumping));
        }
    }

    public override void Exit()
    {
        base.Exit();

        if (chr is Player)
        {
            Player player = (Player)chr;

            player.ThrowItem(chr.DirectionalInput);
        }
    }
}

[System.Serializable]
public class CS_AirDodge : CState
{
    [SerializeField]
    public int duration = 25;
    protected int timer = 0;

    protected Vector2 direction;

    public override void Enter()
    {
        base.Enter();

        timer = 0;

        chr.Flash(EffectManager.ColorDefend, duration);

        chr.GetInputs();
        direction = chr.DirectionalInput;
        direction.y *= 0.3f; //otherwise too high up
    }

    public override void Execute()
    {
        base.Execute();

        timer++;

        //chr.Ctr.forceMovement = direction * 15;
        //direction *= 0.9f;

        chr.GetInputs();

        if (timer <= 3) chr.Ctr.forceMovement = -direction * 3f;
        else if (timer <= 8) chr.Ctr.forceMovement = direction * 15f;

        if (timer > duration)
        {
            ChangeState(typeof(CS_Jumping));
        }

        //if (chr.Ctr.IsGrounded) Debug.Log("grounded");

        chr.CS_CheckLanding();
    }
}

[System.Serializable]
public class CS_ShieldHit : CState
{
    public override void Enter()
    {
        base.Enter();

        chr.shielding = true;
    }
    public override void Execute()
    {
        base.Execute();

        chr.CS_CheckIfStillGrounded();

        if (chr.Anim.animationOver)
        {
            ChangeState(typeof(CS_Idle));
        }
    }

    public override void Exit()
    {
        base.Exit();

        chr.shielding = false;
    }
}