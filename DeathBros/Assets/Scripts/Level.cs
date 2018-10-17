using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour, IState
{
    public GameObject levelTransforms;

    public string backgroundMusic;

    void Start()
    {
        Exit();
    }

    public void Enter()
    {
        levelTransforms.SetActive(true);

        AudioManager.PlaySound(backgroundMusic);
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }

    void Update()
    {

    }
}
