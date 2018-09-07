using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public override void Init()
    {
        base.Init();
    }

    void Update()
    {
        DirectionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));
        if (Mathf.Abs(DirectionalInput.x) < 0.2f) DirectionalInput = new Vector2(0, DirectionalInput.y);

        if (Input.GetButtonDown("Jump"))
        {
            Jump = true;
        }
        else
        {
            Jump = false;
        }
    }
}
