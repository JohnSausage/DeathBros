using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Controller that works for NES Project */
/* Only certain slopes are supported */

[RequireComponent(typeof(BoxCollider2D))]
public class NES_BasicController2D : MonoBehaviour
{
    public string ID;
    /* local variables */
    //protected Vector2 velocity;
    public Vector2 DirectionalInput;
    protected Vector2 LastDirectionalInput;
    public float JumpVelocity;
    public Vector2 velocity;// { get { return velocity; } }
    public Vector2 oldVelocity;
    public bool FallThroughPlatforms;//{get;set;}
    public Vector2 ForceMovement;
    public Vector2 AddMovement;

    protected float faceDirection;
    public float FaceDirection { set { faceDirection = value; } }

    public Vector2 Position { get { return transform.TransformPoint(col.offset); } }

    protected BoxCollider2D col;
    protected Bounds bounds;
    public static float skin = 0.01f;

    [SerializeField]
    public CtrStateMachine ctrSM; //@@@set protected after debugging

    /* can be changed during runtime*/
    public float Gravity = -1;// { get; set; }
    public float Movespeed = 10;// { get; set; }
    public float MaxSlopeAngle = 50; //{get;set;}
    public float aerialAcceleration = 0.2f;
    public float aerialDeceleration = 0.05f;
    public float maxAerialSpeed = 30;
    public float DIStrength = 2;
    public float maxFallSpeed = -15;
    public float fastFallSpeed = -20;
    public bool fastFall = false;
    public float WallslideSpeed = 5;

    /* checks */
    public bool IsGrounded;// { get; set; }
    public bool WasGrounded;// { get; set; }
    public bool HasCollided;// { get; set; }
    public bool OnWall;// { get; set; }
    public bool OnLedge;// { get; set; }
    public bool Frozen;// { get; protected set; }
    public bool InControl;//get;set;}
    public bool IsJumping;//get;set;}
    public bool IsMovingSlopeUp;
    public bool IsMovingSlopeDown;
    public bool NewSlopeSownFound;
    public bool IsOnSlope;

    public int WallDirection { get { return (int)Mathf.Sign(DirectionalInput.x); } }
    public void FreezeCtr(int freezeForFrames)
    {
        Frozen = true;
        freezeCounter = freezeForFrames;
        ChangeState(CtrStateMachine.frozen);
    }

    public bool IsInTumble = false;// { get; protected set; }
    public bool resetVelocity = false;

    public int freezeCounter = 0;
    public Vector2 frozenVelocity = Vector2.zero;
    public Vector2 reflectedVelocity = Vector2.zero;
    public Vector2 plannedVelocity = Vector2.zero;
    public Vector2 requestedPlatformMovement = Vector2.zero;

    public float slopeUpAngle = 0f;
    public float movingSlopeUpAngle = 0f;

    public float slopeDownAngle = 0f;
    public float movingSlopeDownAngle = 0f;

    public float CollisionAngle = 0f;

    public bool slopeUpFound;
    public bool slopeDownFound;

    public EGroundMoveState moveState = EGroundMoveState.Idle;

    public List<ColliderAndLayer> ignoredPlatforms;

    /* Masks used for collisions */
    [SerializeField]
    protected LayerMask collisionMask;

    [SerializeField]
    protected LayerMask platformMask;

    [SerializeField]
    protected LayerMask transporterMask;

    public LayerMask groundMask;

    /* initialization */
    protected void Start()
    {
        ctrSM = new CtrStateMachine();
        ChangeState(CtrStateMachine.airborne);

        col = GetComponent<BoxCollider2D>();

        velocity = Vector2.zero;
    }

