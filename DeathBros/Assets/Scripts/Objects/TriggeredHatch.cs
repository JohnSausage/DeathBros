using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredHatch : MonoBehaviour
{
    [SerializeField]
    protected float movespeed = 5f;
    [SerializeField]
    protected Vector2 localPositionOpen = new Vector2(1.6f, 0f);

    [SerializeField]
    protected List<GameObject> triggerGOs;

    [SerializeField]
    protected string triggerdSound = "NES_pickUp";

    [Space]

    [SerializeField]
    protected GameObject door;

    protected BoxCollider2D doorCol;

    protected List<ITrigger> triggers;

    protected bool isOpen = false;

    protected bool moveDoor = false;

    protected Vector2 targetPosition;


    void Start()
    {
        doorCol = door.GetComponent<BoxCollider2D>();

        triggers = new List<ITrigger>();

        foreach (GameObject go in triggerGOs)
        {
            ITrigger t = go.GetComponent<ITrigger>();

            if (t != null)
            {
                triggers.Add(t);

                t.ATriggered += CheckTriggers;
            }
        }
    }


    protected void CheckTriggers()
    {
        bool triggered = true;

        foreach (ITrigger t in triggers)
        {
            if (t.TriggerOn() == false)
            {
                triggered = false;
            }
        }
        if (isOpen == false)
        {
            if (triggered == true)
            {
                OpenDoor();
            }
        }
        else
        {
            if (triggered == false)
            {
                CloseDoor();
            }
        }
    }

    protected void FixedUpdate()
    {
        MoveDoor();
    }

    protected void OpenDoor()
    {
        isOpen = true;
        targetPosition = localPositionOpen;
        moveDoor = true;

        doorCol.enabled = false;

        GameManager.MainCamera.Shake(20);
        AudioManager.PlaySound(triggerdSound);
    }

    protected void CloseDoor()
    {
        isOpen = false;
        targetPosition = Vector2.zero;
        moveDoor = true;

        GameManager.MainCamera.Shake(20);
    }

    protected void MoveDoor()
    {
        if (moveDoor == true)
        {
            door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, targetPosition, movespeed / 60f);

            if ((Vector2)door.transform.localPosition == targetPosition)
            {
                moveDoor = false;

                if (isOpen == false)
                {
                    doorCol.enabled = true;
                }
            }
        }
    }
}
