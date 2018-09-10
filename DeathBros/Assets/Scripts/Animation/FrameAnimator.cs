﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimator : _MB
{
    public FrameAnimation currentAnimation;

    public List<FrameAnimation> animations { get; protected set; }
    public FrameAnimationsSO frameAnimationsSO;

    public float animationSpeed = 1;

    private SpriteRenderer spr;
    private HurtboxManager hubM;

    private float frameTimer;
    private int animTimer;

    public override void Init()
    {
        base.Init();

        spr = GetComponent<SpriteRenderer>();
        hubM = GetComponent<HurtboxManager>();

        frameTimer = 0;
        animTimer = 0;

        //if (animations[0] != null)
        //    ChangeAnimation(animations[0]);

        if (frameAnimationsSO != null)
        {
            animations = frameAnimationsSO.frameAnimations;
            ChangeAnimation(animations[0]);
        }
    }

    void FixedUpdate()
    {
        PlayCurrentAnimation();
    }

    private void PlayCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            Frame currentFrame = currentAnimation.frames[animTimer];

            spr.sprite = currentFrame.sprite;
            hubM.SetHurtboxes(currentFrame);

            frameTimer += animationSpeed;

            if (frameTimer >= currentAnimation.frames[animTimer].duration)
            {
                animTimer++;
                frameTimer = 0;
            }

            if (animTimer >= currentAnimation.frames.Count)
            {
                animTimer = 0;
            }
        }
        else
        {
            Debug.Log("Current animation == null.");
        }
    }

    public FrameAnimation GetAnimation(string animationName)
    {
        return animations.Find(x => x.animationName == animationName);
    }

    public void ChangeAnimation(string animationName)
    {
        FrameAnimation animation = GetAnimation(animationName);
        Debug.Log(animationName);
        ChangeAnimation(animation);
    }

    public void ChangeAnimation(FrameAnimation animation)
    {
        if (animation != null)
        {
            currentAnimation = animation;

            animTimer = 0;
            frameTimer = 0;
        }
        else
        {
            Debug.Log("Animation not found.");
            /*
            for (int i = 0; i < animations.Count; i++)
            {
                Debug.Log(animations[i].name);
            }
            */
        }
    }
}