    /* Used to move the object in the objects FixedUpdate cycle */
    public virtual void UpdateCtr()
    {
        bounds = col.bounds;
        bounds.Expand(-2 * skin);

        HasCollided = false;
        OnWall = false;
        IsGrounded = false;
        IsJumping = false;
        CollisionAngle = 0f;
        IsOnSlope = false;

        //ResetSlopeAngles();

        SetGroundMask();
        CheckIfInsidePlatforms();

        GiantUpdate();

        //if (Frozen)
        //{
        //    ChangeState(CtrStateMachine.frozen);
        //}
        //else if (IsInTumble)
        //{
        //    ChangeState(CtrStateMachine.tumble);
        //}

        //ctrSM.PerformStateChange(this);
        //ctrSM.FixedUpdate(this);

        ClearIgnoredPlatforms();

        transform.Translate(velocity);

        WasGrounded = IsGrounded;
        LastDirectionalInput = DirectionalInput;
    }

    public enum EGroundMoveState { None, Idle, Moving, SlopeUp, SlopeDown };

    protected void GiantUpdate()
    {
        if (Frozen)
        {
            FrozenUpdate();
            return;
        }

        if(resetVelocity == true)
        {
            velocity = Vector2.zero;
        }

        //check if jumpvelocity was set, changes velocity and sets IsJumping
        CheckJumpVelocity();


        if (IsJumping == false)
        {
            //Sets IsGrounded
            CheckIfGrounded();
        }

        if (ChangedInputDirection())
        {
            moveState = EGroundMoveState.Idle;
        }



        if (IsGrounded == true)
        {
            if (DirectionalInput.x == 0)
            {
                moveState = EGroundMoveState.Idle;
            }


            //grounded movement
            switch (moveState)
            {
                case EGroundMoveState.Idle:
                    {
                        ResetSlopeAngles();

                        velocity.y = 0;
                        velocity.x = 0;

                        ApplyAddMovementToVelocity();

                        if (DirectionalInput.x != 0)
                        {
                            CheckForSlopeDown();

                            if (slopeDownAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeDownAngle <= MaxSlopeAngle)
                            {
                                movingSlopeDownAngle = slopeDownAngle;
                                moveState = EGroundMoveState.SlopeDown;
                            }
                        }


                        break;
                    }
                case EGroundMoveState.Moving:
                    {
                        ResetSlopeAngles();

                        //normal sideways movement
                        velocity.y = 0;
                        velocity.x = DirectionalInput.x * Movespeed / 60;

                        ApplyAddMovementToVelocity();

                        if (CheckForNewSlopeDown() == true)
                        {
                            //no collisioncheck
                        }
                        else
                        {
                            CheckForCollision();
                        }

                        if (CheckForSlopeUp() == true)
                        {
                            if (slopeUpAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeUpAngle <= MaxSlopeAngle)
                            {
                                movingSlopeUpAngle = slopeUpAngle;
                                moveState = EGroundMoveState.SlopeUp;
                            }
                            else
                            {
                                HasCollided = true;
                            }
                        }

                        break;
                    }
                case EGroundMoveState.SlopeUp:
                    {
                        //moving slope up
                        velocity = new Vector2(Mathf.Sign(DirectionalInput.x) * Mathf.Cos(Mathf.Deg2Rad * movingSlopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * movingSlopeUpAngle)).normalized / 60 * Movespeed;
                        velocity *= Mathf.Abs(DirectionalInput.x);

                        ApplyAddMovementToVelocity();

                        if (CheckForNewSlopeUp() == true)
                        {
                            if (slopeUpAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeUpAngle <= MaxSlopeAngle)
                            {
                                movingSlopeUpAngle = slopeUpAngle;
                                moveState = EGroundMoveState.SlopeUp;
                            }
                            else
                            {
                                HasCollided = true;
                            }
                        }

                        if (CheckForSlopeDown() == true)
                        {
                            if (slopeDownAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeDownAngle <= MaxSlopeAngle)
                            {
                                movingSlopeDownAngle = slopeDownAngle;
                                moveState = EGroundMoveState.SlopeDown;
                            }
                        }

                        if(CheckForNewSlopeDown() == true)
                        {
                            //no collisioncheck
                        }
                        else
                        {
                            CheckForCollision();
                        }

                        break;
                    }

                case EGroundMoveState.SlopeDown:
                    {
                        velocity = new Vector2(Mathf.Sign(DirectionalInput.x) * Mathf.Cos(Mathf.Deg2Rad * movingSlopeDownAngle), -Mathf.Sin(Mathf.Deg2Rad * movingSlopeDownAngle)).normalized / 60 * Movespeed;
                        velocity *= Mathf.Abs(DirectionalInput.x);

                        ApplyAddMovementToVelocity();

                        if (CheckForSlopeUp() == true)
                        {
                            if (slopeUpAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                            else if (slopeUpAngle <= MaxSlopeAngle)
                            {
                                movingSlopeUpAngle = slopeUpAngle;
                                moveState = EGroundMoveState.SlopeUp;
                            }
                            else
                            {
                                HasCollided = true;
                            }
                        }

                        if (CheckForNewSlopeDown() == true)
                        {
                            if (slopeDownAngle == 0)
                            {
                                moveState = EGroundMoveState.Moving;
                            }
                        }
                        else
                        {
                            CheckForCollision();
                        }

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            ApplyForceMovementToVelocity();

        }
        else //IsGrounded == false
        {
            //reset ground move state
            moveState = EGroundMoveState.Moving;

            if (IsInTumble == true)
            {
                //Tumble movement in air

                CalculateTumbleVelocityX();
                ApplyTumbleGravity();

                ApplyAddMovementToVelocity();
                ApplyForceMovementToVelocity();

                CheckForOnWall();

                //check if landing on platform
                CheckForCollision();

                CheckForNewSlopeUp();
            }
            else //IsInTumble == false
            {
                //Normal movement in air
                CalculateAerialVelocityX();
                ApplyGravity();

                ApplyAddMovementToVelocity();
                ApplyForceMovementToVelocity();

                CheckForOnWall();

                //check if landing on platform
                CheckForCollision();

                CheckForNewSlopeUp();
            }
        }
    }

    protected void FrozenUpdate()
    {
        velocity = Vector2.zero;

        CheckForCollision();
    }

    public bool ChangedInputDirection()
    {
        bool retVal = false;

        if (DirectionalInput.x != 0)
        {
            if (Mathf.Sign(DirectionalInput.x) != Mathf.Sign(LastDirectionalInput.x))
            {
                retVal = true;
            }
        }

        return retVal;
    }

    public void ChangeState(CtrState newState)
    {
        if (ctrSM.CurrentState == newState)
        {
            return;
        }

        else
        {
            ctrSM.ChangeState(this, newState);
        }
    }

    public void ApplyAddMovementToVelocity()
    {
        velocity += AddMovement / 60f;
        AddMovement = Vector2.zero;
    }

    public void ApplyForceMovementToVelocity()
    {
        if (ForceMovement != Vector2.zero)
        {
            velocity = ForceMovement / 60f;
            ForceMovement = Vector2.zero;

            CheckForCollision();
        }
    }

    public void ApplyGravity()
    {
        /* slow down gravity at the apex of the jump */
        if (velocity.y < 0 && velocity.y > 2 * Gravity / 60f)
        {
            velocity.y += Gravity / 60f / 3f;
        }
        else
        {
            velocity.y += Gravity / 60f;
        }

        if (velocity.y >= 0)
        {
            fastFall = false;
        }

        /* adjust fallspeed */
        if (fastFall)
        {
            velocity.y = fastFallSpeed / 60f;
        }
        else
        {
            if (velocity.y < maxFallSpeed / 60f)
            {
                velocity.y = maxFallSpeed / 60f;
            }
        }
    }

    public void ApplyTumbleGravity()
    {
        float velYAfterGravity = velocity.y + Gravity / 60f;

        //only apply gravity when slower than fastFallSpeed
        if (velYAfterGravity > fastFallSpeed)
        {
            velocity.y = velYAfterGravity;
        }
        else
        { //faster than fastfallspeed
            //increase or decrease fallspeed woth di;
            velocity.y += DirectionalInput.y * DIStrength / 60 * aerialAcceleration;
        }
    }

    public void CheckJumpVelocity()
    {
        if (JumpVelocity != 0)
        {
            velocity.y = JumpVelocity / 60;
            JumpVelocity = 0;

            IsJumping = true;
        }
        else
        {
            IsJumping = false;
        }
    }

    public bool CheckForOnWall()
    {
        RaycastHit2D onWallCheck = RayCastXY(Vector2.right * Mathf.Sign(DirectionalInput.x), Mathf.Abs(velocity.x) + skin, collisionMask);

        if (onWallCheck)
        {
            OnWall = true;
            velocity.x = (onWallCheck.distance - skin) * Mathf.Sign(DirectionalInput.x);

            if (velocity.y < 0)
            {
                velocity.y = -WallslideSpeed / 60f;
            }
        }

        return onWallCheck;
    }

    public bool CheckForOnLedge()
    {
        Debug.Log("CheckForOnLedge() not implemented yet");
        return false;
    }

    public bool CheckForCollision()
    {
        RaycastHit2D collisionCheck = RayCastXY(velocity, velocity.magnitude + skin, collisionMask);

        if (collisionCheck)
        {
            if (velocity != Vector2.zero)
            {
                reflectedVelocity = Vector2.Reflect(velocity, collisionCheck.normal);
            }

            HasCollided = true;

            CollisionAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);

            velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
        }
        else
        {
            HasCollided = false;
        }

        return collisionCheck;
    }


    /// <summary>
    /// checks if the ctr is touching the ground by raycasting
    /// </summary>
    /// <returns> the RaycastHit2D if true and false otherwise </returns>
    public bool CheckIfGrounded()
    {

        // don't check the same frame that the ctr starts jumping 
        if (IsJumping)
        {
            return false;
        }

        // don't check if the ctr is in tumble and is getting launched upwards
        if (IsInTumble)
        {
            if (velocity.y > 0)
            {
                return false;
            }
        }

        // dont check if in the air and moving up
        if(WasGrounded == false)
        {
            if(velocity.y > 0)
            {
                return false;
            }
        }

        // cast a ctr-wide and skin high box at the bottom of the ctr
        RaycastHit2D groundCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2f),
            new Vector2(bounds.size.x, skin), 0, Vector2.down, skin * 2f, groundMask);

        if (groundCheck)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }

