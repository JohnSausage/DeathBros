using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Controller that works for NES Project */
/* Only certain slopes are supported */

[RequireComponent(typeof(BoxCollider2D))]
public class NES_BasicController2D : MonoBehaviour
{
    /* local variables */
    //protected Vector2 velocity;
    public Vector2 DirectionalInput;
    public float JumpVelocity;
    public Vector2 velocity;// { get { return velocity; } }
    public Vector2 oldVelocity;
    public bool FallThroughPlatforms;//{get;set;}
    public Vector2 ForceMovement;
    public Vector2 AddMovement;

    protected float faceDirection;
    public float FaceDirection { set { faceDirection = value; } }

    public Vector2 Position { get { return col.offset; } }

    protected BoxCollider2D col;
    protected Bounds bounds;
    public static float skin = 0.01f;

    [SerializeField]
    protected CtrStateMachine ctrSM;

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
    public bool HasCollided;// { get; set; }
    public bool OnWall;// { get; set; }
    public bool OnLedge;// { get; set; }
    public bool Frozen;// { get; protected set; }
    public bool InControl;//get;set;}
    public bool IsJumping;//get;set;}
    public int WallDirection { get { return (int)Mathf.Sign(DirectionalInput.x); } }
    public void FreezeCtr(int freezeForFrames)
    {
        Frozen = true;
        freezeCounter = freezeForFrames;
        ChangeState(CtrStateMachine.frozen);
    }

    public bool Tumble = false;// { get; protected set; }
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

    public float collisionAngle = 0f;

    public bool slopeUpFound;
    public bool slopeDownFound;

    public List<ColliderAndLayer> ignoredPlatforms;

    /* Masks used for collisions */
    [SerializeField]
    protected LayerMask collisionMask;

    [SerializeField]
    protected LayerMask platformMask;

    [SerializeField]
    protected LayerMask transporterMask;

    protected LayerMask groundMask;

    /* initialization */
    protected void Start()
    {
        ctrSM = new CtrStateMachine();
        ChangeState(CtrStateMachine.airborne);

        col = GetComponent<BoxCollider2D>();

        velocity = Vector2.zero;
    }

    /* Used to move the object in the objects FixedUpdate cycle */
    public virtual void FixedMove()
    {
        bounds = col.bounds;
        bounds.Expand(-2 * skin);

        CheckIfInsidePlatforms();

        ctrSM.FixedUpdate(this);

        ClearIgnoredPlatforms();

        transform.Translate(velocity);
    }

    public void ChangeState(CtrState newState)
    {
        ctrSM.ChangeState(this, newState);
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

            if(velocity.y < 0)
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
            reflectedVelocity = Vector2.Reflect(velocity, collisionCheck.normal);

            HasCollided = true;

            collisionAngle = Vector2.Angle(Vector2.up, collisionCheck.normal);

            velocity = Vector2.ClampMagnitude(velocity, HitDistance(collisionCheck));
        }

        return collisionCheck;
    }

    public bool CheckIfGrounded()
    {
        if(IsJumping)
        {
            return false;
        }

        RaycastHit2D groundCheck = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y - skin / 2f),
            new Vector2(bounds.size.x, skin), 0, Vector2.down, skin * 2, groundMask);

        return groundCheck;
    }

    public bool CheckForSlopeUp()
    {
        RaycastHit2D slopeUpCheck = RayCastLine(velocity, velocity.magnitude + skin, (Vector2)bounds.center + new Vector2(bounds.extents.x * Mathf.Sign(DirectionalInput.x), -bounds.extents.y), groundMask);

        if (slopeUpCheck)
        {
            velocity = Vector2.ClampMagnitude(velocity, HitDistance(slopeUpCheck));

            slopeUpAngle = Vector2.Angle(Vector2.up, slopeUpCheck.normal);


            if (slopeUpAngle == 0)
            {
                ChangeState(CtrStateMachine.groundedMoving);
            }
            else if (slopeUpAngle <= MaxSlopeAngle)
            {
                movingSlopeUpAngle = slopeUpAngle;

                ChangeState(CtrStateMachine.groundedSlopeUp);
            }
            else
            {
                HasCollided = true;
            }
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
        }

        return newSlopeUpCheck;
    }

    public void CheckForSlopeDown()
    {
        RaycastHit2D slopeDownCheck = RayCastLine(Vector2.down, skin, (Vector2)bounds.center + new Vector2(-bounds.extents.x * Mathf.Sign(DirectionalInput.x), -bounds.extents.y), groundMask);

        if (slopeDownCheck)
        {
            slopeDownAngle = Vector2.Angle(Vector2.up, slopeDownCheck.normal);

            if (slopeDownAngle == 0)
            {

            }
            else if (slopeDownAngle <= MaxSlopeAngle)
            {
                movingSlopeDownAngle = slopeDownAngle;

                ChangeState(CtrStateMachine.groundedSlopeDown);
            }
        }
    }

    public void CheckForNewSlopeDown()
    {
        RaycastHit2D newSlopeDownCast = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y + skin / 2) + velocity,
                       new Vector2(bounds.size.x, skin), 0, Vector2.down, 1, groundMask);

        if (newSlopeDownCast)
        {
            if (newSlopeDownCast.distance > skin && newSlopeDownCast.distance < Mathf.Abs(velocity.x) + skin)
            {
                velocity.y += -(newSlopeDownCast.distance - skin);

                slopeDownAngle = Vector2.Angle(Vector2.up, newSlopeDownCast.normal);

                if (slopeDownAngle == 0)
                {
                    ChangeState(CtrStateMachine.groundedMoving);
                }
                else if (slopeDownAngle <= MaxSlopeAngle)
                {
                    movingSlopeDownAngle = slopeDownAngle;

                    ChangeState(CtrStateMachine.groundedSlopeDown);
                }
            }
        }
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
        groundMask = collisionMask + platformMask;

        if (FallThroughPlatforms)
        {
            groundMask -= platformMask;
        }

        if (IsGrounded == false && velocity.y > 0)
        {
            groundMask -= platformMask;
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
        velocity.x -= 1 / 60 * aerialDeceleration * Mathf.Sign(velocity.x);

        if (dirX != Mathf.Sign(velocity.x))
        {
            velocity.x = 0;
        }

        velocity.x += DirectionalInput.x * DIStrength / 60 * aerialAcceleration;

        velocity.x = Mathf.Clamp(velocity.x, -maxAerialSpeed / 60, maxAerialSpeed / 60);

        if (velocity.y > 0)
        {
            velocity = velocity.normalized * (velocity.magnitude + DirectionalInput.y * DIStrength / 60 * aerialAcceleration);
        }
    }
}

