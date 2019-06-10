using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NES_PlatformController : MonoBehaviour
{

    public LayerMask transportMask;

    public Vector2 Movement;// { get; private set; }

    [SerializeField]
    float movespeed = 1;

    [Space]
    public List<ColliderAndLayer> transportedThings;

    public List<Transform> parentedObjects;

    [Space]

    [SerializeField]
    GameObject platform;

    [SerializeField]
    BoxCollider2D TransporterCol;

    [SerializeField]
    Transform[] points;

    [Space]

    private Transform currentPoint;
    private int pointSelection;

    public BoxCollider2D Col { get; protected set; }

    private void Start()
    {
        currentPoint = points[pointSelection];
        //Col = platform.GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        foreach (ColliderAndLayer storedCollider in transportedThings)
        {
            storedCollider.collider.transform.SetParent(null);
        }

        transportedThings.Clear();


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


        RaycastHit2D findThingsToTransport = Physics2D.BoxCast(TransporterCol.bounds.center, TransporterCol.bounds.size, 0, Vector2.zero, 0, transportMask);

        while (findThingsToTransport)
        {
            ColliderAndLayer storeCollider = new ColliderAndLayer(findThingsToTransport.collider, findThingsToTransport.collider.gameObject.layer);
            transportedThings.Add(storeCollider);

            storeCollider.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            storeCollider.collider.transform.SetParent(platform.transform);

            findThingsToTransport = Physics2D.BoxCast(TransporterCol.bounds.center, TransporterCol.bounds.size, 0, Vector2.zero, 0, transportMask);
        }

        platform.transform.Translate(Movement, Space.World);

        foreach (ColliderAndLayer storedCollider in transportedThings)
        {
            storedCollider.collider.gameObject.layer = storedCollider.layerMask;

            //ICanBeTransported transportedObject = storedCollider.collider.GetComponent<ICanBeTransported>();
            //if (transportedObject != null)
            //{
            //    if (transportedObject.allowTransporting == true)
            //    {
            //        //storedCollider.collider.transform.Translate(Movement, Space.World);
            //        //transportedObject.requestedPlatformMovement = Movement;

            //        //storedCollider.collider.transform.SetParent(null);
            //    }
            //}
        }

        //transportedThings.Clear();
    }
}
