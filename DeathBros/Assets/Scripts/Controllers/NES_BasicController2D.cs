using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Controller that works for NES Project */
/* Only certain slopes are supported */

[RequireComponent(typeof(BoxCollider2D))]
public class NES_BasicController2D : MonoBehaviour
{
    /* local variables */
    protected Vector2 velocity;
    public Vector2 Velocity { get { return velocity; } }

    /* static variables */
    protected static float skin = 1f/16f; /* equals 1/pixelsPerUnit */

    /* Masks used for collisions */
    [SerializeField]
    protected LayerMask collisionMask;

    [SerializeField]
    protected LayerMask platformMask;

    [SerializeField]
    protected LayerMask transporterMask;

    /* initialization */
    protected void Start()
    {
        velocity = Vector2.zero;
    }

    protected void ApplyGravity(float gravity)
    {
        velocity.y += gravity;
    }

    protected bool CheckIfGrounded()
    {
        bool returnValue = false;



        return returnValue;
    }
    /* Used to move the object in the objects FixedUpdate cycle */
    public virtual void FixedMove(float gravity = 0f)
    {
        ApplyGravity(gravity);


        transform.Translate(velocity);
    }


}
