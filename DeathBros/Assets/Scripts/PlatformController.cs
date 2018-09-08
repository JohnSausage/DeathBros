using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformController : MonoBehaviour
{
    public Vector2 Movement;// { get; private set; }

    [SerializeField] float movespeed = 1;

    [Space]

    [SerializeField] GameObject platform;
    [SerializeField] Transform[] points;

    Transform currentPoint;
    int pointSelection;

    private void Start()
    {
        currentPoint = points[pointSelection];
    }

    private void FixedUpdate()
    {
        Movement = (currentPoint.position - platform.transform.position).normalized * movespeed / 60f;

        platform.transform.Translate(Movement, Space.World);


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
