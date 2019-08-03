using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SimpleController : MonoBehaviour
{
    [SerializeField]
    protected float gravity = -1;

    [SerializeField]
    protected LayerMask groundMask;



    protected BoxCollider2D col;
    protected Vector2 velocity;



    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        //Apply gravity
        velocity += Vector2.up * gravity;

        //Check for ground
        float skin = 0.05f;
        Vector2 size = new Vector2(col.size.x - skin, col.size.y - skin);

        RaycastHit2D groundCheck;
        groundCheck = Physics2D.BoxCast(col.offset + (Vector2)transform.position, size, 0, velocity, (velocity.magnitude + skin) / 60f, groundMask);

        //stop velocity on hit
        if (groundCheck)
        {
            velocity = velocity * (groundCheck.distance);
        }

        //Apply velocity
        transform.Translate(velocity / 60f);
    }

    public void SetVelocity(Vector2 velocity)
    {
        this.velocity = velocity;
    }
}
