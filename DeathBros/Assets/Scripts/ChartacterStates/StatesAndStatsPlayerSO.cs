//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//[CreateAssetMenu(menuName = "StatesAndStatsPlayer")]
//public class StatesAndStatsPlayerSO : StatesAndStatsSO
//{
//    [Space]
//    public string doublejumpsquat_anim = "jumpsquat";
//    public int doublejumpsquat_duration = 3;
//    [Space]
//    public string wallslidingDown_anim = "idle";
//    public string wallslidingUp_anim = "idle";
//    [Space]
//    public string walljumpstart_anim = "idle";
//    public float wallJumpHeightReductionFactor = 0.75f;
//    public int walljumpstart_duration = 3;
//    [Space]
//    public string walljumping_anim = "idle";
//    public int walljumping_duration = 20;
//    [Space]
//    public string skid_anim = "idle";
//    public int skid_duration = 6;
//    public int skid_idleOutDuration = 3;
//    [Space]
//    public string dash_anim = "idle";
//    public int dash_duration = 10;
//    [Space]
//    public string crouch_anim = "idle";
//    [Space]
//    public string roll_anim = "idle";
//    [Space]
//    public string throwitem_anim = "idle";
//    public string throwitemaerial_anim = "idle";
//    [Space]
//    public string airdodge_anim = "idle";
//    public int airdodge_duration = 25;
//    [Space]
//    public string shieldhit_anim = "idle";
//    [Space]
//    public string grab_anim = "grab";

//    public override void InitStates(Character chr)
//    {
//        base.InitStates(chr);

//        CS_DoubleJumpsquat doubleJumpsquat = new CS_DoubleJumpsquat();
//        doubleJumpsquat.animationName = doublejumpsquat_anim;
//        doubleJumpsquat.duration = doublejumpsquat_duration;
//        doubleJumpsquat.Init(chr);

//        CS_Wallsliding wallsliding = new CS_Wallsliding();
//        wallsliding.animationName = wallslidingDown_anim;
//        wallsliding.wallUpAnimation = wallslidingUp_anim;
//        wallsliding.Init(chr);

//        CS_WalljumpStart walljumpStart = new CS_WalljumpStart();
//        walljumpStart.animationName = walljumpstart_anim;
//        walljumpStart.jumpHeightReductionFactor = wallJumpHeightReductionFactor;
//        walljumpStart.duration = walljumpstart_duration;
//        walljumpStart.Init(chr);

//        CS_Walljumping walljumping = new CS_Walljumping();
//        walljumping.animationName = walljumping_anim;
//        walljumping.duration = walljumping_duration;
//        walljumping.Init(chr);

//        CS_Skid skid = new CS_Skid();
//        skid.animationName = skid_anim;
//        skid.duration = skid_duration;
//        skid.idleOutDuration = skid_idleOutDuration;
//        skid.Init(chr);

//        CS_Dash dash = new CS_Dash();
//        dash.animationName = dash_anim;
//        dash.duration = dash_duration;
//        dash.Init(chr);

//        CS_Crouch crouch = new CS_Crouch();
//        crouch.animationName = crouch_anim;
//        crouch.Init(chr);

//        CS_Roll roll = new CS_Roll();
//        roll.animationName = roll_anim;
//        roll.Init(chr);

//        CS_ThrowItem throwItem = new CS_ThrowItem();
//        throwItem.animationName = throwitem_anim;
//        throwItem.aerialAnimationName = throwitemaerial_anim;
//        throwItem.Init(chr);

//        CS_AirDodge airDodge = new CS_AirDodge();
//        airDodge.animationName = airdodge_anim;
//        airDodge.duration = airdodge_duration;
//        airDodge.Init(chr);

//        CS_ShieldHit shieldHit = new CS_ShieldHit();
//        shieldHit.animationName = shieldhit_anim;
//        shieldHit.Init(chr);

//        CS_Grab grab = new CS_Grab();
//        grab.animationName = grab_anim;
//        grab.Init(chr);
//    }
//}
