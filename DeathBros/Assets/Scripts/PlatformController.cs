using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Vector2 moveTo;
    public float speed;

    private Vector2 start, end;
    private bool forward;

    void Start()
    {
        start = transform.position;
        end = start + moveTo;

        forward = true;
    }

    void FixedUpdate()
    {
        if (forward)
        {
            transform.Translate(moveTo.normalized * speed / 60);

            if(((Vector2)transform.position - end).sqrMagnitude < 0.1f)
            {
                forward = false;
            }
        }
        else
        {
            transform.Translate(-moveTo.normalized * speed / 60);

            if (((Vector2)transform.position - start).sqrMagnitude < 0.1f)
            {
                forward = true;
            }
        }
    }
}
