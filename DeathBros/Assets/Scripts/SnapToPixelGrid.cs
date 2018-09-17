using UnityEngine;

public class SnapToPixelGrid : MonoBehaviour
{
    [SerializeField]
    private int pixelsPerUnit = 16;

    private void LateUpdate()
    {
        Vector3 newLocalPosition = Vector3.zero;

        newLocalPosition.x = (Mathf.Round(transform.position.x * pixelsPerUnit) / pixelsPerUnit);
        newLocalPosition.y = (Mathf.Round(transform.position.y * pixelsPerUnit) / pixelsPerUnit);
        newLocalPosition.z = transform.position.z;

        transform.position = newLocalPosition;
    }
}