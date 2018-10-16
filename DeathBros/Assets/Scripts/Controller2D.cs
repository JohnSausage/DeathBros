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
    public bool freeze = false;
    private Vector2 velocityAfterFreeze;

    public Vector2 testKnockback = new Vector2(15, 20);

    public bool IsGrounded { get { return (grounded); } }
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
                velocity.y = jumpVelocity / 60;
            }

            if (addMovement != Vector2.zero)
            {
                velocity += addMovement / 60;
            }


            //check if grounded
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

            /*
            RaycastHit2D ceilingCheck = Physics2D.BoxCast((Vector2)bounds.center + new Vector2(0, bounds.extents.y - skin / 2),
                new Vector2(bounds.size.x, skin), 0, Vector2.up, skin * Mathf.Sqrt(2), collisionMask);

            if (ceilingCheck)
            {
                onCeiling = true;

                velocity.y += gravity / 60;
            }
            */

            if (grounded)
            {
                airTimer = 0;
                velocity.y = 0;
                fastFall = false;

                //check for moving platforms and other transporters
                RaycastHit2D transporterCheck = RCXY(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), transporterMask);

                if (transporterCheck)
                {
                    transform.SetParent(transporterCheck.transform);
                }

                //slide down slope if the angle is too high
                if (slopeDownAngle > maxSlopeAngle && slopeDownAngle < 90)
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
                    if (!slopeUpFound && slopeDownAngle > 0 && slopeDownAngle <= maxSlopeAngle) // !slopeUpFound, because you're not on a slope when both rays hit
                    {
                        velocity = new Vector2(Mathf.Sign(input.x) * Mathf.Cos(Mathf.Deg2Rad * slopeDownAngle), -Mathf.Sin(Mathf.Deg2Rad * slopeDownAngle)).normalized / 60 * movespeed;
                        velocity *= Mathf.Abs(input.x);

                        slopeDown = true;
                    }

                    //slope up
                    if (!slopeDownFound && slopeUpAngle > 0 && slopeUpAngle <= maxSlopeAngle) //!slopeDownFound, because you're not on a slope when both rays hit
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

                        if (newSlopeUpAngle < 90 && slopeUpCast.distance > 0) //ignore the slope if inside of it, or no slope at all
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

                if (velocity.y < 0)
                {
                    velocity.y += gravity / 60 * 0.5f;
                }

                //set jump physics with longer airtime
                if (oldVelocity.y > 0 && velocity.y <= 0) //at jump apex
                {
                    airTimer = airFrames;
                }

                if (airTimer > 0)
                {
                    airTimer--;

                    velocity.y -= gravity / 60 / (2 - airTimer / airFrames);
                }


                //manage fall speeds
                if (fastFall)
                {
                    airTimer = 0;

                    velocity.y += gravity / 60 * 2;

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

            forceMovement = Vector2.zero;

            Debug.Log("forceMovement: " + forceMovement);
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

           
            //manage freezing
            if(freeze)
            {
                if(velocityAfterFreeze == Vector2.zero)
                {
                    velocityAfterFreeze = velocity;
                }

                velocity = velocityAfterFreeze *  0.05f;
            }
            else
            {
                velocity += velocityAfterFreeze;
                velocityAfterFreeze = Vector2.zero;
            }

            //check for collisions
            collisionCheck = RCXY(velocity, velocity.magnitude, groundMask); //check for all collisions and stop velocity

            if (collisionCheck)
            {
                //velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));

                // REMOVE THIS LATER //////////////////////////////////////////////////////////////
                //inControl = true;

                velocity = 0.2f * Vector2.Reflect(velocity, collisionCheck.normal);

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