using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour, IState
{
    public GameObject levelTransforms;
    public Transform backgroundForCamera;

    public string backgroundMusic;

    void Start()
    {
        Exit();
    }

    public void Enter()
    {
        levelTransforms.SetActive(true);

        AudioManager.PlaySound(backgroundMusic);

        backgroundForCamera = levelTransforms.transform.Find("Background");

        Camera.main.GetComponent<CameraController>().SetLevel(backgroundForCamera);
    }

    public void Execute()
    {
    }

    public void Exit()
    {
        levelTransforms.SetActive(false);
    }

    void Update()
    {

    }
}