[System.Serializable]
public class CtrStateMachine
{
    public string currentState;

    public CtrState CurrentState { get; protected set; }
    protected CtrState previousState;

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

        currentState = CurrentState.ToString();
    }

    public void ChangeState(NES_BasicController2D ctr, CtrState newState)
    {
        if (newState != null)
        {
            CurrentState.Exit(ctr);
            previousState = CurrentState;
            CurrentState = newState;
            newState.Enter(ctr);
        }
        else
        {
            Debug.Log("CtrStateMachine - New State not found!");
        }
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

    }

    public virtual void Execute(NES_BasicController2D ctr)
    {
        ctr.SetGroundMask();

        ctr.HasCollided = false;
        ctr.OnWall = false;
        ctr.collisionAngle = 0f;
      
        if (ctr.Frozen)
        {
            ctr.ChangeState(CtrStateMachine.frozen);
        }

        MoveCtr(ctr);

    }

    public virtual void Exit(NES_BasicController2D ctr)
    {

    }

    protected virtual void MoveCtr(NES_BasicController2D ctr)
    {
        if(ctr.resetVelocity)
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
        ctr.IsGrounded = true;

        ctr.ResetSlopeAngles();
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        

        if (ctr.CheckIfGrounded() == false)
        {
            ctr.ChangeState(CtrStateMachine.airborne);
            return;
        }

        if (ctr.DirectionalInput != Vector2.zero)
        {
            ctr.ChangeState(CtrStateMachine.groundedMoving);
        }

        ctr.CheckForCollision();
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
        ctr.IsGrounded = true;

        ctr.ResetSlopeAngles();
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForSlopeUp();
        ctr.CheckForSlopeDown();
        ctr.CheckForNewSlopeDown();

        ctr.CheckJumpVelocity();

        if (ctr.DirectionalInput == Vector2.zero)
        {
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }

        if (ctr.CheckIfGrounded() == false)
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
        ctr.IsGrounded = true;
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForSlopeUp();
        ctr.CheckForNewSlopeDown();

        ctr.CheckJumpVelocity();

        if (ctr.DirectionalInput == Vector2.zero)
        {
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }

        if (ctr.CheckIfGrounded() == false)
        {
            ctr.ChangeState(CtrStateMachine.airborne);
        }
    }

    protected override void ApplyStateMovement(NES_BasicController2D ctr)
    {
        ctr.velocity = new Vector2(Mathf.Sign(ctr.DirectionalInput.x) * Mathf.Cos(Mathf.Deg2Rad * ctr.movingSlopeUpAngle), Mathf.Sin(Mathf.Deg2Rad * ctr.movingSlopeUpAngle)).normalized / 60 * ctr.Movespeed;
        ctr.velocity *= Mathf.Abs(ctr.DirectionalInput.x);
    }
}

public class CtrS_groundedSlopeDown : CtrState
{
    public override void Enter(NES_BasicController2D ctr)
    {
        base.Enter(ctr);
        ctr.IsGrounded = true;
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForSlopeUp();
        ctr.CheckForNewSlopeDown();

        ctr.CheckJumpVelocity();

        if (ctr.DirectionalInput == Vector2.zero)
        {
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }

        if (ctr.CheckIfGrounded() == false)
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
        ctr.IsGrounded = false;
        ctr.fastFall = false;
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForOnWall();
        ctr.CheckForCollision();

        ctr.CheckForNewSlopeUp();

        if (ctr.CheckIfGrounded() == true)
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
        ctr.frozenVelocity = ctr.velocity;
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.freezeCounter--;

        ctr.frozenVelocity += ctr.velocity;
        ctr.velocity = Vector2.zero;

        if (ctr.freezeCounter <= 0)
        {
            if (ctr.Tumble == true)
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

        ctr.Frozen = false;
        ctr.InControl = true;
        ctr.velocity = ctr.frozenVelocity;
        ctr.CheckForCollision();
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
        ctr.IsGrounded = false;
        ctr.fastFall = false;
    }

    public override void Execute(NES_BasicController2D ctr)
    {
        base.Execute(ctr);

        ctr.CheckForOnWall();
        ctr.CheckForCollision();

        ctr.CheckForNewSlopeUp();

        if (ctr.CheckIfGrounded() == true)
        {
            ctr.ChangeState(CtrStateMachine.groundedIdle);
        }
    }

    public override void Exit(NES_BasicController2D ctr)
    {
        base.Exit(ctr);

        ctr.Tumble = false;
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