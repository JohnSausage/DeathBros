using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform parallaxCamera;
    public float parallaxScale = 0.1f;

    public Vector2 start;
    public Vector2 currentCameraPosition;
    void Start()
    {
        start = parallaxCamera.position;
    }

    void FixedUpdate()
    {
        currentCameraPosition = (Vector2)parallaxCamera.position - start;

        transform.localPosition = -currentCameraPosition * parallaxScale;
    }
}
