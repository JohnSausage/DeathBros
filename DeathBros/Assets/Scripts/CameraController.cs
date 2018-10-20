using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraController : MonoBehaviour
{
    public Transform target;
    public Transform levelBoundsTransform;
    public float smoothSpeed = 0.125f;

    public float shakeSpeed = 1;
    public float shakeStrength = 1;

    public int shakeTimer;

    protected Bounds levelBounds;
    protected Bounds cameraSizeBounds;
    protected Bounds movementBounds;
    protected Camera cam;

    protected BoxCollider2D movementBoundsCollider;

    void Start()
    {
        cam = Camera.main;

        movementBoundsCollider = GetComponent<BoxCollider2D>();

        //levelBounds = levelBoundsTransform.GetComponent<Renderer>().bounds;

        Character.TakesDamageAll += ShakeCameraOnDamage;
    }

    void LateUpdate()
    {
        MoveCameraToPlayer();

        if (shakeTimer > 0)
        {
            ShakeCamera();
        }
    }

    private void FixedUpdate()
    {
        if (shakeTimer > 0)
        {
            shakeTimer--;

            ShakeCamera();
        }
    }

    private void ShakeCamera()
    {
        Vector3 newPos = Vector3.zero;

        newPos.x = (Mathf.PerlinNoise(Time.time * shakeSpeed * 60, 0) - 0.5f) * shakeStrength / 60;
        newPos.y = (Mathf.PerlinNoise(0, Time.time * shakeSpeed * 60) - 0.5f) * shakeStrength / 60;

        transform.position += newPos;
    }

    private void ShakeCameraOnDamage(Damage damage)
    {
        shakeStrength = damage.damageNumber * 2;
        shakeTimer = (int)damage.damageNumber;
    }

    public void SetLevel(Transform levelTransform)
    {
        levelBoundsTransform = levelTransform;
        levelBounds = levelBoundsTransform.GetComponentInChildren<Renderer>().bounds;
    }

    private void MoveCameraToPlayer()
    {
        Vector2 newPosition = transform.position;

        cameraSizeBounds = cam.OrthographicBounds();

        movementBounds = movementBoundsCollider.bounds;
        movementBounds.center = new Vector3(movementBounds.center.x, movementBounds.center.y, 0);



        //smooth movement
        newPosition = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10), smoothSpeed);
        /*
        if (movementBounds.Contains(target.position))
        {
            newPosition = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10), smoothSpeed);
        }



        if (target.position.x < movementBounds.Rect().xMin)
        {
            newPosition.x = target.position.x;// + cameraSizeBounds.extents.x;
        }

        if (target.position.x > movementBounds.Rect().xMax)
        {
            newPosition.x = target.position.x;// - cameraSizeBounds.extents.x;
        }

        if (target.position.y < movementBounds.Rect().yMin)
        {
            newPosition.y = target.position.y;
        }

        if (target.position.y > movementBounds.Rect().yMax)
        {
            newPosition.y = target.position.y;
        }
        */

        //contstrain camera bounds to level bounds
        if (newPosition.x - cameraSizeBounds.extents.x < levelBounds.Rect().xMin)
        {
            newPosition.x = levelBounds.Rect().xMin + cameraSizeBounds.extents.x;
        }

        if (newPosition.x + cameraSizeBounds.extents.x > levelBounds.Rect().xMax)
        {
            newPosition.x = levelBounds.Rect().xMax - cameraSizeBounds.extents.x;
        }

        if (newPosition.y - cameraSizeBounds.extents.y < levelBounds.Rect().yMin)
        {
            newPosition.y = levelBounds.Rect().yMin + cameraSizeBounds.extents.y;
        }

        if (newPosition.y + cameraSizeBounds.extents.y > levelBounds.Rect().yMax)
        {
            newPosition.y = levelBounds.Rect().yMax - cameraSizeBounds.extents.y;
        }

        //move camera
        transform.position = new Vector3(newPosition.x, newPosition.y, -10);
        //transform.position = Vector3.Lerp(transform.position, new Vector3(newPosition.x, newPosition.y, -10), 0.05f);
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