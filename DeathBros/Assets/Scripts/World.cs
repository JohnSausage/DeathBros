using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : _MB
{
    [SerializeField]
    protected string worldName;
    public string WorldName { get { return worldName; } }

    public List<Level> Levels { get; protected set; }

    private void Start()
    {
        Levels = new List<Level>();

        foreach (Transform lvl in transform)
        {
            Level addLvl = lvl.GetComponent<Level>();

            if (addLvl != null)
            {
                Levels.Add(addLvl);
            }
        }
    }
}
