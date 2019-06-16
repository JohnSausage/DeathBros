using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameManager : MonoBehaviour {

    [SerializeField]
    protected PauseMenu menu;

    public static StartGameManager Instance { get; protected set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        PauseMenu.Open();
    }
    // Update is called once per frame
    void Update () {
		
	}
}
