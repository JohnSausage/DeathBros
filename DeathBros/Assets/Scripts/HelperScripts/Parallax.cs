using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float parallaxScale = 0.1f;

    private Transform parallaxCamera;
    private Vector2 start;
    private Vector2 currentCameraPosition;

    void Start()
    {
        parallaxCamera = Camera.main.transform;
        //start = parallaxCamera.position;

        start = GetComponentInChildren<Renderer>().bounds.center;
    }

    void FixedUpdate()
    {
        currentCameraPosition = (Vector2)parallaxCamera.position - start;

        transform.localPosition = currentCameraPosition * parallaxScale;
    }
}
