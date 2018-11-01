using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : _MB
{
    public MiniMap miniMap { get; protected set; }

    [SerializeField]
    protected List<Level> levels;

    private void Start()
    {
        miniMap = new MiniMap();

        foreach (Transform lvl in transform)
        {
            Level addLvl = lvl.GetComponent<Level>();

            if (addLvl != null)
            {
                levels.Add(addLvl);
                miniMap.mapSquares.Add(addLvl.MapSquare);
            }
        }
    }
}

[System.Serializable]
public class MiniMap
{
    public List<MapSquare> mapSquares;

    public MiniMap()
    {
        mapSquares = new List<MapSquare>();
    }
}