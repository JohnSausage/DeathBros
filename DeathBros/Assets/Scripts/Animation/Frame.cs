using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Frame
{
    public int duration = 1;
    public Sprite sprite;
    public bool newHitID;

    public bool applyGravity = true;
    public bool resetVelocity = false;
    public Vector2 forceMovement;
    public Vector2 addMovment;
    public Vector2 spawnProjectile = Vector2.zero;
    public GameObject spawnHoldItem;

    public List<Hurtbox> hurtboxes;
    public List<Hitbox> hitboxes;

    public string soundName;

    public Frame()
    {
        hurtboxes = new List<Hurtbox>();
    }
}