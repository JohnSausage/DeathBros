using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour, IState
{
    public GameObject levelTransforms;

    public void Enter()
    {
        levelTransforms.SetActive(true);
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }

    void Start()
    {
        Exit();
    }

    void Update()
    {

    }
}
