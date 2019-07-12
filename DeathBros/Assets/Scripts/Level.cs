using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tiled2Unity;

public class Level : MonoBehaviour, IState
{
    //[SerializeField]
    //protected MapSquare mapSquare;
    //public MapSquare MapSquare { get { return mapSquare; } }

    [SerializeField]
    protected Sprite mapSprite;
    public Sprite MapSrite { get { return mapSprite; } }

    [SerializeField]
    protected Level levelLeft, levelRight, levelTop, levelBottom;

    public string backgroundMusic;
    public List<GameObject> spawnEnemyList;

    [SerializeField]
    protected List<Transform> enemies;
    protected Transform enemiesTransform;
    public World world { get; protected set; }

    protected GameObject levelTransforms;

    public Transform backgroundForCamera { get; protected set; }

    public static event Action<Level> Entered;

    protected void Awake()
    {
        levelTransforms = GetComponentInChildren<TiledMap>().gameObject;

        if (levelTransforms == null)
        {
            Debug.Log("Level transform not found!");
        }

        enemiesTransform = transform.Find("Enemies");

        if (enemiesTransform != null)
        {
            foreach (Transform enemy in enemiesTransform)
            {
                enemies.Add(enemy);
            }
        }

        world = GetComponentInParent<World>();

        //mapSquare.position = transform.localPosition / 64;

        Exit();
    }

    public void Enter()
    {
        gameObject.SetActive(true);

        AudioManager.PlaySound(backgroundMusic, true);

        backgroundForCamera = levelTransforms.transform.Find("Background");

        Camera.main.GetComponent<CameraController>().SetLevel(backgroundForCamera);

        if (Entered != null) Entered(this);
    }

    public void Execute()
    {
        if (GameManager.PlayerLevelPosition.x < 0)
        {
            GameManager.ChangeLevel(levelLeft);
        }

        if (GameManager.PlayerLevelPosition.x > 64)
        {
            GameManager.ChangeLevel(levelRight);
        }

        if (GameManager.PlayerLevelPosition.y > 0)
        {
            GameManager.ChangeLevel(levelTop);
        }

        if (GameManager.PlayerLevelPosition.y < -64)
        {
            GameManager.ChangeLevel(levelBottom);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (spawnEnemyList.Count > 0)
            {
                if (spawnEnemyList[0] != null)
                {
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    position.z = 0;
                    Instantiate(spawnEnemyList[0], position, Quaternion.identity);

                    Debug.Log(spawnEnemyList[0].name + " spawned.");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (spawnEnemyList.Count > 1)
            {
                if (spawnEnemyList[1] != null)
                {
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    position.z = 0;
                    Instantiate(spawnEnemyList[1], position, Quaternion.identity);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (spawnEnemyList.Count > 2)
            {
                if (spawnEnemyList[2] != null)
                {
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    position.z = 0;
                    Instantiate(spawnEnemyList[2], position, Quaternion.identity);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (spawnEnemyList.Count > 3)
            {
                if (spawnEnemyList[2] != null)
                {
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    position.z = 0;
                    Instantiate(spawnEnemyList[3], position, Quaternion.identity);
                }
            }
        }
    }

    public void Exit()
    {
        AudioManager.StopSound(backgroundMusic);

        if (enemiesTransform != null)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] != null)
                    enemies[i].SetParent(enemiesTransform);
            }
        }

        gameObject.SetActive(false);
    }

    void Update()
    {

    }
}

/*
[System.Serializable]
public class MapSquare
{
    public string name;
    public Vector2 position;
}
*/
