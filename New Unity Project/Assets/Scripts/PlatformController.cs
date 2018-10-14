using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformController : MonoBehaviour
{
    public LayerMask transportMask;

    public Vector2 Movement;// { get; private set; }

    [SerializeField] float movespeed = 1;

    [Space]

    [SerializeField] GameObject platform;
    [SerializeField] BoxCollider2D TransporterCol;
    [SerializeField] Transform[] points;

    [Space]

    Transform currentPoint;
    int pointSelection;

    public BoxCollider2D Col { get; protected set; }

    private void Start()
    {
        currentPoint = points[pointSelection];
        Col = platform.GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        Movement = (currentPoint.position - platform.transform.position).normalized * movespeed / 60f;

        platform.transform.Translate(Movement, Space.World);



        //RaycastHit2D target = Physics2D.BoxCast((Vector2)Col.bounds.center + Movement, Col.bounds.size, 0, Movement, Movement.magnitude, transportMask);
        RaycastHit2D target = Physics2D.BoxCast((Vector2)Col.bounds.center + Movement, Col.bounds.size, 0, Vector2.zero, 0, transportMask);

        if (target)
        {

            RaycastHit2D transporting = Physics2D.BoxCast((Vector2)TransporterCol.bounds.center, TransporterCol.bounds.size, 0, Vector2.zero, 0, transportMask);


            if (!transporting)
            {
                target.transform.Translate(Movement);
            }

        }



        //moving
        if ((platform.transform.position - currentPoint.position).sqrMagnitude <= 0.1f)
        {
            Movement = Vector2.zero;

            pointSelection++;

            if (pointSelection >= points.Length)
            {
                pointSelection = 0;
            }

            currentPoint = points[pointSelection];
        }
    }
}
