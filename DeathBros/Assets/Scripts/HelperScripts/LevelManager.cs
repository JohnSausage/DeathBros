using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Level startingLevel { get { return allLevels[0]; } }

    public int startingLevelIndex;

    [SerializeField]
    protected List<Level> allLevels;

    #region Singelton
    public static LevelManager Instance { get; protected set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion


}
