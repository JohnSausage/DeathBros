using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public Controller2D Ctr { get; protected set; }

    public override void Init()
    {
        base.Init();

        Ctr = GetComponent<Controller2D>();
    }

    void Update()
    {
        Movement = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

        Ctr.input = Movement;
        if (Input.GetButtonDown("Jump"))
            Ctr.jump = true;

        /*
        if (Input.GetButtonDown("Jump"))
        {
            FrameAnimator anim = GetComponent<FrameAnimator>();
            anim.ChangeAnimation("idle");
        }

        if (Input.GetButtonDown("Fire1"))
        {
            FrameAnimator anim = GetComponent<FrameAnimator>();
            anim.ChangeAnimation("walking");
        }
        */
    }
}
