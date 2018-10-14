using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public Vector2 input;
    public float setJumpVelocity;
    public bool fallThroughPlatform = false;
    public bool fastFall = false;
    public Vector2 forceMovement;
    public bool allowDI = true;
    public bool inControl = true;

    public Vector2 testKnockback = new Vector2(15, 20);

    [Space]

    public float movespeed = 10;
    public float jumpForce = 15;
    public float gravity = -1;
    public float maxFallSpeed = -15;
    public float fastFallSpeed = -20;
    public float maxAirSpeed = 20;
    public float aerialAcceleration = 0.2f;
    public float aerialDeceleration = 0.05f;

    public int airFrames = 15;
    private int airTimer;
    [Space]

    public float maxSlopeAngle = 45;

    [Space]

    public LayerMask collisionMask;
    public LayerMask platformMask;
    public LayerMask transporterMask;

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
        transform.SetParent(null);

        oldVelocity = velocity;
        oldGrounded = grounded;
        oldSlopeDown = slopeDown;

        grounded = false;
        onCeiling = false;
        onWall = false;
        lastCollisionCheck = false;
        slopeDown = false;

        oldSlopeUpAngle = slopeUpAngle;
        slopeUpAngle = 0;
        slopeDownAngle = 0;

        fallThroughPlatform = false;
        setJumpVelocity = 0;

        bounds = Col.bounds;
        bounds.Expand(-2 * skin);



        //set inputs
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(input.x) < 0.15f)
        {
            input.x = 0;
        }

        if (Input.GetButtonDown("Jump"))
        {
            setJumpVelocity = jumpForce / 60;
        }

        if (input.y <= -0.75)
        {
            fastFall = true;
        }

        if (input.y <= -0.25)
        {
            fallThroughPlatform = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            forceMovement = testKnockback;
            inControl = false;
        }
        else
        {
            forceMovement = Vector2.zero;
        }

        RaycastHit2D collisionCheck;
        RaycastHit2D groundCheck;

        Vector2 groundPoint = Vector2.zero;


        if (forceMovement == Vector2.zero && inControl)
        {
            //mangage velocity before collisions
            if (oldGrounded)
            {
                velocity.y = gravity / 60;
                velocity.x = input.x * movespeed / 60;
            }
            else
            {
                velocity.y += gravity / 60;

                float dirX = Mathf.Sign(velocity.x);
                velocity.x -= movespeed / 60 * aerialDeceleration * Mathf.Sign(velocity.x);

                if (dirX != Mathf.Sign(velocity.x))
                {
                    velocity.x = 0;
                }

                velocity.x += input.x * movespeed / 60 * aerialAcceleration;

                velocity.x = Mathf.Clamp(velocity.x, -movespeed / 60, movespeed / 60);
            }

            if (setJumpVelocity != 0)
            {
                velocity.y = jumpForce / 60;
            }

            groundMask = collisionMask + platformMask;

            if (fallThroughPlatform)
            {
                groundMask = collisionMask;
            }



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

                RaycastHit2D transporterCheck = RCXY(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), transporterMask);

                if (transporterCheck)
                {
                    transform.SetParent(transporterCheck.transform);
                }

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
                        collisionCheck = RCL(Vector2.down, 2 * skin, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);

                        if (collisionCheck) //not going down slope
                        {
                            slopeUpAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);
                        }

                        collisionCheck = RCL(velocity, velocity.magnitude * 2, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);

                        if (collisionCheck)
                        {
                            if (HitDistance(collisionCheck) <= triangleDistance) //if very close to wall
                            {
                                onWall = true;

                                slopeUpAngle = Vector2.Angle(Vector2.up, collisionCheck.normal); //set new angle if directly on wall
                            }
                        }

                        if (slopeUpAngle <= maxSlopeAngle) //if can move up this slope
                        {
                            velocity = new Vector2(Mathf.Sign(input.x) * Mathf.Cos(Mathf.Deg2Rad * slopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * slopeUpAngle)).normalized / 60 * movespeed;
                            velocity *= Mathf.Abs(input.x);
                        }

                        if (slopeUpAngle <= 90) //otherwise gets stuck on pltform when moving up
                        {
                            velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
                        }

                        //checking for collisions while moving up slopes or sideways
                        collisionCheck = RCXY(velocity, velocity.magnitude, collisionMask);

                        if (collisionCheck)
                        {
                            velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
                        }
                        else //don't check for slopedown if already collided with something
                        {
                            //slopedown
                            collisionCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y + skin / 2) + velocity,
                                new Vector2(bounds.size.x, skin), 0, Vector2.down, Mathf.Abs(velocity.x) + skin * 2, groundMask);

                            if (collisionCheck)
                            {
                                if (collisionCheck.distance >= Mathf.Abs(gravity / 60) + skin && velocity.y <= 0)
                                {
                                    float angle = Vector2.Angle(Vector2.up, collisionCheck.normal);

                                    if (angle <= maxSlopeAngle && angle > 0)
                                    {
                                        slopeDown = true;

                                        velocity.y = -(collisionCheck.distance - skin);

                                        if (oldSlopeDown) // when already on slope -> reduce velocity to normal movespeed
                                        {
                                            velocity = Vector2.ClampMagnitude(velocity, Mathf.Abs(input.x) * movespeed / 60);
                                        }
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

                //set jump physics with longer airtime
                if (oldVelocity.y > 0 && velocity.y <= 0) //at jump apex
                {
                    airTimer = airFrames;
                }

                if (airTimer > 0)
                {
                    airTimer--;

                    velocity.y -= gravity / 60 / 2;
                }


                //manage fall speeds
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
                collisionCheck = RCXY(Vector2.right * Mathf.Sign(velocity.x), skin * 2, collisionMask);

                if (collisionCheck)
                {
                    velocity.x = 0;
                    onWall = true;
                }

                collisionCheck = RCXY(velocity, velocity.magnitude, collisionMask); //check for other collisions and stop velocity

                if (collisionCheck)
                {
                    velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
                }

                //check if landing
                if (velocity.y <= 0)
                {
                    //check if landing on moving platforms
                    RaycastHit2D transporterCheck = RCXY(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), transporterMask);

                    if (transporterCheck)
                    {
                        transform.SetParent(transporterCheck.transform);
                    }

                    //reduce velocity when hitting the ground
                    groundCheck = RCXY(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), collisionMask);

                    if (groundCheck)
                    {
                        float groundAngle = Vector2.Angle(Vector2.up, groundCheck.normal);

                        if (groundAngle <= maxSlopeAngle) //ohterwise gets stuck on walls
                        {
                            velocity = Vector2.ClampMagnitude(velocity, HitDistance(groundCheck));
                        }
                    }

                    //reduce velocity when hitting platforms, ignore platforms inside you
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
        }



        //forceMovement
        if (forceMovement != Vector2.zero)
        {
            velocity = forceMovement / 60;

            groundMask = collisionMask;

            if (velocity.y <= 0)
            {
                groundMask += platformMask;
            }

            collisionCheck = RCXY(velocity, velocity.magnitude, groundMask); //check for all collisions and stop velocity

            if (collisionCheck)
            {
                velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
            }
        }

        //lost control -> character just falls down
        if (!inControl)
        {
            velocity.y += gravity / 60;

            groundMask = collisionMask;

            if (velocity.y <= 0)
            {
                groundMask += platformMask;

                if (velocity.y < maxFallSpeed / 60)
                {
                    velocity.y = maxFallSpeed / 60;
                }
            }

            float dirX = Mathf.Sign(velocity.x);
            velocity.x -= movespeed / 60 * aerialDeceleration / 5 * Mathf.Sign(velocity.x); // (/ 5) for less deceleration when not in control for better knockback feeling



            if (dirX != Mathf.Sign(velocity.x))
            {
                velocity.x = 0;
            }

            if (allowDI)
            {
                velocity += movespeed * input / 60 * aerialAcceleration / 5; // (/ 5) like for the deceleration
            }

            velocity.x = Mathf.Clamp(velocity.x, -maxAirSpeed / 60, maxAirSpeed / 60);

            collisionCheck = RCXY(velocity, velocity.magnitude, groundMask); //check for all collisions and stop velocity

            if (collisionCheck)
            {
                //velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));

                // REMOVE THIS LATER //////////////////////////////////////////////////////////////
                //inControl = true;

                velocity = 0.5f * Vector2.Reflect(velocity, collisionCheck.normal);

                if(velocity.magnitude <= 0.1f)
                {
                    inControl = true;
                }
            }
        }

        //last collisioncheck only in case of errors
        velocitybfCol = velocity;

        collisionCheck = Physics2D.BoxCast((Vector2)bounds.center + velocity, bounds.size, 0, Vector2.zero, 0, collisionMask);

        if (collisionCheck)
        {
            velocity = Vector2.zero;
            lastCollisionCheck = true;
        }

        moveAngle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        //actually move the transform
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


