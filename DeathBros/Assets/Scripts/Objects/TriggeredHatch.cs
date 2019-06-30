using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredHatch : MonoBehaviour
{
    [SerializeField]
    protected List<GameObject> triggerGOs;


    [Space]

    [SerializeField]
    protected GameObject door;

    protected List<ITrigger> triggers;
    protected bool isOpen = false;

    void Start()
    {
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

        if (triggered == true)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    protected void OpenDoor()
    {
        isOpen = true;
        StartCoroutine(MoveDoor(1.6f));
        //door.transform.localPosition = new Vector3(1.6f, 0, 0);
    }

    protected void CloseDoor()
    {
        isOpen = false;
        StartCoroutine(MoveDoor(0f));
        //door.transform.localPosition = new Vector3(0, 0, 0);
    }

    protected IEnumerator MoveDoor(float posX)
    {
        if(isOpen == true)
        {
            while (door.transform.localPosition.x < posX)
            {
                door.transform.Translate(Vector2.right  / 60f);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (door.transform.localPosition.x > 0)
            {
                door.transform.Translate(Vector2.left / 60f);
                yield return new WaitForEndOfFrame();
            }
        }

        
    }
}
