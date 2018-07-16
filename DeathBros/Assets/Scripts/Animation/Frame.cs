using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Frame
{
    public int duration = 1;
    public Sprite sprite;

    public List<Hurtbox> hurtBoxes;

    public Frame()
    {
        hurtBoxes = new List<Hurtbox>();
    }
}