        return groundCheck;
    }

    public bool CheckForSlopeUp()
    {
        RaycastHit2D slopeUpCheck = RayCastLine(velocity, velocity.magnitude + skin, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(DirectionalInput.x), -bounds.extents.y), groundMask);

        if (slopeUpCheck)
        {
            velocity = Vector2.ClampMagnitude(velocity, HitDistance(slopeUpCheck));

            slopeUpAngle = Vector2.Angle(Vector2.up, slopeUpCheck.normal);
        }

        return slopeUpCheck;
    }

    public bool CheckForNewSlopeUp()
    {
        RaycastHit2D newSlopeUpCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2f),
           new Vector2(bounds.size.x, skin), 0, velocity, velocity.magnitude + skin, groundMask);

        if (newSlopeUpCheck)
        {
            velocity = Vector2.ClampMagnitude(velocity, HitDistance(newSlopeUpCheck));

            slopeUpAngle = Vector2.Angle(Vector2.up, newSlopeUpCheck.normal);
        }

        return newSlopeUpCheck;
    }

    public bool CheckForSlopeDown()
    {
        RaycastHit2D slopeDownCheck = RayCastLine(Vector2.down, skin * 2, (Vector2)bounds.center + new Vector2(-bounds.extents.x * Mathf.Sign(DirectionalInput.x), -bounds.extents.y), groundMask);

        if (slopeDownCheck)
        {
            slopeDownAngle = Vector2.Angle(Vector2.up, slopeDownCheck.normal);
        }

        return slopeDownCheck;
    }

    public bool CheckForNewSlopeDown()
    {
        RaycastHit2D newSlopeDownCast = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y + skin / 2) + velocity,
                       new Vector2(bounds.size.x, skin), 0, Vector2.down, 4, groundMask);

        if (newSlopeDownCast)
        {

            if (newSlopeDownCast.distance > skin && newSlopeDownCast.distance < Mathf.Abs(velocity.x) * 2 + skin)
            {
                velocity.y += -(newSlopeDownCast.distance - skin);

                slopeDownAngle = Vector2.Angle(Vector2.up, newSlopeDownCast.normal);
            }
        }

        return newSlopeDownCast;
    }

    public RaycastHit2D RayCastXY(Vector2 direction, float distance, LayerMask layerMask)
    {
        float angle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        float skinDistance = skin;

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        if (cos > skin) skinDistance = skin / cos;

        return Physics2D.BoxCast(bounds.center, bounds.size, 0, direction, distance + skinDistance, layerMask);
    }

    public RaycastHit2D RayCastLine(Vector2 direction, float distance, Vector2 center, LayerMask layerMask)
    {
        float angle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        float skinDistance = skin;

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        if (cos > skin)
            skinDistance = skin / cos;

        return Physics2D.Raycast(center, direction, distance + skinDistance, layerMask);
    }

    public float HitDistance(RaycastHit2D collision)
    {
        Vector2 hitDirection;

        hitDirection = (Vector2)bounds.center - collision.point;
        hitDirection = new Vector2(Mathf.Sign(hitDirection.x), Mathf.Sign(hitDirection.y));

        float gamma = Mathf.Abs(90 - Vector2.Angle(collision.normal, hitDirection));
        float alpha = Mathf.Abs(90 - Vector2.Angle(collision.normal, velocity));

        float triangleDistance = skin;

        triangleDistance = skin * Mathf.Sqrt(2) * Mathf.Sin(gamma * Mathf.Deg2Rad) / Mathf.Sin(alpha * Mathf.Deg2Rad);

        float moveDistance = (collision.distance - triangleDistance);

        //to stop backwards movemenet
        if (moveDistance < 0) moveDistance = 0;

        return moveDistance;
    }

    public void SetGroundMask()
    {
        groundMask = collisionMask;

        if (FallThroughPlatforms == false)
        {
            groundMask += platformMask;
        }
    }

    public void ResetSlopeAngles()
    {
        slopeDownAngle = 0;
        slopeUpAngle = 0;
        movingSlopeDownAngle = 0;
        movingSlopeUpAngle = 0;
    }

    /* checks if the player is inside a platform and adds those to a list of ignored platforms */
    public void CheckIfInsidePlatforms()
    {
        ignoredPlatforms.Clear();

        /* repeatedly check for platforms and add them to the list of ignored platforms*/
        RaycastHit2D platformCheck = RayCastXY(Vector2.up, 0, platformMask);

        while (platformCheck == true)
        {
            ColliderAndLayer storeCollider = new ColliderAndLayer(platformCheck.collider, platformCheck.collider.gameObject.layer);
            ignoredPlatforms.Add(storeCollider);

            storeCollider.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            platformCheck = RayCastXY(Vector2.up, 0, platformMask);
        }
    }

    public void SetIgnoredPlatforms()
    {
        foreach (ColliderAndLayer storedCollider in ignoredPlatforms)
        {
            storedCollider.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    public void ClearIgnoredPlatforms()
    {
        foreach (ColliderAndLayer storedCollider in ignoredPlatforms)
        {
            storedCollider.collider.gameObject.layer = storedCollider.layerMask;
        }
    }

    public void CalculateAerialVelocityX()
    {
        float dirX = Mathf.Sign(velocity.x);
        velocity.x -= Movespeed / 60 * aerialDeceleration * Mathf.Sign(velocity.x);

        if (dirX != Mathf.Sign(velocity.x))
        {
            velocity.x = 0;
        }

        velocity.x += DirectionalInput.x * Movespeed / 60 * aerialAcceleration;

        velocity.x = Mathf.Clamp(velocity.x, -Movespeed / 60, Movespeed / 60);
    }

    public void CalculateTumbleVelocityX()
    {
        float dirX = Mathf.Sign(velocity.x);

        //natural deceleration
        velocity.x -= 1 / 60 * aerialDeceleration * Mathf.Sign(velocity.x);

        //stop wehn changing directions
        if (dirX != Mathf.Sign(velocity.x))
        {
            velocity.x = 0;
        }


        //allow DI away only if slower than maxairspeed
        float velXAfterDI = velocity.x + DirectionalInput.x * DIStrength / 60 * aerialAcceleration;

        if (Mathf.Abs(velXAfterDI) <= maxAerialSpeed)
        {
            velocity.x = velXAfterDI;
        }

        //allow vectoring away only if slower than maxairspeed
        if (velocity.y > 0)
        {
            Vector2 velAfterVectoring = velocity.normalized * (velocity.magnitude + DirectionalInput.y * DIStrength / 60 * aerialAcceleration);

            if (Mathf.Abs(velAfterVectoring.x) <= maxAerialSpeed)
            {
                velocity.x = velAfterVectoring.x;
            }

            velocity.y = velAfterVectoring.y;
        }
    }
}

[System.Serializable]
public class CtrStateMachine
{
    public string currentStateName;

    public CtrState CurrentState { get; protected set; }
    protected CtrState previousState;
    protected CtrState newState;

    public static CtrS_groundedIdle groundedIdle = new CtrS_groundedIdle();
    public static CtrS_groundedMoving groundedMoving = new CtrS_groundedMoving();
    public static CtrS_groundedSlopeUp groundedSlopeUp = new CtrS_groundedSlopeUp();
    public static CtrS_groundedSlopeDown groundedSlopeDown = new CtrS_groundedSlopeDown();
    public static CtrS_airborne airborne = new CtrS_airborne();
    public static CtrS_frozen frozen = new CtrS_frozen();
    public static CtrS_tumble tumble = new CtrS_tumble();

    public CtrStateMachine()
    {
        CurrentState = new CtrState();
        previousState = new CtrState();
    }

    public void FixedUpdate(NES_BasicController2D ctr)
    {
        CurrentState.Execute(ctr);

        currentStateName = CurrentState.ToString();
    }

    public void PerformStateChange(NES_BasicController2D ctr)
    {
        if (newState != null)
        {
            if (newState != CurrentState)
            {
                CurrentState.Exit(ctr);
                previousState = CurrentState;
                CurrentState = newState;
                newState.Enter(ctr);
            }
        }
        else
        {
            Debug.Log("CtrStateMachine - New State not found!");
        }
    }

    public void ChangeState(NES_BasicController2D ctr, CtrState newState)
    {
        this.newState = newState;
        //if (newState != null)
        //{
        //    CurrentState.Exit(ctr);
        //    previousState = CurrentState;
        //    CurrentState = newState;
        //    newState.Enter(ctr);
        //}
        //else
        //{
        //    Debug.Log("CtrStateMachine - New State not found!");
        //}
    }

    public void GoToPreviousState(NES_BasicController2D ctr)
    {
        ChangeState(ctr, previousState);
    }
}

public class CtrState
{
    public virtual void Enter(NES_BasicController2D ctr)
    {
        Debug.Log(ctr.ID + " enter " + GetType());
    }

    public virtual void Execute(NES_BasicController2D ctr)
    {
        ctr.SetGroundMask();

        ctr.HasCollided = false;
        ctr.OnWall = false;
        ctr.IsGrounded = false;
        ctr.CollisionAngle = 0f;

        if (ctr.Frozen)
        {
            ctr.ChangeState(CtrStateMachine.frozen);
        }
        else if (ctr.IsInTumble)
        {
            ctr.ChangeState(CtrStateMachine.tumble);
        }

        MoveCtr(ctr);

    }

    public virtual void Exit(NES_BasicController2D ctr)
    {
        Debug.Log(ctr.ID + "exit " + this.GetType());
    }

    protected virtual void MoveCtr(NES_BasicController2D ctr)
    {
        if (ctr.resetVelocity)
        {
            ctr.velocity = Vector2.zero;
            ctr.resetVelocity = false;
        }

        ApplyStateMovement(ctr);
        ctr.ApplyAddMovementToVelocity();
        ctr.ApplyForceMovementToVelocity();
    }

    protected virtual void ApplyStateMovement(NES_BasicController2D ctr)
    {

    }
}

public class CtrS_groundedIdle : CtrState
{
    public override void Enter(NES_BasicController2D ctr)
    {
        base.Enter(ctr);

        ctr.ResetSlopeAngles();
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckIfGrounded();

        ctr.CheckForCollision();

        if (ctr.IsGrounded == false)
        {
            ctr.ChangeState(CtrStateMachine.airborne);
        }

        if (ctr.DirectionalInput != Vector2.zero)
        {
            ctr.ChangeState(CtrStateMachine.groundedMoving);
        }
    }

    public override void Exit(NES_BasicController2D ctr)
    {
        base.Exit(ctr);
    }

    protected override void ApplyStateMovement(NES_BasicController2D ctr)
    {
        ctr.velocity = Vector2.zero;
        ctr.velocity += ctr.requestedPlatformMovement;

        ctr.CheckJumpVelocity();
    }
}

public class CtrS_groundedMoving : CtrState
{
    public override void Enter(NES_BasicController2D ctr)
    {
        base.Enter(ctr);

        ctr.ResetSlopeAngles();
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForSlopeUp();
        ctr.CheckForSlopeDown();
        ctr.CheckForNewSlopeDown();
        ctr.CheckJumpVelocity();

        ctr.CheckIfGrounded();

        if (ctr.DirectionalInput == Vector2.zero)
        {
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }

        if (ctr.IsGrounded == false)
        {
            ctr.ChangeState(CtrStateMachine.airborne);
        }
    }

    protected override void ApplyStateMovement(NES_BasicController2D ctr)
    {
        ctr.velocity.y = 0;
        ctr.velocity.x = ctr.DirectionalInput.x * ctr.Movespeed / 60;
    }
}

public class CtrS_groundedSlopeUp : CtrState
{
    public override void Enter(NES_BasicController2D ctr)
    {
        base.Enter(ctr);
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForSlopeUp();
        ctr.CheckForNewSlopeDown();
        ctr.CheckJumpVelocity();

        ctr.CheckIfGrounded();


        if (ctr.DirectionalInput.x == 0)
        {
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }

        if (ctr.IsGrounded == false)
        {
            ctr.ChangeState(CtrStateMachine.airborne);
        }
    }

    protected override void ApplyStateMovement(NES_BasicController2D ctr)
    {
        if (ctr.ChangedInputDirection() == true)
        {
            ctr.velocity = Vector2.zero;
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }
        else
        {
            ctr.velocity = new Vector2(Mathf.Sign(ctr.DirectionalInput.x) * Mathf.Cos(Mathf.Deg2Rad * ctr.movingSlopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * ctr.movingSlopeUpAngle)).normalized / 60 * ctr.Movespeed;
            ctr.velocity *= Mathf.Abs(ctr.DirectionalInput.x);
        }
    }
}

public class CtrS_groundedSlopeDown : CtrState
{
    public override void Enter(NES_BasicController2D ctr)
    {
        base.Enter(ctr);
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForSlopeUp();
        ctr.CheckForNewSlopeDown();
        ctr.CheckJumpVelocity();

        ctr.CheckIfGrounded();

        if (ctr.DirectionalInput == Vector2.zero)
        {
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }

        if (ctr.IsGrounded == false)
        {
            ctr.ChangeState(CtrStateMachine.airborne);
        }

        if (ctr.CheckForNewSlopeUp())
        {
            ctr.ChangeState(CtrStateMachine.groundedMoving);
        }
    }

    protected override void ApplyStateMovement(NES_BasicController2D ctr)
    {
        ctr.velocity = new Vector2(Mathf.Sign(ctr.DirectionalInput.x) * Mathf.Cos(Mathf.Deg2Rad * ctr.movingSlopeDownAngle), -Mathf.Sin(Mathf.Deg2Rad * ctr.movingSlopeDownAngle)).normalized / 60 * ctr.Movespeed;
        ctr.velocity *= Mathf.Abs(ctr.DirectionalInput.x);
    }
}

public class CtrS_airborne : CtrState
{
    public override void Enter(NES_BasicController2D ctr)
    {
        base.Enter(ctr);
        ctr.fastFall = false;
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForOnWall();

        ctr.CheckForCollision();

        ctr.CheckForNewSlopeUp();
        ctr.CheckIfGrounded();

        if (ctr.IsGrounded == true)
        {
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }

    }

    protected override void ApplyStateMovement(NES_BasicController2D ctr)
    {
        ctr.CalculateAerialVelocityX();
        ctr.ApplyGravity();
        ctr.CheckJumpVelocity();
    }
}

public class CtrS_frozen : CtrState
{
    public override void Enter(NES_BasicController2D ctr)
    {
        base.Enter(ctr);
        ctr.IsGrounded = false;
        ctr.InControl = false;

        ctr.frozenVelocity = Vector2.zero;
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForCollision();

        ctr.velocity = Vector2.zero;

        if (ctr.Frozen == false)
        {
            if (ctr.IsInTumble == true)
            {
                ctr.ChangeState(CtrStateMachine.tumble);
            }
            else
            {
                ctr.ChangeState(CtrStateMachine.airborne);
            }
        }
    }

    public override void Exit(NES_BasicController2D ctr)
    {
        base.Exit(ctr);

        ctr.InControl = true;
    }

    protected override void ApplyStateMovement(NES_BasicController2D ctr)
    {
        ctr.JumpVelocity = 0;
        ctr.velocity = Vector2.zero;
    }
}

public class CtrS_tumble : CtrState
{
    public override void Enter(NES_BasicController2D ctr)
    {
        base.Enter(ctr);

        ctr.fastFall = false;


    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForCollision();
        ctr.CheckForOnWall();
        ctr.CheckIfGrounded();

        Debug.Log(ctr.ID + " execute ctrs_tumble velocity: " + ctr.velocity * 60);

        //ctr.CheckForNewSlopeUp();

        if (ctr.IsInTumble == false)
        {
            ctr.ChangeState(CtrStateMachine.airborne);
        }

        if (ctr.IsGrounded == true)
        {
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }
    }

    public override void Exit(NES_BasicController2D ctr)
    {
        base.Exit(ctr);

        ctr.IsInTumble = false;
    }

    protected override void ApplyStateMovement(NES_BasicController2D ctr)
    {
        ctr.CalculateTumbleVelocityX();
        ctr.ApplyGravity();
    }
}

[System.Serializable]
public struct ColliderAndLayer
{
    public Collider2D collider;
    public LayerMask layerMask;

    public ColliderAndLayer(Collider2D collider, LayerMask layerMask)
    {
        this.collider = collider;
        this.layerMask = layerMask;
    }
}