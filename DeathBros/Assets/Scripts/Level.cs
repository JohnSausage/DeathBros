using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour, IState
{
    public GameObject levelTransforms;
    public string backgroundMusic;

    public List<GameObject> enemyList;

    public Transform backgroundForCamera { get; protected set; }

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
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (enemyList.Count > 0)
            {
                if (enemyList[0] != null)
                {
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    position.z = 0;
                    Instantiate(enemyList[0], position, Quaternion.identity);

                    Debug.Log(enemyList[0].name + " spawned.");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (enemyList.Count > 1)
            {
                if (enemyList[1] != null)
                {
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    position.z = 0;
                    Instantiate(enemyList[1], position, Quaternion.identity);
                }
            }
        }
    }

    public void Exit()
    {
        levelTransforms.SetActive(false);
    }

    void Update()
    {

    }
}
