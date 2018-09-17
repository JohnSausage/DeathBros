﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Frame
{
    public int duration = 1;
    public Sprite sprite;
    public bool newHitID;

    public List<Hurtbox> hurtboxes;
    public List<Hitbox> hitboxes;

    public string soundName;

    public Frame()
    {
        hurtboxes = new List<Hurtbox>();
    }
}