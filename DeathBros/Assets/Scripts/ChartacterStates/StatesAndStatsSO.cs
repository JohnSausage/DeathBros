using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatesAndStats")]
public class StatesAndStatsSO : ScriptableObject
{
    public string idle_anim = "idle";
    [Space]
    public string walking_anim = "walk";
    [Space]
    public string running_anim = "run";
    [Space]
    public string jumpsquat_anim = "jumpsquat";
    public int jumpsquat_duration = 4;
    [Space]
    public string jumpUp_anim = "idle";
    public string jumpDown_anim = "idle";
    [Space]
    public string landing_anim = "idle";
    public int landing_duration = 4;
    [Space]
    public string hitstunUp_anim = "idle";
    public string hitstunDown_anim = "idle";
    [Space]
    public string hitfreeze_anim = "idle";
    public int hitfreeze_duration = 5;
    [Space]
    public string hitland_anim = "idle";
    public int hitland_duration = 10;
    [Space]
    public string shield_anim = "shield";
    [Space]
    public string die_anim = "idle";
    [Space]
    public string dead_anim = "idle";
    [Space]
    public string standup_anim = "idle";
    [Space]
    public string hitlanded_anim = "idle";
    public int hitLanded_minDuration = 20;
    [Space]
    public string getGrabbed_anim = "idle";

    [Space]
    [Space]

    [Space]
    public string doublejumpsquat_anim = "jumpsquat";
    public int doublejumpsquat_duration = 3;
    [Space]
    public string wallslidingDown_anim = "idle";
    public string wallslidingUp_anim = "idle";
    [Space]
    public string walljumpstart_anim = "idle";
    public float wallJumpHeightReductionFactor = 0.75f;
    public int walljumpstart_duration = 3;
    [Space]
    public string walljumping_anim = "idle";
    public int walljumping_duration = 20;
    [Space]
    public string skid_anim = "idle";
    public int skid_duration = 6;
    public int skid_idleOutDuration = 3;
    [Space]
    public string dash_anim = "idle";
    public int dash_duration = 10;
    [Space]
    public string crouch_anim = "idle";
    [Space]
    public string roll_anim = "idle";
    [Space]
    public string throwitem_anim = "idle";
    public string throwitemaerial_anim = "idle";
    [Space]
    public string airdodge_anim = "idle";
    public int airdodge_duration = 25;
    [Space]
    public string shieldhit_anim = "idle";
    [Space]
    public string grab_anim = "grab";


    public virtual void InitStates(Character chr)
    {
        CS_Idle idle = new CS_Idle();
        idle.animationName = idle_anim;
        idle.Init(chr);

        CS_Walking walking = new CS_Walking();
        walking.animationName = walking_anim;
        walking.Init(chr);

        CS_Jumpsquat jumpsquat = new CS_Jumpsquat();
        jumpsquat.animationName = jumpsquat_anim;
        jumpsquat.duration = jumpsquat_duration;
        jumpsquat.Init(chr);

        CS_Jumping jumping = new CS_Jumping();
        jumping.animationName = jumpDown_anim;
        jumping.jumpRisingAnimation = jumpUp_anim;
        jumping.Init(chr);

        CS_Landing landing = new CS_Landing();
        landing.animationName = landing_anim;
        landing.duration = landing_duration;
        landing.Init(chr);

        CS_Hitstun hitstun = new CS_Hitstun();
        hitstun.animationName = hitstunUp_anim;
        hitstun.hitstunFallAnimation = hitstunDown_anim;
        hitstun.Init(chr);

        CS_Hitfreeze hitfreeze = new CS_Hitfreeze();
        hitfreeze.animationName = hitfreeze_anim;
        hitfreeze.duration = hitfreeze_duration;
        hitfreeze.Init(chr);

        CS_HitLand hitLand = new CS_HitLand();
        hitLand.animationName = hitland_anim;
        hitLand.duration = hitland_duration;
        hitLand.Init(chr);

        CS_Shield shield = new CS_Shield();
        shield.animationName = shield_anim;
        shield.Init(chr);

        CS_Die die = new CS_Die();
        die.animationName = die_anim;
        die.Init(chr);

        CS_Dead dead = new CS_Dead();
        dead.animationName = dead_anim;
        dead.Init(chr);

        CS_StandUp standUp = new CS_StandUp();
        standUp.animationName = standup_anim;
        standUp.Init(chr);

        CS_HitLanded hitLanded = new CS_HitLanded();
        hitLanded.animationName = hitlanded_anim;
        hitLanded.minDuration = hitLanded_minDuration;
        hitLanded.Init(chr);

        CS_GetGrabbed getGrabbed= new CS_GetGrabbed();
        getGrabbed.animationName = getGrabbed_anim;
        getGrabbed.Init(chr);

        CS_DoubleJumpsquat doubleJumpsquat = new CS_DoubleJumpsquat();
        doubleJumpsquat.animationName = doublejumpsquat_anim;
        doubleJumpsquat.duration = doublejumpsquat_duration;
        doubleJumpsquat.Init(chr);

        CS_Wallsliding wallsliding = new CS_Wallsliding();
        wallsliding.animationName = wallslidingDown_anim;
        wallsliding.wallUpAnimation = wallslidingUp_anim;
        wallsliding.Init(chr);

        CS_WalljumpStart walljumpStart = new CS_WalljumpStart();
        walljumpStart.animationName = walljumpstart_anim;
        walljumpStart.jumpHeightReductionFactor = wallJumpHeightReductionFactor;
        walljumpStart.duration = walljumpstart_duration;
        walljumpStart.Init(chr);

        CS_Walljumping walljumping = new CS_Walljumping();
        walljumping.animationName = walljumping_anim;
        walljumping.duration = walljumping_duration;
        walljumping.Init(chr);

        CS_Skid skid = new CS_Skid();
        skid.animationName = skid_anim;
        skid.duration = skid_duration;
        skid.idleOutDuration = skid_idleOutDuration;
        skid.Init(chr);

        CS_Dash dash = new CS_Dash();
        dash.animationName = dash_anim;
        dash.duration = dash_duration;
        dash.Init(chr);

        CS_Crouch crouch = new CS_Crouch();
        crouch.animationName = crouch_anim;
        crouch.Init(chr);

        CS_Roll roll = new CS_Roll();
        roll.animationName = roll_anim;
        roll.Init(chr);

        CS_ThrowItem throwItem = new CS_ThrowItem();
        throwItem.animationName = throwitem_anim;
        throwItem.aerialAnimationName = throwitemaerial_anim;
        throwItem.Init(chr);

        CS_AirDodge airDodge = new CS_AirDodge();
        airDodge.animationName = airdodge_anim;
        airDodge.duration = airdodge_duration;
        airDodge.Init(chr);

        CS_ShieldHit shieldHit = new CS_ShieldHit();
        shieldHit.animationName = shieldhit_anim;
        shieldHit.Init(chr);

        CS_Grab grab = new CS_Grab();
        grab.animationName = grab_anim;
        grab.Init(chr);

        chr.CSMachine.ChangeState(idle);
    }
}