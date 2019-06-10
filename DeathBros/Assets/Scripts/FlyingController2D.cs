//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(BoxCollider2D))]
//public class FlyingController2D : Controller2D
//{
//    Vector2 groundPoint = Vector2.zero;

//    RaycastHit2D collisionCheck;
//    RaycastHit2D groundCheck;

//    protected override void Move()
//    {


//        if (forceMovement == Vector2.zero && inControl)
//        {
//            if (resetVelocity)
//            {
//                velocity = Vector2.zero;
//            }

//            //mangage velocity before collisions
//            if (oldGrounded)
//            {
//                //velocity.y = gravity / 60;
//                velocity.x = input.x * movespeed / 60;
//                velocity.y = input.y * movespeed / 60;
//            }
//            else
//            {
//                //velocity.y += gravity / 60;

//                float dirX = Mathf.Sign(velocity.x);
//                velocity.x -= movespeed / 60 * aerialDeceleration * Mathf.Sign(velocity.x);
//                velocity.y -= movespeed / 60 * aerialDeceleration * Mathf.Sign(velocity.y);

//                if (dirX != Mathf.Sign(velocity.x))
//                {
//                    velocity.x = 0;
//                }

//                velocity.x += input.x * movespeed / 60 * aerialAcceleration;

//                velocity.x = Mathf.Clamp(velocity.x, -movespeed / 60, movespeed / 60);

//                velocity.y += input.y * movespeed / 60 * aerialAcceleration;

//                velocity.y = Mathf.Clamp(velocity.y, -movespeed / 60, movespeed / 60);
//            }

//            if (jumpVelocity != 0)
//            {
//                velocity.y = jumpVelocity / 60;
//            }

//            if (addMovement != Vector2.zero)
//            {
//                velocity += addMovement / 60;
//            }

//            //check if grounded
//            groundMask = collisionMask + platformMask;

//            if (fallThroughPlatform)
//            {
//                groundMask = collisionMask;
//            }


//            if (velocity.y <= 0)
//            {
//                groundCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2),
//            new Vector2(bounds.size.x, skin), 0, Vector2.down, skin * 3, collisionMask);

//                if (groundCheck)
//                {
//                    grounded = true;

//                    slopeDownAngle = Vector2.Angle(Vector2.up, groundCheck.normal);

//                    groundPoint = groundCheck.point - (Vector2)bounds.center;
//                }


//                if (!fallThroughPlatform)
//                {
//                    platformCasts = new RaycastHit2D[10];

//                    RCXY_noAl(Vector2.down, skin * 2, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), platformMask);

//                    for (int i = 0; i < platformCasts.Length; i++)
//                    {
//                        if (platformCasts[i])
//                        {
//                            if (platformCasts[i].distance > 0) //ignore a platform when inside of the platform
//                            {
//                                grounded = true;

//                                slopeDownAngle = Vector2.Angle(Vector2.up, platformCasts[i].normal);
//                            }
//                        }
//                    }
//                }
//            }


//            if (input.x != 0) faceDirection = Mathf.Sign(input.x);

//            if (grounded)
//            {
//                airTimer = 0;
//                velocity.y = 0;
//                fastFall = false;


//                //check for moving platforms and other transporters
//                RaycastHit2D transporterCheck = RCXY(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), transporterMask);

//                if (transporterCheck)
//                {
//                    transform.SetParent(transporterCheck.transform);
//                }

//                //check if on ledge
//                RaycastHit2D ledgeCheck = RCL(Vector2.down, 1f, (Vector2)bounds.center + new Vector2(bounds.extents.x * faceDirection, -bounds.extents.y), groundMask);

//                if (!ledgeCheck)
//                {
//                    onLedge = true;
//                }

//                //slide down slope if the angle is too high
//                if (slopeDownAngle > maxSlopeAngle && slopeDownAngle < 90)
//                {
//                    float moveDirX = -Mathf.Sign(groundPoint.x);

//                    float inputDir = 0;
//                    if (input.x > 0) inputDir = Mathf.Sign(input.x);

//                    if (moveDirX != inputDir)
//                    {
//                        velocity = new Vector2(moveDirX * Mathf.Cos(Mathf.Deg2Rad * slopeDownAngle), -Mathf.Sin(Mathf.Deg2Rad * slopeDownAngle)).normalized / 60 * movespeed;
//                    }

