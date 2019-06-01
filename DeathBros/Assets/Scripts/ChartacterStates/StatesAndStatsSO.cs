using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatesAndStats")]
public class StatesAndStatsSO : ScriptableObject
{
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

     

    }
}

