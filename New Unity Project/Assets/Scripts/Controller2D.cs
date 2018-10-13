using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public Vector2 input;
    public bool fallThroughPlatform = false;
    public bool fastFall = false;

    [Space]

    public float movespeed = 10;
    public float jumpForce = 5;
    public float gravity = -1;
    public float maxFallSpeed = -2;
    public float fastFallSpeed = -4;
    public int airFrames = 30;
    private int airTimer;
    [Space]

    public float maxSlopeAngle = 45;

    [Space]

    public LayerMask collisionMask;
    public LayerMask platformMask;

    private LayerMask groundMask;

    [Space]

    public Vector2 velocity;
    private Vector2 oldVelocity;
    public Vector2 velocitybfCol;

    public float moveAngle;
    public bool grounded;
    public bool onWall;
    public bool onCeiling;

    public bool slopeDown;
    private bool oldSlopeDown;

    public float slopeUpAngle, oldSlopeUpAngle;
    public float slopeDownAngle;

    public bool lastCollisionCheck;

    private bool oldGrounded;
    //private bool oldOnWall;

    public BoxCollider2D Col { get; protected set; }
    protected Bounds bounds;
    public float skin = 0.01f;
    public float skinDistance;
    public float triangleDistance;

    protected RaycastHit2D[] platforms;

    private void Start()
    {
        Col = GetComponent<BoxCollider2D>();
        platforms = new RaycastHit2D[3];
    }

    void FixedUpdate()
    {
        oldVelocity = velocity;
        oldGrounded = grounded;
        oldSlopeDown = slopeDown;
        //oldOnWall = onWall;

        grounded = false;
        onCeiling = false;
        onWall = false;
        lastCollisionCheck = false;
        slopeDown = false;

        //moveAngle = 0;
        oldSlopeUpAngle = slopeUpAngle;
        slopeUpAngle = 0;
        slopeDownAngle = 0;

        fallThroughPlatform = false;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");


        bounds = Col.bounds;
        bounds.Expand(-2 * skin);

        velocity.x = input.x * movespeed / 60;

        if (oldGrounded)
        {
            velocity.y = gravity / 6;
        }
        else
        {
            velocity.y += gravity / 60;
        }


        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce / 60;
        }

        groundMask = collisionMask + platformMask;

        if (input.y < 0)
        {
            fallThroughPlatform = true;
            fastFall = true;


            groundMask = collisionMask;
        }

        RaycastHit2D collisionCheck;
        RaycastHit2D groundCheck;

        Vector2 groundPoint = Vector2.zero;

        if (velocity.y <= 0)
        {
            groundCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2),
        new Vector2(bounds.size.x, skin), 0, Vector2.down, skin * 2, collisionMask);

            if (groundCheck)
            {
                grounded = true;

                slopeDownAngle = Vector2.Angle(Vector2.up, groundCheck.normal);

                groundPoint = groundCheck.point - (Vector2)bounds.center;
            }


            if (!fallThroughPlatform)
            {
                platforms = new RaycastHit2D[3];

                RCXY_noAl(Vector2.down, skin * 2, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), platformMask);

                for (int i = 0; i < platforms.Length; i++)
                {
                    if (platforms[i])
                    {
                        if (platforms[i].distance > 0) //ignore a platform when inside of the platform
                        {
                            grounded = true;

                            slopeDownAngle = Vector2.Angle(Vector2.up, platforms[i].normal);
                        }
                    }
                }
            }

        }

        RaycastHit2D ceilingCheck = Physics2D.BoxCast((Vector2)bounds.center + new Vector2(0, bounds.extents.y - skin / 2),
            new Vector2(bounds.size.x, skin), 0, Vector2.up, skin * Mathf.Sqrt(2), collisionMask);

        if (ceilingCheck)
        {
            onCeiling = true;

            velocity.y += gravity / 60;
        }

        if (grounded)
        {
            airTimer = 0;
            velocity.y = 0;
            fastFall = false;

            if (slopeDownAngle > maxSlopeAngle) //slide down slope
            {
                float moveDirX = -Mathf.Sign(groundPoint.x);

                float inputDir = 0;
                if (input.x > 0) inputDir = Mathf.Sign(input.x);

                if (moveDirX != inputDir)
                {
                    velocity = new Vector2(moveDirX * Mathf.Cos(Mathf.Deg2Rad * slopeDownAngle), -Mathf.Sin(Mathf.Deg2Rad * slopeDownAngle)).normalized / 60 * movespeed;
                }

                collisionCheck = RCXY(velocity, velocity.magnitude, groundMask);

                if (collisionCheck)
                {
                    velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
                }
            }
            else // try movement on ground
            {
                if (velocity.x != 0)
                {
                    //2* velocity to check for slopes that are far away
                    collisionCheck = RCL(velocity, velocity.magnitude * 2, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);

                    if (collisionCheck)
                    {
                        slopeUpAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);

                        if (slopeUpAngle <= 90)
                        {
                            velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
                        }
                    }

                    if (HitDistance(collisionCheck) <= triangleDistance) //if very close to wall
                    {
                        onWall = true;
                    }
                    else // this is to stop getting stop at the end of slopes, there the angle can sometimes not be found
                    {
                        collisionCheck = RCL(Vector2.down, 2 * skin, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);

                        slopeUpAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);
                    }

                    if (onWall && slopeUpAngle <= maxSlopeAngle && slopeUpAngle > 0) //if can move up this slope, bigger 0 to not repeat the same code on even ground
                    {
                        velocity = new Vector2(Mathf.Sign(input.x) * Mathf.Cos(Mathf.Deg2Rad * slopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * slopeUpAngle)).normalized / 60 * movespeed;

                        collisionCheck = RCL(velocity, velocity.magnitude * 2, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), collisionMask);

                        if (collisionCheck)
                        {
                            velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
                        }
                    }




                    //slopedown
                    collisionCheck = Physics2D.BoxCast((Vector2)bounds.center + velocity, bounds.size, 0, Vector2.down, Mathf.Abs(velocity.x) + skin * 2, groundMask);

                    if (collisionCheck)
                    {
                        if (collisionCheck.distance >= Mathf.Abs(gravity / 60) + skin && velocity.y <= 0)
                        {
                            float angle = Vector2.Angle(Vector2.up, collisionCheck.normal);

                            if (angle <= maxSlopeAngle && angle > 0)
                            {
                                slopeDown = true;

                                velocity.y = -(collisionCheck.distance - skin);
                                //velocity = Vector2.ClampMagnitude(velocity, movespeed / 60); //then player gets stuck on top of slopes sometimes

                                if (oldSlopeDown)
                                {
                                    velocity = Vector2.ClampMagnitude(velocity, movespeed / 60);
                                }
                            }
                        }
                    }
                }
            }
        }



        if (!grounded)
        {
            if (velocity.y >= 0)
            {
                fastFall = false;
            }

            if (oldVelocity.y > 0 && velocity.y <= 0) //at jump apex
            {
                airTimer = airFrames;
            }

            if (airTimer > 0)
            {
                airTimer--;

                velocity.y -= gravity / 60 / 2;
            }


            if (fastFall)
            {
                airTimer = 0;

                velocity.y += gravity / 60;

                if (velocity.y < fastFallSpeed / 60)
                {
                    velocity.y = fastFallSpeed / 60;

                }
            }
            else
            {
                if (velocity.y < maxFallSpeed / 60)
                {
                    velocity.y = maxFallSpeed / 60;
                }
            }

            //only check walls and not platforms
            collisionCheck = RCXY(Vector2.right, velocity.x, collisionMask); //check for walls

            if (collisionCheck)
            {
                velocity.x = Mathf.Sign(input.x) * (collisionCheck.distance - skin); //reduce velocity.x, so that player doesn't get stuck on walls

                if (Mathf.Abs(velocity.x) <= skin)
                {
                    velocity.x = 0;
                    onWall = true;
                }
            }

            collisionCheck = RCXY(velocity, velocity.magnitude, collisionMask); //check for other collisions and stop velocity

            if (collisionCheck)
            {
                velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
            }

            //check if landing

            if (velocity.y <= 0)
            {
                groundCheck = RCXY(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), collisionMask);

                if (groundCheck)
                {
                    float groundAngle = Vector2.Angle(Vector2.up, groundCheck.normal);

                    if (groundAngle <= maxSlopeAngle) //ohterwise gets stuck on walls
                    {
                        velocity = Vector2.ClampMagnitude(velocity, HitDistance(groundCheck));
                    }
                }


                if (!fallThroughPlatform)
                {
                    platforms = new RaycastHit2D[3];

                    RCXY_noAl(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), platformMask);

                    for (int i = 0; i < platforms.Length; i++)
                    {
                        if (platforms[i])
                        {
                            if (platforms[i].distance > 0)
                            {
                                float groundAngle = Vector2.Angle(Vector2.up, platforms[i].normal);

                                if (groundAngle <= maxSlopeAngle) //ohterwise gets stuck on walls
                                {
                                    velocity = Vector2.ClampMagnitude(velocity, HitDistance(platforms[i]));
                                }
                            }
                        }
                    }
                }


            }
        }

        //last collisioncheck

        velocitybfCol = velocity;

        collisionCheck = Physics2D.BoxCast((Vector2)bounds.center + velocity, bounds.size, 0, Vector2.zero, 0, collisionMask);

        if (collisionCheck)
        {
            velocity = Vector2.zero;
            lastCollisionCheck = true;
        }

        moveAngle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));


        transform.Translate(velocity);
    }


    protected RaycastHit2D RCXY(Vector2 direction, float distance, LayerMask layerMask)
    {
        float angle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        skinDistance = skin;

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        if (cos > skin) skinDistance = skin / cos;

        return Physics2D.BoxCast(bounds.center, bounds.size, 0, direction, distance + skinDistance, layerMask);
    }

    protected RaycastHit2D RCXY(Vector2 direction, float distance, Vector2 center, Vector2 size, LayerMask layerMask)
    {
        float angle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        skinDistance = skin;

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        if (cos > skin) skinDistance = skin / cos;

        return Physics2D.BoxCast(center, size, 0, direction, distance + skinDistance, layerMask);
    }

    protected void RCXY_noAl(Vector2 direction, float distance, Vector2 center, Vector2 size, LayerMask layerMask)
    {
        float angle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        skinDistance = skin;

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        if (cos > skin)
            skinDistance = skin / cos;

        Physics2D.BoxCastNonAlloc(center, size, 0, direction, platforms, distance + skinDistance, layerMask);
    }

    protected RaycastHit2D RCL(Vector2 direction, float distance, Vector2 center, LayerMask layerMask)
    {
        float angle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        skinDistance = skin;

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        if (cos > skin)
            skinDistance = skin / cos;

        return Physics2D.Raycast(center, direction, distance + skinDistance, layerMask);
    }

    protected RaycastHit2D RCXY_ns(Vector2 direction, float distance)
    {
        return Physics2D.BoxCast(Col.bounds.center, Col.bounds.size, 0, direction, distance, groundMask);
    }

    protected float HitDistance(RaycastHit2D collision)
    {
        Vector2 hitDirection;

        hitDirection = (Vector2)bounds.center - collision.point;
        hitDirection = new Vector2(Mathf.Sign(hitDirection.x), Mathf.Sign(hitDirection.y));

        float gamma = Mathf.Abs(90 - Vector2.Angle(collision.normal, hitDirection));
        float alpha = Mathf.Abs(90 - Vector2.Angle(collision.normal, velocity));

        triangleDistance = skin;

        triangleDistance = skin * Mathf.Sqrt(2) * Mathf.Sin(gamma * Mathf.Deg2Rad) / Mathf.Sin(alpha * Mathf.Deg2Rad);

        float moveDistance = (collision.distance - triangleDistance);

        return moveDistance;
    }
}


