using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatesAndStats")]
public class StatesAndStatsSO : ScriptableObject
{
    public EStateType stateTypes = EStateType.Standard;
    [Space]
    [Space]
    public string idle_anim = "idle";
    [Space]
    public string walking_anim = "walking";
    [Space]
    public string running_anim = "running";
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

        chr.CSMachine.ChangeState(idle);
    }
}