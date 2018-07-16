using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
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

    }
}
