﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FrameAnimation/FrameAnimation")]
public class FrameAnimation : ScriptableObject
{
    public string animationName;

    public List<Frame> frames;

    void Start()
    {
        frames = new List<Frame>();
    }
}
