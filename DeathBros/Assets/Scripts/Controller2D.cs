using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public Vector2 input;
    public float jumpVelocity;
    public bool fallThroughPlatform = false;
    public bool fastFall = false;
    public Vector2 forceMovement;
    public Vector2 addMovement;
    public bool resetVelocity = false;
    public bool allowDI = true;
    public bool inControl = true;

    public Vector2 testKnockback = new Vector2(15, 20);

    public bool IsGrounded { get { return (grounded || oldGrounded); } }
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
    public int wallDirection;
    public bool onCeiling;
    public bool slidingDownSlope;

    public bool slopeDown;
    private bool oldSlopeDown;
    public bool slopeUp;

    public float slopeUpAngle, oldSlopeUpAngle;
    public float slopeDownAngle;

    public bool lastCollisionCheck;

    private bool oldGrounded;

    public BoxCollider2D Col { get; protected set; }
    protected Bounds bounds;
    public float skin = 0.01f;
    public float skinDistance;
    public float triangleDistance;

    protected RaycastHit2D[] platformCasts;

    private void Start()
    {
        Col = GetComponent<BoxCollider2D>();
        platformCasts = new RaycastHit2D[3];
    }

    public void ManualFixedUpdate()
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
        slopeUp = false;
        slidingDownSlope = false;

        oldSlopeUpAngle = slopeUpAngle;
        slopeUpAngle = 0;
        slopeDownAngle = 0;



        bounds = Col.bounds;
        bounds.Expand(-2 * skin);



        //set inputs
        /*
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(input.x) < 0.15f)
        {
            input.x = 0;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpVelocity = jumpForce / 60;
        }

        if (input.y <= -0.75)
        {
            fastFall = true;
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
        */

        if (input.y <= -0.25)
        {
            fallThroughPlatform = true;
        }

        RaycastHit2D collisionCheck;
        RaycastHit2D groundCheck;

        Vector2 groundPoint = Vector2.zero;


        if (forceMovement == Vector2.zero && inControl)
        {
            if (resetVelocity)
            {
                velocity = Vector2.zero;
            }

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

            if (jumpVelocity != 0)
            {
                velocity.y = jumpForce / 60;
            }

            if (addMovement != Vector2.zero)
            {
                velocity += addMovement / 60;
            }

            groundMask = collisionMask + platformMask;

            if (fallThroughPlatform)
            {
                groundMask = collisionMask;
            }



            if (velocity.y <= 0)
            {
                groundCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2),
            new Vector2(bounds.size.x, skin), 0, Vector2.down, skin * 3, collisionMask);

                if (groundCheck)
                {
                    grounded = true;

                    slopeDownAngle = Vector2.Angle(Vector2.up, groundCheck.normal);

                    groundPoint = groundCheck.point - (Vector2)bounds.center;

                    //Debug.Log(slopeDownAngle);
                }


                if (!fallThroughPlatform)
                {
                    platformCasts = new RaycastHit2D[10];

                    RCXY_noAl(Vector2.down, skin * 2, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), platformMask);

                    for (int i = 0; i < platformCasts.Length; i++)
                    {
                        if (platformCasts[i])
                        {
                            if (platformCasts[i].distance > 0) //ignore a platform when inside of the platform
                            {
                                grounded = true;

                                slopeDownAngle = Vector2.Angle(Vector2.up, platformCasts[i].normal);
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


                if (slopeDownAngle > maxSlopeAngle && slopeDownAngle < 90) //slide down slope
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

                    slidingDownSlope = true;
                }

                if (velocity.x != 0)
                {
                    //RaycastHit2D slopeDownCast = RCL(Vector2.down, 2 * skin, (Vector2)bounds.center + new Vector2(-bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);
                    //if (slopeDownCast)
                    //{
                    //    slopeDownAngle = Vector2.Angle(Vector2.up, slopeDownCast.normal);
                    //
                    //    Debug.Log(slopeDownCast.distance);
                    //}

                    //RaycastHit2D slopeUpCast = RCL(Vector2.down, 2 * skin, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);
                    //if (slopeUpCast)
                    //{
                    //    slopeUpAngle = Vector2.Angle(Vector2.up, slopeUpCast.normal);
                    //}

                    //cast a ray down on each side of the collider to determine ground angles
                    bool slopeDownFound = false;
                    platformCasts = new RaycastHit2D[3];

                    Physics2D.RaycastNonAlloc((Vector2)bounds.center + new Vector2(-bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y),
                        Vector2.down, platformCasts, 3 * skin, groundMask);

                    for (int i = 0; i < platformCasts.Length; i++)
                    {
                        if (platformCasts[i])
                        {
                            if (platformCasts[i].distance > 0)
                            {
                                slopeDownAngle = Vector2.Angle(Vector2.up, platformCasts[i].normal);
                                slopeDownFound = true;
                            }
                        }
                    }

                    bool slopeUpFound = false;
                    platformCasts = new RaycastHit2D[3];

                    Physics2D.RaycastNonAlloc((Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y),
                        Vector2.down, platformCasts, 3 * skin, groundMask);

                    for (int i = 0; i < platformCasts.Length; i++)
                    {
                        if (platformCasts[i])
                        {
                            if (platformCasts[i].distance > 0)
                            {
                                slopeUpAngle = Vector2.Angle(Vector2.up, platformCasts[i].normal);
                                slopeUpFound = true;
                            }
                        }
                    }

                    //try slope down first
                    if (!slopeUpFound && slopeDownAngle > 0 && slopeDownAngle <= maxSlopeAngle) // !slopeUpCast, because you're not on a slope when both rays hit
                    {
                        velocity = new Vector2(Mathf.Sign(input.x) * Mathf.Cos(Mathf.Deg2Rad * slopeDownAngle), -Mathf.Sin(Mathf.Deg2Rad * slopeDownAngle)).normalized / 60 * movespeed;
                        velocity *= Mathf.Abs(input.x);

                        slopeDown = true;
                    }

                    //slope up
                    if (!slopeDownFound && slopeUpAngle > 0 && slopeUpAngle <= maxSlopeAngle) //!slopeDownCast, because you're not on a slope when both rays hit
                    {
                        velocity = new Vector2(Mathf.Sign(input.x) * Mathf.Cos(Mathf.Deg2Rad * slopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * slopeUpAngle)).normalized / 60 * movespeed;
                        velocity *= Mathf.Abs(input.x);

                        slopeUp = true;
                    }

                    //check for a new slope up while moving
                    RaycastHit2D slopeUpCast = RCL(velocity, velocity.magnitude * 2, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);
                    
                    if (slopeUpCast)
                    {
                        float newSlopeUpAngle = Vector2.Angle(Vector2.up, slopeUpCast.normal);
                    
                        if (newSlopeUpAngle < 90  && slopeUpCast.distance > 0) //ignore the slope if inside of it, or no slope at all
                        {
                            velocity = Vector2.ClampMagnitude(velocity, HitDistance(slopeUpCast));
                    
                            onWall = true;
                        }
                    
                    
                        //new slope up
                        if (newSlopeUpAngle > 0 && newSlopeUpAngle <= maxSlopeAngle && HitDistance(slopeUpCast) <= triangleDistance)
                        {
                            velocity = new Vector2(Mathf.Sign(input.x) * Mathf.Cos(Mathf.Deg2Rad * newSlopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * newSlopeUpAngle)).normalized / 60 * movespeed;
                            velocity *= Mathf.Abs(input.x);
                    
                            slopeUp = true;
                        }
                    }

                    //collisionCheck when moving
                    collisionCheck = RCXY(velocity, velocity.magnitude, collisionMask);

                    if (collisionCheck)
                    {
                        velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
                    }

                    //check for new slope down
                    RaycastHit2D newSlopeDownCast = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y + skin / 2) + velocity,
                        new Vector2(bounds.size.x, skin), 0, Vector2.down, 1, groundMask);

                    if (newSlopeDownCast)
                    {
                        float newSlopeDownAngle = Vector2.Angle(Vector2.up, newSlopeDownCast.normal);

                        if (newSlopeDownCast.distance > skin && newSlopeDownCast.distance < Mathf.Abs(velocity.x) + skin)
                        {
                            velocity.y += -(newSlopeDownCast.distance - skin);
                        }
                    }
                }

                /*
                if (slopeDownAngle > maxSlopeAngle && slopeDownAngle < 90) //slide down slope
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
                    //Debug.Log("0: " + slopeUpAngle);

                    if (velocity.x != 0)
                    {
                        collisionCheck = RCL(Vector2.down, 2 * skin, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);

                        if (collisionCheck) //not going down slope
                        {
                            slopeUpAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);

                            //Debug.Log("1: " + slopeUpAngle);
                        }

                        collisionCheck = RCL(velocity, velocity.magnitude * 2, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);

                        if (collisionCheck)
                        {
                            if (HitDistance(collisionCheck) <= triangleDistance) //if very close to wall
                            {
                                onWall = true;

                                slopeUpAngle = Vector2.Angle(Vector2.up, collisionCheck.normal); //set new angle if directly on wall

                                //Debug.Log("2: " + slopeUpAngle);
                            }
                        }

                        if (slopeUpAngle <= maxSlopeAngle) //if can move up this slope
                        {
                            velocity = new Vector2(Mathf.Sign(input.x) * Mathf.Cos(Mathf.Deg2Rad * slopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * slopeUpAngle)).normalized / 60 * movespeed;
                            velocity *= Mathf.Abs(input.x);
                        }

                        if (slopeUpAngle <= 90 && collisionCheck.distance > 0) //otherwise gets stuck on pltform when moving up
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
                            if (velocity.x != 0)
                            {
                                collisionCheck = RCL(Vector2.down, 2 * skin, (Vector2)bounds.center + new Vector2(-bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);

                                if (collisionCheck) //not going down slope
                                {
                                    slopeDownAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);

                                    //Debug.Log("1: " + slopeUpAngle);
                                }

                                if(slopeDownAngle <= maxSlopeAngle && slopeDownAngle > 0)
                                {
                                    velocity = new Vector2(Mathf.Sign(input.x) * Mathf.Cos(Mathf.Deg2Rad * slopeUpAngle), -Mathf.Sin(Mathf.Deg2Rad * slopeUpAngle)).normalized / 60 * movespeed;
                                    velocity *= Mathf.Abs(input.x);

                                    collisionCheck = RCXY(velocity, velocity.magnitude, collisionMask);

                                    if (collisionCheck)
                                    {
                                        velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
                                    }
                                }


                            }

                            
                            ////slopedown
                            //collisionCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y + skin / 2) + velocity,
                            //    new Vector2(bounds.size.x, skin), 0, Vector2.down, Mathf.Abs(velocity.x) + skin * 2, groundMask);
                            //
                            //if (collisionCheck)
                            //{
                            //    //if (collisionCheck.distance >= Mathf.Abs(gravity / 60) + skin && velocity.y <= 0)
                            //    if (velocity.y <= 0)
                            //    {
                            //        float angle = Vector2.Angle(Vector2.up, collisionCheck.normal);
                            //
                            //        if (angle <= maxSlopeAngle && angle > 0)
                            //        {
                            //            slopeDown = true;
                            //
                            //            velocity.y = -(collisionCheck.distance - skin);
                            //
                            //            if (oldSlopeDown) // when already on slope -> reduce velocity to normal movespeed
                            //            {
                            //                velocity = Vector2.ClampMagnitude(velocity, Mathf.Abs(input.x) * movespeed / 60);
                            //            }
                            //        }
                            //    }
                            //}
                            
                        }

    
                    }
                }
                */
            }



            if (!grounded)
            {
                if (oldGrounded)
                {
                    //Debug.Log("not grounded anymore");
                }

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

                    velocity.y -= gravity / 60 /  (2 - airTimer/airFrames);
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
                    wallDirection = (int)Mathf.Sign(velocity.x);
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
                        platformCasts = new RaycastHit2D[3];

                        RCXY_noAl(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), platformMask);

                        for (int i = 0; i < platformCasts.Length; i++)
                        {
                            if (platformCasts[i])
                            {
                                if (platformCasts[i].distance > 0)
                                {
                                    float groundAngle = Vector2.Angle(Vector2.up, platformCasts[i].normal);

                                    if (groundAngle <= maxSlopeAngle) //ohterwise gets stuck on walls
                                    {
                                        velocity = Vector2.ClampMagnitude(velocity, HitDistance(platformCasts[i]));
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

                if (velocity.magnitude <= 0.1f)
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

        resetVelocity = false;
        addMovement = Vector2.zero;
        fallThroughPlatform = false;
        jumpVelocity = 0;
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

        Physics2D.BoxCastNonAlloc(center, size, 0, direction, platformCasts, distance + skinDistance, layerMask);
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




/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : MonoBehaviour
{
    public Vector2 input;
    public Vector2 knockback;
    public bool slowDown;
    [Space]
    public Vector2 velocity;
    public float jumpVelocity;
    public bool jump;
    [Space]
    public bool applyGravity = true;
    public bool resetVelocity = false;
    public Vector2 forceMovement = Vector2.zero;
    public Vector2 addMovement = Vector2.zero;
    [Space]
    public float angleX, angleY;
    public float oldAngleX;
    [Space]
    public bool grounded;
    public bool oldGrounded;
    public bool onWall;
    public bool oldOnWall;
    public bool onPlatform;
    public bool oldOnPlatform;
    public bool insidePlatform;
    public int wallDirection;
    [Space]
    public bool fallThroughPlatform;
    public int fallThroughPlatformDuration = 5;
    private int fallThroughPlatformTimer = 0;
    [Space]
    public Vector2 hitDirection;
    public float gamma;
    public float alpha;
    [Space]
    public LayerMask collisionMask;
    public LayerMask platformMask;
    public string movingPlatformTag = "MovingPlatform";
    public BoxCollider2D Col { get; protected set; }
    public PlatformController currentPlatform;
    [Space]
    public float gravity = -1f;
    public float movespeed = 10f;
    public float wallSlideSpeed = 3f;
    //public float accelerationTimeAerial = 0.1f;
    [Space]
    public float maxNormalFallSpeed = -20f;
    public float maxFastFallSpeed = -30f;
    public bool fastFall = false;
    [Space]
    public float maxSlopeDownAngle = 45;
    public float maxSlopeUpAngle = 45;
    [Space]
    public float velocityXSmoothingAerial = 0.5f;
    protected float moveAngle;

    protected Bounds bounds;
    protected Bounds fullBounds;

    protected static float skin = 0.01f;
    protected float diag = Mathf.Sqrt(2) * skin;

    protected Vector2 gizmoVector;
    protected Vector2 gizmoSlopeDown;

    void Start()
    {
        velocity = Vector2.zero;
        Col = GetComponent<BoxCollider2D>();

    }

    private void Update()
    {
        //input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));


    }

    public void ManualFixedUpdate()
    {
        Move();

        transform.Translate(velocity);
    }

    protected RaycastHit2D RCXY(Vector2 direction, float distance, bool includePlatforms = false)
    {
        moveAngle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        gizmoVector = velocity.normalized;
        gizmoVector *= (velocity.magnitude);

        LayerMask collisionMasks = collisionMask;

        if (includePlatforms)
            collisionMasks += platformMask;

        float skinDistance = skin;
        float cos = Mathf.Cos(moveAngle * Mathf.Deg2Rad);
        if (cos > skin && cos != 0)
            skinDistance = skin / cos;

        return Physics2D.BoxCast(bounds.center, bounds.size, 0, direction, distance + skinDistance, collisionMasks);
    }

    protected RaycastHit2D RCXY(bool includePlatforms = false)
    {
        return RCXY(velocity, velocity.magnitude, includePlatforms);
    }

    private void Move()
    {
        oldGrounded = grounded;
        oldAngleX = angleX;
        oldOnWall = onWall;
        oldOnPlatform = onPlatform;

        angleX = angleY = 0;
        //wallDirection = 0;

        grounded = false;
        onWall = false;
        onPlatform = false;
        insidePlatform = false;

        //update bounds
        bounds = fullBounds = Col.bounds;
        bounds.Expand(-2 * skin);

        //set often used variables
        float dirX = Mathf.Sign(input.x);



        //reset moving platform vector
        Vector2 movingPlatform = Vector2.zero;

        if (input.y < 0)
        {
            fallThroughPlatform = true;
            fallThroughPlatformTimer = fallThroughPlatformDuration;
        }

        if (fallThroughPlatform)
        {
            fallThroughPlatformTimer--;
        }

        if (fallThroughPlatformTimer <= 0)
        {
            fallThroughPlatform = false;
        }

        bool jump = false;

        //set velocity
        if (resetVelocity)
        {
            velocity = Vector2.zero;
        }

        if (oldGrounded)
        {
            velocity.x = input.x / 60 * movespeed;

            velocity.y = gravity / 6;
        }
        else
        {
            float targetVelocityX = input.x / 60 * movespeed;

            //float smoothedVelocityX = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothingAerial, accelerationTimeAerial / 60);

            //velocity.x += smoothedVelocityX;
            float oldVel = velocity.x;
            velocity.x -= Mathf.Sign(velocity.x) * velocityXSmoothingAerial / 60;

            if (Mathf.Sign(oldVel) != Mathf.Sign(velocity.x))
            {
                velocity.x = 0;
            }

            velocity.x += targetVelocityX;

            velocity.x = Mathf.Clamp(velocity.x, -movespeed / 60, movespeed / 60);
            velocity.y += gravity / 60;

            if (!fastFall)
            {
                if (velocity.y < maxNormalFallSpeed / 60)
                {
                    velocity.y = maxNormalFallSpeed / 60;
                }
            }
            else if (velocity.y < 0)
            {
                velocity.y = maxFastFallSpeed / 60;
            }
            else
            {
                fastFall = false;
            }
        }


        if (jumpVelocity != 0)
        {
            velocity.y = jumpVelocity / 60;
            jumpVelocity = 0;
            jump = true;
        }

        velocity += addMovement;

        if (forceMovement.x != 0) velocity.x = forceMovement.x;
        if (forceMovement.y != 0) velocity.y = forceMovement.y;

        //check if inside platform
        if (!fallThroughPlatform && !jump)
        {
            RaycastHit2D platformCheck = Physics2D.BoxCast(Col.bounds.center, Col.bounds.size, 0, velocity, velocity.magnitude, platformMask);

            if (platformCheck)
            {
                if (platformCheck.transform.tag == movingPlatformTag)
                {
                    if (currentPlatform == platformCheck.transform.GetComponentInParent<PlatformController>())
                    {
                        movingPlatform = currentPlatform.Movement;
                        velocity = movingPlatform;

                        onPlatform = true;
                        grounded = true;

                        //push upwards if inside of platform
                        if (platformCheck.distance == 0)
                        {
                            velocity.y -= gravity / 30;
                        }
                    }
                }
                else if (platformCheck.distance == 0)
                {
                    //check again for standing platforms
                    platformCheck = Physics2D.BoxCast(bounds.center, bounds.size, 0, new Vector2(velocity.x, 0), Mathf.Abs(velocity.x) + skin, platformMask);

                    if (platformCheck)
                    {
                        onPlatform = false;
                        //grounded = false;
                        //insidePlatform = true;
                        //fallThroughPlatform = true;
                    }
                }
            }
        }




        //CHECK IF GROUNDED//////////////////////////////////////////////////////////
        LayerMask groundMask = collisionMask;


        //only groundcheck when falling down
        if (velocity.y <= 0)
        {
            //check only for platforms as ground when not pressing down
            if (!fallThroughPlatform)
            {
                groundMask += platformMask;
            }

            RaycastHit2D groundCheck;

            moveAngle = Vector2.Angle(velocity, Vector2.right * dirX);

            float skinDistance = skin;
            float cos = Mathf.Cos(moveAngle * Mathf.Deg2Rad);
            if (cos > skin && cos != 0)
                skinDistance = skin / cos;

            groundCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y),
                new Vector2(bounds.size.x, skin), 0, velocity, velocity.magnitude + skinDistance, groundMask);

            float wallAngle = Vector2.Angle(groundCheck.normal, Vector2.up);

            if (groundCheck)// && groundCheck.distance != 0)// && wallAngle != 90) //not grounded if inside the collider (duteance == 0) for example in platforms
            {
                if (wallAngle >= 90) //if on wall check for found only inm downwards direction, otherwise grounded on wall
                {
                    groundCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y),
                new Vector2(bounds.size.x, skin), 0, Vector2.up, velocity.y + -skin, groundMask);

                }

                if (groundCheck)
                {
                    grounded = true;

                    if (groundCheck.collider.gameObject.layer == 9)
                    {
                        onPlatform = true;
                    }

                    //triangle calculations
                    hitDirection = (Vector2)bounds.center - groundCheck.point;
                    hitDirection = new Vector2(Mathf.Sign(hitDirection.x), Mathf.Sign(hitDirection.y));

                    gamma = Mathf.Abs(90 - Vector2.Angle(groundCheck.normal, hitDirection));
                    alpha = Mathf.Abs(90 - Vector2.Angle(groundCheck.normal, velocity));

                    float distance = skin;

                    float sin = Mathf.Sin(alpha * Mathf.Deg2Rad);
                    if (sin != 0)
                        distance = diag * Mathf.Sin(gamma * Mathf.Deg2Rad) / sin;

                    float moveDistance = (groundCheck.distance - distance);

                    //reduce velocity
                    velocity.Normalize();
                    velocity *= (moveDistance);

                    if (velocity.y > 0) //prevent the player from moving up platforms when standing inside of them
                    {
                        velocity.y = 0;
                    }

                    //add platform velocity if on platform
                    if (groundCheck.transform.tag == movingPlatformTag)
                    {
                        currentPlatform = groundCheck.transform.GetComponentInParent<PlatformController>();

                        movingPlatform = currentPlatform.Movement;

                        if (!oldOnPlatform)
                            velocity.y -= movingPlatform.y;

                        velocity += movingPlatform;

                        onPlatform = true;
                    }
                }
            }
        }

        //normalize gravity, set higher before to cope with changing slopeAngles
        if (!grounded && oldGrounded && !jump)
        {
            velocity.y = gravity / 60;
        }

        //if grounded, try again to move on x axis
        if (oldGrounded)
        {
            velocity.x += input.x / 60 * movespeed;
        }

        //SLOPES/////////////////////////////////////////////////////////////////////////

        if (velocity.x != 0 && oldGrounded && !jump)
        {
            LayerMask slopeMask = collisionMask;

            if (!insidePlatform)
            {
                slopeMask += platformMask;
            }

            //ASCEND SLOPES////////////////////////////////////////////////////////////////////////////
            Vector2 raycastXOrigin = (Vector2)bounds.center + new Vector2(bounds.extents.x * dirX, -bounds.extents.y - skin); // added skin here to prevent getting stuck on ending slopes

            RaycastHit2D hitX = Physics2D.Raycast(raycastXOrigin, Vector2.right * dirX, Mathf.Abs(velocity.x) + skin, slopeMask);

            if (hitX)
            {
                float angle = Vector2.Angle(Vector2.up, hitX.normal);

                if (angle < 90)
                    angleX = angle;

                //move up slope only when at the slope; don't check when there is no wall (90)
                if (angleX <= maxSlopeUpAngle && angleX != 90)
                {
                    //always move up  when on platforms, otherwise you can't climb them when running on the ground, since you're insidePlatform
                    if (angleX == oldAngleX || hitX.collider.gameObject.layer == 9)
                    {
                        velocity = new Vector2(dirX * Mathf.Cos(Mathf.Deg2Rad * angleX),
                            Mathf.Sin(Mathf.Deg2Rad * angleX)).normalized / 60 * movespeed * Mathf.Abs(input.x);
                    }

                    hitX = Physics2D.Raycast(raycastXOrigin, velocity, velocity.magnitude + skin, slopeMask);

                    if (hitX)
                    {
                        angle = Vector2.Angle(Vector2.up, hitX.normal);

                        if (angle < 90)
                            angleX = angle;
                    }
                }
            }


            //DESCEND SLOPES////////////////////////////////////////////////////////////////////////////
            gizmoSlopeDown = new Vector2(velocity.x, 0) + (Vector2)bounds.center - new Vector2(bounds.extents.x * dirX, bounds.extents.y);

            RaycastHit2D slopeDown = Physics2D.Raycast(gizmoSlopeDown, Vector2.down, 1, slopeMask);

            if (slopeDown)
            {
                angleY = Vector2.Angle(Vector2.up, slopeDown.normal);

                float angleD = Vector2.Angle(slopeDown.normal, new Vector2(input.x, 0));


                //push player down to slope //&& angleY > 0 removed
                if (velocity.y < 0 && input.x != 0 && slopeDown && angleY <= maxSlopeDownAngle && angleD <= 90)
                {
                    grounded = true;

                    float skinDistance = diag;

                    alpha = angleY;
                    gamma = 90 + Vector2.Angle(slopeDown.normal, new Vector2(dirX, 1));

                    float sin = Mathf.Sin(Mathf.Deg2Rad * alpha);

                    if (sin != 0)
                        skinDistance = diag * (Mathf.Sin(Mathf.Deg2Rad * gamma) / Mathf.Sin(Mathf.Deg2Rad * alpha));

                    velocity.y = -(slopeDown.distance - skinDistance);

                    //normalize the velocity
                    velocity.Normalize();
                    velocity *= Mathf.Abs(input.x) * movespeed / 60;
                }
            }

        }

        //APPLY KNOCKBACK ??????????????????????????????????????????????????????????????
        if (knockback != Vector2.zero)
        {
            //velocity.x += knockback.x / 60;
            //velocity.y = knockback.y / 60;
            velocity = knockback / 60;

            grounded = false;
        }

        knockback = Vector2.zero;

        if (slowDown)
        {
            velocity *= 0.1f;
        }

        //CHECK FOR COLLISIONS//////////////////////////////////////////////////////////
        if (oldOnWall)
        {
            velocity.x = input.x * movespeed / 60;
        }

        RaycastHit2D collisionCheck = RCXY();

        //try to stop collision by just moving on x axis
        if (collisionCheck)
        //if (collisonCheck)
        {
            RaycastHit2D wallDistanceCheck = RCXY(Vector2.right, velocity.x);

            if (wallDistanceCheck)
            {
                velocity.x = dirX * (wallDistanceCheck.distance * skin);
            }

            collisionCheck = RCXY();


            if (!collisionCheck && !grounded)
            {
                onWall = true;
                wallDirection = (int)dirX;

                if (velocity.y <= 0)
                {
                    velocity.y = -wallSlideSpeed / 60;
                }
            }

        }

        if (collisionCheck)
        {
            //triangle calculations
            hitDirection = (Vector2)bounds.center - collisionCheck.point;
            hitDirection = new Vector2(Mathf.Sign(hitDirection.x), Mathf.Sign(hitDirection.y));

            gamma = Mathf.Abs(90 - Vector2.Angle(collisionCheck.normal, hitDirection));
            alpha = Mathf.Abs(90 - Vector2.Angle(collisionCheck.normal, velocity));

            float distance = skin;

            float sin = Mathf.Sin(alpha * Mathf.Deg2Rad);
            if (sin != 0)
                distance = diag * Mathf.Sin(gamma * Mathf.Deg2Rad) / sin;

            float moveDistance = (collisionCheck.distance - distance);

            //reduce velocity
            velocity.Normalize();
            velocity *= (moveDistance);


            //don't stick on roof
            if (!grounded) //!!!!!!!!! causes slipping through ground
            {
                //velocity.y += gravity / 60;
            }


            currentPlatform = null;

        }

        if (!grounded)
        {
            currentPlatform = null;
        }
        else
        {
            fastFall = false;
        }

        forceMovement = Vector2.zero;
        addMovement = Vector2.zero;
        applyGravity = true;
        resetVelocity = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, bounds.size);

        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + (Vector3)gizmoVector, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, bounds.size);

        Gizmos.matrix = Matrix4x4.TRS((Vector3)gizmoSlopeDown, transform.rotation, Vector3.one);
        Gizmos.DrawRay(Vector3.zero, Vector2.down);
    }
}


*/
