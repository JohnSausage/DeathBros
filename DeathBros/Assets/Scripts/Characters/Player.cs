using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public string soundFolderName = "Sounds/Player/";

    public Stat wallSlideSpeed = new Stat("WallslideSpeed", 5);

    public CStates_AdvancedMovement advancedMovementStates;
    public CStates_Attack attackStates;

    public override void Init()
    {
        base.Init();

        LoadSoundFiles();

        advancedMovementStates.Init(this);
        attackStates.Init(this);

        stats.AddStat(wallSlideSpeed);

        CStates_InitExitStates();
    }

    void Update()
    {
        DirectionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));
        if (Mathf.Abs(DirectionalInput.x) < 0.2f) DirectionalInput = new Vector2(0, DirectionalInput.y);

        if(Input.GetButtonDown("Fire1"))
        {
            Attack = true;
        }
        else
        {
            Attack = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump = true;
        }

        else
        {
            //reset at the end of FixedUpdate to not miss any inputs
            //Jump = false;
        }


        if (Input.GetButton("Jump"))
        {
            HoldJump = true;
        }
        else
        {
            HoldJump = false;
        }
    }

    private void LoadSoundFiles()
    {
        Object[] objects = Resources.LoadAll(soundFolderName);

        foreach (var o in objects)
        {
            Sound s = new Sound();
            s.clip = (AudioClip)o;
            s.name = o.name;
            AudioManager.AddSound((s));
        }
    }
}
