using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticStates
{

   // public static StaticStates Instance { get; protected set; }
   //
   // public void Awake()
   // {
   //     if (Instance == null)
   //     {
   //         Instance = this;
   //     }
   //     else
   //     {
   //         Destroy(gameObject);
   //         return;
   //     }
   //
   //     InitStates();
   // }

    public static SCS_Idle idle = new SCS_Idle();
    public static SCS_Walking walking = new SCS_Walking();
    public static SCS_Jumping jumping = new SCS_Jumping();
    public static SCS_Jumpsquat jumpsquat = new SCS_Jumpsquat();
    public static SCS_Crouch crouch = new SCS_Crouch();
    public static SCS_Landing landing = new SCS_Landing();
    public static SCS_DoubleJumpsquat doubleJumpsquat = new SCS_DoubleJumpsquat();
    public static SCS_Dash dash = new SCS_Dash();
    public static SCS_Skid skid = new SCS_Skid();
    //// called in Init()
    //public static void InitStates()
    //{
    //    idle = new SCS_Idle();
    //    walking = new SCS_Walking();
    //    jumping = new SCS_Jumping();
    //}
}