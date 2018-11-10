using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesCollider : DamagingCollider
{
    protected override void SetCollider()
    {
        Col = GetComponentInChildren<Collider2D>();
    }
}
