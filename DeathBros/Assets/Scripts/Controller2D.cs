

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : MonoBehaviour
{
    public Vector2 input;

    public Vector2 velocity;
    public float angleX, angleY;
    public float oldAngleX;
    public bool grounded;
    public bool oldGrounded;
    public bool fallThroughPlatform;
    public bool onWall;

    public bool jump;

    public Vector2 hitDirection;
    public float gamma;
    public float alpha;

    public LayerMask collisionMask;
    public LayerMask platformMask;
    public BoxCollider2D Col { get; protected set; }

    public float gravity = -1f;
    public float movespeed = 10f;
    public float jumpForce = 20f;

    public float wallSlideSpeed = 3f;

    public float maxSlopeDownAngle = 45;
    public float maxSlopeUpAngle = 45;

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
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("Jump")) //jump
        {
            jump = true;
        }

        if (input.y < 0)
        {
            fallThroughPlatform = true;
        }
        else
        {
            fallThroughPlatform = false;
        }

        Move();

        //Cutoff velocity
        if (Mathf.Abs(velocity.x) < skin)
            velocity.x = 0;

        if (Mathf.Abs(velocity.y) < skin)
            velocity.y = 0;


        transform.Translate(velocity);
    }

    protected RaycastHit2D RCXY(Vector2 direction, float distance, bool shrink = true)
    {
        moveAngle = Vector2.Angle(velocity, Vector2.right * Mathf.Sign(velocity.x));

        gizmoVector = velocity.normalized;
        gizmoVector *= (velocity.magnitude);

        Bounds b;
        if (shrink)
            b = bounds;
        else
            b = Col.bounds;

        float skinDistance = skin;
        float cos = Mathf.Cos(moveAngle * Mathf.Deg2Rad);
        if (cos > skin)
            skinDistance = skin / cos;

        return Physics2D.BoxCast(b.center, b.size, 0, direction, distance + skinDistance, collisionMask);
    }

    protected RaycastHit2D RCXY(bool shrink = true)
    {
        return RCXY(velocity, velocity.magnitude, shrink);
    }

    private void Move()
    {
        oldGrounded = grounded;
        oldAngleX = angleX;
        angleX = angleY = 0;

        grounded = false;
        onWall = false;

        //update bounds
        bounds = fullBounds = Col.bounds;
        bounds.Expand(-2 * skin);

        //manage velocity.y
        if (oldGrounded)
            velocity.y = gravity / 60;

        else
            velocity.y += gravity / 60;

        if (jump)
        {
            if (oldGrounded)
                velocity.y = jumpForce / 60;
            jump = false;
        }

        velocity.x = input.x / 60 * movespeed;

        //check movement on y axis only
        //RaycastHit2D groundCheckY = RCXY(new Vector2(0, velocity.y), Mathf.Abs(velocity.y));


        RaycastHit2D groundCheckY;

        if (velocity.y <= 0 && !fallThroughPlatform)
        {
            groundCheckY = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y),
            new Vector2(bounds.size.x, skin), 0, new Vector2(0, velocity.y), Mathf.Abs(velocity.y), collisionMask + platformMask);
        }
        else
        {
            groundCheckY = Physics2D.BoxCast((Vector2)bounds.center - new Vector2(0, bounds.extents.y),
            new Vector2(bounds.size.x, skin), 0, new Vector2(0, velocity.y), Mathf.Abs(velocity.y), collisionMask);
        }

        if (groundCheckY)
        {
            velocity.y = Mathf.Sign(velocity.y) * (groundCheckY.distance - skin);

            if (Mathf.Abs(velocity.y) < skin)
            {
                velocity.y = 0;
            }

            grounded = true;

        }

        //check movement on x axis only
        if (!grounded || !oldGrounded)
        {
            RaycastHit2D wallCheck = RCXY(new Vector2(velocity.x, 0), Mathf.Abs(velocity.x));

            if (wallCheck)
            {

                velocity.x = Mathf.Sign(velocity.x) * (wallCheck.distance - skin);

                if (Mathf.Abs(velocity.x) < skin)
                {
                    velocity.x = 0;
                }

                onWall = true;
                grounded = false;

                if (velocity.y < 0)
                    velocity.y = -wallSlideSpeed / 60;

            }
        }

        //check if falling onto slope
        RaycastHit2D groundCheck = RCXY();

        if (groundCheck)
        {
            //triangle calculations
            hitDirection = (Vector2)bounds.center - groundCheck.point;
            hitDirection = new Vector2(Mathf.Sign(hitDirection.x), Mathf.Sign(hitDirection.y));

            if (hitDirection.y > 0)
            {
                grounded = true;
                angleY = Vector2.Angle(Vector2.up, groundCheck.normal);
            }

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

            //don't stick on roof
            if (!grounded)
            {
                velocity.y += gravity / 60;
                //velocity.x = 0;
            }

        }

        //movement when grounded
        if (oldGrounded) //oldGrounded to wait for being grounded in last frame
        {
            //don't stick on walls
            if (!onWall)
                velocity.x = input.x / 60 * movespeed;

            //check for descending slope
            gizmoSlopeDown = velocity + (Vector2)bounds.center - new Vector2(bounds.extents.x * Mathf.Sign(input.x), bounds.extents.y);

            RaycastHit2D slopeDown = Physics2D.Raycast(gizmoSlopeDown, Vector2.down, Mathf.Infinity, collisionMask);

            angleY = Vector2.Angle(Vector2.up, slopeDown.normal);

            float angleD = Vector2.Angle(slopeDown.normal, new Vector2(input.x, 0));

            //push player down to slope
            if (velocity.y < 0 && input.x != 0 && slopeDown && angleY > 0 && angleY <= maxSlopeDownAngle && angleD < 90)
            {
                grounded = true;

                alpha = angleY;
                gamma = 90 + Vector2.Angle(slopeDown.normal, new Vector2(Mathf.Sign(input.x), 1));

                velocity.y = -(slopeDown.distance - diag * (Mathf.Sin(Mathf.Deg2Rad * gamma) / Mathf.Sin(Mathf.Deg2Rad * alpha)));
            }

            //check for ascending slope
            if (velocity.x != 0)
            {
                RaycastHit2D hitXY = RCXY();

                angleX = Vector2.Angle(Vector2.up, hitXY.normal);

                //move up slope only when at the slope
                if (angleX <= maxSlopeUpAngle && angleX == oldAngleX)
                {
                    velocity = new Vector2(Mathf.Sign(input.x) * Mathf.Cos(Mathf.Deg2Rad * oldAngleX), Mathf.Sin(Mathf.Deg2Rad * oldAngleX)).normalized / 60 * movespeed;

                    //raycast again
                    hitXY = RCXY();
                }

                //check for collision when moving
                if (hitXY)
                {

                    angleX = Vector2.Angle(Vector2.up, hitXY.normal);

                    hitDirection = (Vector2)bounds.center - hitXY.point;
                    hitDirection = new Vector2(Mathf.Sign(hitDirection.x), Mathf.Sign(hitDirection.y));

                    gamma = Mathf.Abs(90 - Vector2.Angle(hitXY.normal, hitDirection));
                    alpha = Mathf.Abs(90 - Vector2.Angle(hitXY.normal, velocity));

                    float distance = skin;

                    distance = diag * Mathf.Sin(gamma * Mathf.Deg2Rad) / Mathf.Sin(alpha * Mathf.Deg2Rad);

                    float moveDistance = (hitXY.distance - distance);


                    velocity.Normalize();
                    velocity *= (moveDistance);

                }
            }
        }
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


