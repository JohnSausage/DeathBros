using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon_MoveUpAndDown : MonoBehaviour
{
    protected int timer = 0;
    protected bool moveUp = false;
    private void FixedUpdate()
    {
        timer++;

        bool moveTick = false;

        //move every 10 frames
        if (timer % 10 == 0)
        {
            moveTick = true;
        }

        //move up or down
        if (moveTick == true)
        {
            //move one pixel
            if (moveUp == true)
            {
                transform.Translate(Vector2.up * 1f / 16f);
            }
            else
            {
                transform.Translate(Vector2.down * 1f / 16f);
            }
        }

        //toggle moveUp
        if (timer >= 60)
        {
            moveUp = !moveUp;
            timer = 0;
        }
    }
}
