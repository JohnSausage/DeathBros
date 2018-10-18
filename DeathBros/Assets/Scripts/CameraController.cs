using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraController : MonoBehaviour
{
    public Transform target;
    public Transform levelBoundsTransform;
    public float smoothSpeed = 0.125f;

    protected Bounds levelBounds;
    protected Bounds cameraBounds;
    protected Camera cam;

    void Start()
    {
        cam = Camera.main;

        levelBounds = levelBoundsTransform.GetComponent<Renderer>().bounds;
    }

    void LateUpdate()
    {
        Vector2 newPosition;

        cameraBounds = cam.OrthographicBounds();

        //smooth movement
        newPosition = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10), smoothSpeed);

        //transform.position = new Vector3(target.position.x, target.position.y, -10);

        //newPosition = new Vector3(target.position.x, target.position.y, -10);

        /*
        if (target.position.x < movementBounds.PositionRect().xMin)
        {
            transform.position = new Vector3(target.position.x + movementBounds.extents.x, transform.position.y, transform.position.z);
        }

        if (target.position.x > movementBounds.PositionRect().xMax)
        {
            transform.position = new Vector3(target.position.x - movementBounds.extents.x, transform.position.y, transform.position.z);
        }

        if (target.position.y < movementBounds.PositionRect().yMin)
        {
            transform.position = new Vector3(transform.position.x, target.position.y + movementBounds.extents.y, transform.position.z);
        }

        if (target.position.y > movementBounds.PositionRect().yMax)
        {
            transform.position = new Vector3(transform.position.x, target.position.y - movementBounds.extents.y, transform.position.z);
        }
        */

        //contstrain camera bounds to level bounds
        if (newPosition.x - cameraBounds.extents.x < levelBounds.Rect().xMin)
        {
            newPosition.x = levelBounds.Rect().xMin + cameraBounds.extents.x;
        }

        if (newPosition.x + cameraBounds.extents.x > levelBounds.Rect().xMax)
        {
            newPosition.x = levelBounds.Rect().xMax - cameraBounds.extents.x;
        }

        if (newPosition.y - cameraBounds.extents.y < levelBounds.Rect().yMin)
        {
            newPosition.y = levelBounds.Rect().yMin + cameraBounds.extents.y;
        }
        
        if (newPosition.y + cameraBounds.extents.y > levelBounds.Rect().yMax)
        {
            newPosition.y = levelBounds.Rect().yMax - cameraBounds.extents.y;
        }
        
        //move camera
        transform.position = new Vector3(newPosition.x, newPosition.y, -10);
    }
}

public static class CameraExtensions
{
    public static Bounds OrthographicBounds(this Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
}

public static class BoundsExtensions
{
    public static Rect Rect(this Bounds bounds)
    {
        return new Rect(bounds.center - bounds.extents, bounds.size);
    }
}