using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NES_Character : MonoBehaviour, ICanBeTransported
{
    public Vector2 ForeceMovement;
    /* Inputs */
    public Vector2 DirectionalInput { get; protected set; }
    public Vector2 StrongInputs { get; protected set; }
    public virtual void ClearStrongInputs() { StrongInputs = Vector2.zero; }
    public Vector2 TiltInput { get; protected set; }

    public bool Jump { get; set; }
    public bool HoldJump { get; protected set; }
    public bool Attack { get; set; }
    public bool HoldAttack { get; set; }
    public bool Special { get; set; }
    public bool HoldSpecial { get; protected set; }
    public bool Shield { get; set; }
    public bool HoldShield { get; protected set; }
    public bool Grab { get; set; }

    public bool allowTransporting { get; set; }

    protected NES_BasicController2D ctr;

    protected void Start()
    {
        InitCtr();

        allowTransporting = false;
    }

    protected void Update()
    {
        GetInputs();
        MapInputsToCtr();
    }

    protected void FixedUpdate()
    {
        ctr.FixedMove();

        allowTransporting = ctr.IsGrounded;
    }

    protected void InitCtr()
    {
        ctr = GetComponent<NES_BasicController2D>();

        ctr.Gravity = -1f;
    }

    protected virtual void GetInputs()
    {

    }

    protected virtual void MapInputsToCtr()
    {
        ctr.DirectionalInput = DirectionalInput;

        if(Attack)
        {
            ctr.FreezeCtr(10);
            ctr.ForceMovement = ForeceMovement / 60;
            ctr.Tumble = true;
        }

        if(HoldShield)
        {
            ctr.FreezeCtr(2);
            ctr.ForceMovement = ForeceMovement / 60;
            ctr.Tumble = true;
        }

        if(Jump)
        {
            ctr.JumpVelocity = 12;
        }

        if(StrongInputs.y < -0.75f)
        {
            ctr.FallThroughPlatforms = true;
            ctr.fastFall = true;
        }
        else
        {
            ctr.FallThroughPlatforms = false;
        }
    }
}

public interface ICanBeTransported
{
    bool allowTransporting { get; set; }
}