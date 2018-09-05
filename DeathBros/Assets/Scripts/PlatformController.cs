using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformController : MonoBehaviour
{
    public Vector2 Movement;// { get; private set; }

    [SerializeField] float speed;
    [SerializeField] Transform start,end;

    private Vector3 startPoint, endPoint;

    private Vector3 goal;

    void Start()
    {
        start = transform.Find("Start");
        end = transform.Find("End");

        startPoint = start.position;
        endPoint = end.position;

        goal = endPoint;
    }


    void FixedUpdate()
    {
        Movement = (goal - transform.position).normalized * speed / 60;

        if ((transform.localPosition - goal).sqrMagnitude < 0.1f)
        {
            ChangeGoal();
        }

        transform.Translate(Movement);

    }

    private void ChangeGoal()
    {
        if (goal == endPoint)
            goal = startPoint;
        else
            goal = endPoint;
    }
}