//                    collisionCheck = RCXY(velocity, velocity.magnitude, groundMask);

//                    if (collisionCheck)
//                    {
//                        velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
//                    }

//                    slidingDownSlope = true;
//                }

//                if (velocity.x != 0)
//                {

//                    //cast a ray down on each side of the collider to determine ground angles
//                    bool slopeDownFound = false;

//                    platformCasts = new RaycastHit2D[3];

//                    Physics2D.RaycastNonAlloc((Vector2)bounds.center + new Vector2(-bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y),
//                        Vector2.down, platformCasts, 3 * skin, groundMask);

//                    for (int i = 0; i < platformCasts.Length; i++)
//                    {
//                        if (platformCasts[i])
//                        {

//                            if (platformCasts[i].distance > 0)
//                            {
//                                slopeDownAngle = Vector2.Angle(Vector2.up, platformCasts[i].normal);
//                                slopeDownFound = true;
//                            }
//                        }
//                    }


//                    bool slopeUpFound = false;
//                    platformCasts = new RaycastHit2D[3];

//                    Physics2D.RaycastNonAlloc((Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y),
//                        Vector2.down, platformCasts, 3 * skin, groundMask);

//                    for (int i = 0; i < platformCasts.Length; i++)
//                    {
//                        if (platformCasts[i])
//                        {
//                            if (platformCasts[i].distance > 0)
//                            {
//                                slopeUpAngle = Vector2.Angle(Vector2.up, platformCasts[i].normal);
//                                slopeUpFound = true;
//                            }
//                        }
//                    }

//                    //try slope down first
//                    if (!slopeUpFound && slopeDownAngle > 0 && slopeDownAngle <= maxSlopeAngle) // !slopeUpFound, because you're not on a slope when both rays hit
//                    {
//                        velocity = new Vector2(Mathf.Sign(velocity.x) * Mathf.Cos(Mathf.Deg2Rad * slopeDownAngle), -Mathf.Sin(Mathf.Deg2Rad * slopeDownAngle)).normalized / 60;

//                        if (addMovement.x == 0) velocity *= Mathf.Abs(input.x) * movespeed;
//                        else velocity *= Mathf.Abs(addMovement.x);

//                        slopeDown = true;
//                    }

//                    //slope up
//                    if (!slopeDownFound && slopeUpAngle > 0 && slopeUpAngle <= maxSlopeAngle) //!slopeDownFound, because you're not on a slope when both rays hit
//                    {

//                        velocity = new Vector2(Mathf.Sign(velocity.x) * Mathf.Cos(Mathf.Deg2Rad * slopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * slopeUpAngle)).normalized / 60;

//                        if (addMovement.x == 0) velocity *= Mathf.Abs(input.x) * movespeed;
//                        else velocity *= Mathf.Abs(addMovement.x);

//                        slopeUp = true;
//                    }



//                    //check for a new slope up while moving
//                    RaycastHit2D slopeUpCast = RCL(velocity, velocity.magnitude * 2, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(velocity.x), -bounds.extents.y), groundMask);

//                    if (slopeUpCast)
//                    {
//                        float newSlopeUpAngle = Vector2.Angle(Vector2.up, slopeUpCast.normal);

//                        if (newSlopeUpAngle < 90 && slopeUpCast.distance > 0) //ignore the slope if inside of it, or no slope at all
//                        {
//                            velocity = Vector2.ClampMagnitude(velocity, HitDistance(slopeUpCast));

//                            onWall = true;
//                        }


//                        //new slope up
//                        if (newSlopeUpAngle > 0 && newSlopeUpAngle <= maxSlopeAngle && HitDistance(slopeUpCast) <= triangleDistance)
//                        {
//                            velocity = new Vector2(Mathf.Sign(velocity.x) * Mathf.Cos(Mathf.Deg2Rad * newSlopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * newSlopeUpAngle)).normalized / 60;

//                            if (addMovement.x == 0) velocity *= Mathf.Abs(input.x) * movespeed;
//                            else velocity *= Mathf.Abs(addMovement.x);

