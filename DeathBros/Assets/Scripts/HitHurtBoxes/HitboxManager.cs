using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : _MB
{
    private Frame previousFrame;
    private SpriteRenderer spr;
    private bool flipped;

    public override void Init()
    {
        base.Init();

        spr = GetComponent<SpriteRenderer>();
    }
}

[System.Serializable]
public class Hitbox
{
    public Vector2 position;
    public float radius;

    public Hitbox Clone()
    {
        return new Hitbox
        {
            radius = radius,
            position = position
        };
    }
}