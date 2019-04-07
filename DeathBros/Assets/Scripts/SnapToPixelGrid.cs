using UnityEngine;

public class SnapToPixelGrid : MonoBehaviour
{
    [SerializeField]
    private bool snapToParent = false;

    [SerializeField]
    private int pixelsPerUnit = 16;

    private Vector3 oldPosition;

    private void LateUpdate()
    {
        if(oldPosition == transform.position)
        {
            return;
        }

        if (snapToParent) transform.localPosition = Vector3.zero;

        Vector3 newLocalPosition;
        newLocalPosition = Vector3.zero;

        newLocalPosition.x = (Mathf.Round(transform.position.x * pixelsPerUnit) / pixelsPerUnit);
        newLocalPosition.y = (Mathf.Round(transform.position.y * pixelsPerUnit) / pixelsPerUnit);
        newLocalPosition.z = transform.position.z;

        transform.position = newLocalPosition;

        oldPosition = newLocalPosition;
    }
}