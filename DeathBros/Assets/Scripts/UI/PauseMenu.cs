using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; protected set; }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);

            gameObject.SetActive(false);
        }
        else
        {
            Destroy(this);
            return;
        }
    }
}