//                            slopeUp = true;
//                        }
//                    }


//                    //collisionCheck when moving
//                    collisionCheck = RCXY(velocity, velocity.magnitude, collisionMask);

//                    if (collisionCheck)
//                    {
//                        velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));

//                        onWall = true;
//                        onWall = true;
//                    }

//                    //check for new slope down
//                    RaycastHit2D newSlopeDownCast = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y + skin / 2) + velocity,
//                        new Vector2(bounds.size.x, skin), 0, Vector2.down, 1, groundMask);

//                    if (newSlopeDownCast)
//                    {
//                        //float newSlopeDownAngle = Vector2.Angle(Vector2.up, newSlopeDownCast.normal);

//                        if (newSlopeDownCast.distance > skin && newSlopeDownCast.distance < Mathf.Abs(velocity.x) + skin)
//                        {
//                            velocity.y += -(newSlopeDownCast.distance - skin);
//                            slopeDown = true;
//                        }
//                    }
//                }
//            }



//            if (!grounded)
//            {
//                if (oldGrounded)
//                {
//                    //Debug.Log("not grounded anymore");
//                }


//                if (velocity.y < maxFallSpeed / 60)
//                {
//                    velocity.y = maxFallSpeed / 60;
//                }


//                //only check walls and not platforms

//                if (velocity.x != 0)
//                {
//                    collisionCheck = RCXY(Vector2.right * Mathf.Sign(velocity.x), skin * 2, collisionMask);

//                    if (collisionCheck)
//                    {
//                        wallDirection = (int)Mathf.Sign(velocity.x);
//                        velocity.x = 0;
//                        onWall = true;

//                        if (velocity.y < -wallslideSpeed / 60) velocity.y = -wallslideSpeed / 60;
//                    }
//                }
//                collisionCheck = RCXY(velocity, velocity.magnitude, collisionMask); //check for other collisions and stop velocity

//                if (collisionCheck)
//                {
//                    velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
//                }

//                //check if landing
//                if (velocity.y <= 0)
//                {
//                    //check if landing on moving platforms
//                    RaycastHit2D transporterCheck = RCXY(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), transporterMask);

//                    if (transporterCheck)
//                    {
//                        transform.SetParent(transporterCheck.transform);
//                    }

//                    //reduce velocity when hitting the ground
//                    groundCheck = RCXY(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), collisionMask);

//                    if (groundCheck)
//                    {
//                        float groundAngle = Vector2.Angle(Vector2.up, groundCheck.normal);

//                        if (groundAngle <= maxSlopeAngle) //ohterwise gets stuck on walls
//                        {
//                            velocity = Vector2.ClampMagnitude(velocity, HitDistance(groundCheck));
//                        }
//                    }

//                    //reduce velocity when hitting platforms, ignore platforms inside you
//                    if (!fallThroughPlatform)
//                    {
//                        platformCasts = new RaycastHit2D[3];

//                        RCXY_noAl(velocity, velocity.magnitude, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), platformMask);

//                        for (int i = 0; i < platformCasts.Length; i++)
//                        {
//                            if (platformCasts[i])
//                            {
//                                if (platformCasts[i].distance > 0)
//                                {
//                                    float groundAngle = Vector2.Angle(Vector2.up, platformCasts[i].normal);

//                                    if (groundAngle <= maxSlopeAngle) //ohterwise gets stuck on walls
//                                    {
//                                        velocity = Vector2.ClampMagnitude(velocity, HitDistance(platformCasts[i]));
//                                    }
//                                }
//                            }
//                        }
//                    }


//                }
//            }
//        }


//        ManageFreeze();

//        ManageNotInControl();


//        //last collisioncheck only in case of errors
//        velocitybfCol = velocity;

//        collisionCheck = Physics2D.BoxCast((Vector2)bounds.center + velocity, bounds.size, 0, Vector2.zero, 0, collisionMask);

//        if (collisionCheck)
//        {
//            velocity = Vector2.zero;
//            lastCollisionCheck = true;
//        }

//        moveAngle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));



//        //actually move the transform
//        transform.Translate(velocity);

//        LateResetValues();
//    }

