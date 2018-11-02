using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapPanel : _MB
{
    [SerializeField]
    protected GameObject mapSquarePrefab;

    [SerializeField]
    protected GameObject currentSquarePrefab;

    protected World world;

    private void Start()
    {
        world = GameManager.Instance.startWorld;

        Level.Entered += CreateNewMiniMap;

        for (int i = 0; i < world.Levels.Count; i++)
        {
            GameObject newSquare;
            newSquare = Instantiate(mapSquarePrefab, transform);

            if (world.Levels[i].MapSrite != null)
            {
                newSquare.GetComponent<Image>().sprite = world.Levels[i].MapSrite;
            }
            else
            {
                Debug.Log("No mapSprite defined for " + world.Levels[i].name);
            }

            newSquare.transform.localPosition = world.Levels[i].transform.position / 64 * 16;
        }
    }

    private void CreateNewMiniMap(Level currentLevel)
    {
        currentSquarePrefab.transform.localPosition = currentLevel.transform.position / 64 * 16;

        /*
        for (int i = 0; i < world.Levels.Count; i++)
        {
            GameObject newSquare;
            newSquare = Instantiate(mapSquarePrefab, transform);

            if (world.Levels[i].MapSrite != null)
            {
                newSquare.GetComponent<Image>().sprite = world.Levels[i].MapSrite;
            }
            else
            {
                Debug.Log("No mapSprite defined for " + world.Levels[i].name);
            }

            newSquare.transform.localPosition = world.Levels[i].transform.position / 64 * 16;
        }
        */
    }
}