using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ItemController2D : MonoBehaviour
{
    public bool grounded;
    public bool rolling;
    public Vector2 velocity;
    public LayerMask groundMask;
    public float gravity = -1;
    public float reflectVelocity = 0.5f;
    public float stopToReflectVelocity = 10;
    public float minRollAngle = 15;
    public float rollSlowDown = 0.5f;

    public float groundAngle;
    private float rollDirection;
    private float skin = 0.01f;

    private CircleCollider2D col;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        groundAngle = 0;
        rollDirection = 0;

        grounded = false;
        rolling = false;

        float magnitude = velocity.magnitude;

        velocity.y += gravity / 60;


        RaycastHit2D collisionCheck = Physics2D.CircleCast((Vector2)transform.position + col.offset, col.radius - skin, velocity, velocity.magnitude + skin, groundMask);

        if (collisionCheck)
        {

            if (velocity.magnitude * 60 <= stopToReflectVelocity)
            {
                velocity = Vector2.ClampMagnitude(velocity, collisionCheck.distance - skin);

                RaycastHit2D groundCheck = Physics2D.CircleCast((Vector2)transform.position + col.offset, col.radius - skin, collisionCheck.normal, -skin, groundMask);

                if (groundCheck)
                {
                    groundAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);
                    grounded = true;

                    rollDirection = Mathf.Sign(groundCheck.normal.x);
                }
            }
            else
            {
                velocity = Vector2.Reflect(velocity, collisionCheck.normal) * reflectVelocity;
            }
        }

        if (grounded)
        {
            if (groundAngle >= minRollAngle)
            {
                magnitude -= gravity / 60;
                magnitude *= rollSlowDown;


                velocity = new Vector2(rollDirection * Mathf.Cos(groundAngle * Mathf.Deg2Rad), -Mathf.Sin(groundAngle * Mathf.Deg2Rad)) * magnitude;
                rolling = true;

                collisionCheck = Physics2D.CircleCast((Vector2)transform.position + col.offset, col.radius - skin, velocity, velocity.magnitude + skin, groundMask);

                if (collisionCheck)
                {
                    velocity = Vector2.ClampMagnitude(velocity, collisionCheck.distance - skin);
                }
            }
        }

        transform.Translate(velocity);
    }
}
