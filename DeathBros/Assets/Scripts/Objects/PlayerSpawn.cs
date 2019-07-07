using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField]
    protected int spawnerID;

    public int SpawnerID { get { return spawnerID; } }
}
