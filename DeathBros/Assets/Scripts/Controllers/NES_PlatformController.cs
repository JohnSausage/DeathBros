using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NES_PlatformController : MonoBehaviour
{
    public LayerMask transportMask;

    public Vector2 Movement { get; private set; }

    [SerializeField]
    protected float movespeed = 1;

    [Space]
    public List<ColliderAndLayer> transportedThings;

    public List<Transform> parentedObjects;

    [Space]

    [SerializeField]
    protected GameObject platform;

    [SerializeField]
    protected BoxCollider2D TransporterCol;

    protected BoxCollider2D platformCol;
    protected Vector2 platformReducedSize;

    [SerializeField]
    protected Transform[] points;

    [Space]

    private Transform currentPoint;
    private int pointSelection;

    public BoxCollider2D Col { get; protected set; }

    private void Start()
    {
        currentPoint = points[pointSelection];
        platformCol = platform.GetComponent<BoxCollider2D>();
        platformReducedSize = platformCol.bounds.size * 0.9f;
    }

    private void FixedUpdate()
    {
        //reset the parents of the transported things
        foreach (ColliderAndLayer storedCollider in transportedThings)
        {
            //check if destroyed while on platform
            if (storedCollider.collider != null)
            {
                storedCollider.collider.transform.SetParent(null);
            }
        }

        //clear the list with the transported things
        transportedThings.Clear();

        //calculate the new movement
        Movement = (currentPoint.position - platform.transform.position).normalized * movespeed / 60f;

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


        //check if something that can be transported is in the transporter area
        RaycastHit2D findThingsToTransport = Physics2D.BoxCast(TransporterCol.bounds.center, TransporterCol.bounds.size, 0, Vector2.zero, 0, transportMask);

        //repeateldy cast while something was found
        while (findThingsToTransport)
        {
            //store everything that has been found
            ColliderAndLayer storeCollider = new ColliderAndLayer(findThingsToTransport.collider, findThingsToTransport.collider.gameObject.layer);

            //before setting its layer to ignore, check if is inside the platform collider
            RaycastHit2D checkIfInsidePlatform = Physics2D.BoxCast(platformCol.bounds.center, platformReducedSize, 0, Vector2.zero, 0, transportMask);

            transportedThings.Add(storeCollider);
            storeCollider.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            //only set parent if it is not inside the platform
            if (checkIfInsidePlatform == false)
            {
                storeCollider.collider.transform.SetParent(platform.transform);
            }

            //cast again
            findThingsToTransport = Physics2D.BoxCast(TransporterCol.bounds.center, TransporterCol.bounds.size, 0, Vector2.zero, 0, transportMask);
        }

        //move the platform
        platform.transform.Translate(Movement, Space.World);

        //reset the layermasks after moving
        foreach (ColliderAndLayer storedCollider in transportedThings)
        {
            storedCollider.collider.gameObject.layer = storedCollider.layerMask;
        }
    }
}
