using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraController : MonoBehaviour
{
    public CanvasScaler canvasScaler;

    [Space]
    public Transform target;
    public Transform levelBoundsTransform;
    public float smoothSpeed = 0.125f;

    //[SerializeField]
    //private int pixelsPerUnit = 16;

    public float shakeSpeed = 1;
    public float shakeStrength = 1;

    public int shakeTimer;

    [SerializeField]
    protected LayerMask activateObjectsMask;

    protected Bounds levelBounds;
    protected Bounds cameraSizeBounds;
    protected Bounds movementBounds;
    protected Camera cam;

    protected BoxCollider2D activateGoCollider;

    public static Vector2 Position { get { return Camera.main.transform.position; } }
    public Bounds ActivateionBounds { get { return new Bounds(new Vector3(Position.x, Position.y, 0), activateGoCollider.bounds.size); } }

    void Start()
    {
        cam = Camera.main;
        activateGoCollider = GetComponent<BoxCollider2D>();

        //levelBounds = levelBoundsTransform.GetComponent<Renderer>().bounds;

        //Character.ATakesDamageAll += ShakeCameraOnDamage;
        //Character.ATakesDamageAll += FreezeCameraOnDamage;
        Character.ATakesDamageAll += FreezeThenShakeCameraOnDamage;

        //cam.orthographicSize = 160 / pixelsPerUnit / 2;
        //canvasScaler.referenceResolution = new Vector2(cam.pixelRect.width, cam.pixelRect.height);
    }

    private void MoveCamera()
    {
        MoveCameraToPlayer();

        if (shakeTimer > 0)
        {
            ShakeCamera();
        }

        //Vector3 newLocalPosition = Vector3.zero;

        //newLocalPosition.x = (Mathf.Round(transform.position.x * pixelsPerUnit) / pixelsPerUnit);
        //newLocalPosition.y = (Mathf.Round(transform.position.y * pixelsPerUnit) / pixelsPerUnit);
        //newLocalPosition.z = transform.position.z;

        //transform.position = newLocalPosition;
    }

    private void FixedUpdate()
    {
        if (shakeTimer > 0)
        {
            shakeTimer--;

            ShakeCamera();
        }

        MoveCamera();
    }

    private void ShakeCamera()
    {
        Vector3 newPos = Vector3.zero;

        newPos.x = (Mathf.PerlinNoise(Time.time * shakeSpeed * 60, 0) - 0.5f) * shakeStrength / 60;
        newPos.y = (Mathf.PerlinNoise(0, Time.time * shakeSpeed * 60) - 0.5f) * shakeStrength / 60;

        transform.position += newPos;
    }

    private void ShakeCameraOnDamage(Damage damage, Character chr)
    {
        shakeStrength = damage.damageNumber * 1.5f;
        shakeTimer = (int)damage.damageNumber;
    }

    private void FreezeCameraOnDamage(Damage damage, Character chr)
    {
        StartCoroutine(HitFreeze(damage));
    }

    private void FreezeThenShakeCameraOnDamage(Damage damage, Character chr)
    {
        StartCoroutine(IHitFreezeThenShake(damage, chr));
    }

    private IEnumerator IHitFreezeThenShake(Damage damage, Character chr)
    {
        Time.timeScale = 0.05f;

        float freezeTime = Mathf.Clamp(damage.damageNumber, 1f, 10f);
        freezeTime = freezeTime / 60f;

        yield return new WaitForSecondsRealtime(freezeTime);

        Time.timeScale = 1f;

        ShakeCameraOnDamage(damage, chr);
    }

    private IEnumerator HitFreeze(Damage damage)
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(damage.damageNumber / 100);
        Time.timeScale = 1f;
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

        movementBounds = activateGoCollider.bounds;
        movementBounds.center = new Vector3(movementBounds.center.x, movementBounds.center.y, 0);


        //smooth movement
        newPosition = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10), smoothSpeed);
        //newPosition = Vector3.MoveTowards(transform.position, new Vector3(target.position.x, target.position.y, -10), smoothSpeed);
        //newPosition = new Vector3(target.position.x, target.position.y, -10) - transform.position;

        //if (movementBounds.Contains(target.position))
        //{
        //    newPosition = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10), smoothSpeed);
        //}


        /*
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

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    IActivatedByCamera activatedObject = collision.GetComponent<IActivatedByCamera>();
    //    if (activatedObject != null)
    //    {
    //        activatedObject.isActive = true;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    IActivatedByCamera activatedObject = collision.GetComponent<IActivatedByCamera>();
    //    if (activatedObject != null)
    //    {
    //        activatedObject.isActive = false;
    //    }
    //}
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

public static class ColorExtensions
{
    public static Color SetTransparency(this Color color, float a)
    {
        return new Color(color.r, color.g, color.b, a);
    }
}