//    protected void LateResetValues()
//    {
//        oldInControl = inControl;
//        forceMovement = Vector2.zero;
//        resetVelocity = false;
//        addMovement = Vector2.zero;
//        fallThroughPlatform = false;
//        jumpVelocity = 0;
//    }

//    protected void ManageFreeze()
//    {
//        //manage freezing
//        if (freeze)
//        {
//            if (velocityAfterFreeze == Vector2.zero)
//            {
//                velocityAfterFreeze = velocity;
//            }

//            velocity = velocityAfterFreeze * -0.001f;
//        }
//        else
//        {
//            velocity += velocityAfterFreeze;
//            velocityAfterFreeze = Vector2.zero;
//        }

//        //forceMovement
//        if (forceMovement != Vector2.zero)
//        {
//            velocity = forceMovement / 60;

//            groundMask = collisionMask;

//            if (velocity.y <= 0)
//            {
//                groundMask += platformMask;
//            }

//            collisionCheck = RCXY(velocity, velocity.magnitude, groundMask); //check for all collisions and stop velocity

//            if (collisionCheck && inControl) // checks again in !inControl
//            {
//                velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
//            }

//        }
//    }

//    protected void ManageNotInControl()
//    {
//        //lost control -> character just falls down
//        if (!inControl)
//        {
//            //perform a groundcheck

//            groundCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2),
//        new Vector2(bounds.size.x, skin), 0, Vector2.down, skin * 3, collisionMask);

//            if (groundCheck)
//            {
//                grounded = true;

//                slopeDownAngle = Vector2.Angle(Vector2.up, groundCheck.normal);

//                groundPoint = groundCheck.point - (Vector2)bounds.center;
//            }


//            if (!fallThroughPlatform)
//            {
//                platformCasts = new RaycastHit2D[10];

//                RCXY_noAl(Vector2.down, skin * 2, (Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2), new Vector2(bounds.size.x, skin), platformMask);

//                for (int i = 0; i < platformCasts.Length; i++)
//                {
//                    if (platformCasts[i])
//                    {
//                        if (platformCasts[i].distance > 0) //ignore a platform when inside of the platform
//                        {
//                            grounded = true;

//                            slopeDownAngle = Vector2.Angle(Vector2.up, platformCasts[i].normal);
//                        }
//                    }
//                }
//            }



//            if (!freeze)
//            {
//                velocity.y += gravity / 60;

//                groundMask = collisionMask;

//                if (velocity.y <= 0)
//                {
//                    groundMask += platformMask;

//                    if (velocity.y < maxFallSpeed / 60 / 1.2f) //reduce maxfallspeed for testing
//                    {
//                        velocity.y -= gravity / 60 * 1.1f; //if too fast, slowly decrease fallspeed
//                    }
//                }

//                float dirX = Mathf.Sign(velocity.x);
//                velocity.x -= movespeed / 60 * aerialDeceleration / 5 * Mathf.Sign(velocity.x); // (/ 5) for less deceleration when not in control for better knockback feeling



//                if (dirX != Mathf.Sign(velocity.x))
//                {
//                    velocity.x = 0;
//                }

//                if (allowDI)
//                {
//                    velocity += movespeed * input / 60 * aerialAcceleration / 5; // (/ 5) like for the deceleration
//                }

//                velocity.x = Mathf.Clamp(velocity.x, -maxAirSpeed / 60, maxAirSpeed / 60);

//            }

//            //check for collisions
//            collisionCheck = RCXY(velocity, velocity.magnitude, groundMask); //check for all collisions and stop velocity

//            if (collisionCheck)
//            {
//                if (collisionCheck.distance > 0)
//                {
//                    collision = true;

//                    collisionReflect = Vector2.Reflect(velocity, collisionCheck.normal);

//                    lastCollisionAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);

//                    velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
//                }
//            }

//        }


//        if (oldInControl == false && inControl == true) //otherwise falls through platform after hitLand
//        {
//            groundMask = collisionMask;

//            if (velocity.y <= 0)
//            {
//                groundMask += platformMask;
//            }

//            //check for collisions
//            collisionCheck = RCXY(velocity, velocity.magnitude, groundMask); //check for all collisions and stop velocity

//            if (collisionCheck)
//            {
//                velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
//            }
//        }
//    }
//}