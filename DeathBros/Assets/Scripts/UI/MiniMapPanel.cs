using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapPanel : _MB
{
    [SerializeField]
    protected World world;
    [SerializeField]
    protected GameObject emptySquare, fullSquare;

    private void Start()
    {
        world = GameManager.Instance.startWorld;

        Level.Entered += CreateNewMiniMap;
    }

    private void CreateNewMiniMap(Level currentLevel)
    {
        for (int i = 0; i < world.miniMap.mapSquares.Count; i++)
        {
            GameObject newSquare;
            newSquare = Instantiate(emptySquare, transform);
            newSquare.transform.localPosition = world.miniMap.mapSquares[i].position * 16;
        }
    }
}