using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public string followTag = "Player";

    public float attackRange = 1f;

    public Transform target;

    public Vector2 TargetDirection;// { get; protected set; }
    public Vector2 TargetVector;// { get; protected set; }

    public float DistanceToTarget
    {
        get
        {
            return TargetVector.magnitude;
        }
    }

    public bool InAttackRange
    {
        get
        {
            return DistanceToTarget <= attackRange;
        }
    }

    public Enemy enemy { get; protected set; }

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        target = GameObject.FindGameObjectWithTag(followTag).transform;    
    }

    private void FixedUpdate()
    {
        float targetX = Mathf.Clamp(target.position.x - transform.position.x, -1f, 1f);
        float targetY = Mathf.Clamp(target.position.y - transform.position.y, -1f, 1f);

        TargetDirection = new Vector2(targetX, targetY);

        TargetVector = target.position - transform.position;
    }
}
