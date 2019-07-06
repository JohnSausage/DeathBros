using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameAnimatorUI : _MB
{
    public FrameAnimation currentAnimation;

    public List<FrameAnimation> animations { get; protected set; }
    public FrameAnimationsSO frameAnimationsSO;

    public float animationSpeed = 1;

    public Image image;
    public event Action<FrameAnimation> AnimationOver;

    private float frameTimer;
    private int animTimer;

    public bool animationOver { get; protected set; }
    public string lastAnimationName { get; protected set; }
    public bool stopAnimation { get; set; }

    public override void Init()
    {
        base.Init();

        image = GetComponent<Image>();

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

    public void CopyFrameAnimator(FrameAnimator fanim)
    {
        this.frameAnimationsSO = fanim.frameAnimationsSO;
        this.animations = fanim.animations;
    }

    void FixedUpdate()
    {
        ManualUpdate();
    }

    public void ManualUpdate()
    {
        if (stopAnimation == true)
        {
            image = null;
            return;
        }

        animationOver = false;

        if (currentAnimation != null)
        {
            Frame currentFrame = currentAnimation.frames[animTimer];

            image.sprite = currentFrame.sprite;           


            //actual animation
            frameTimer += animationSpeed;

            if (frameTimer >= currentAnimation.frames[animTimer].duration)
            {
                animTimer++;
                frameTimer = 0;
            }

            if (animTimer >= currentAnimation.frames.Count)
            {
                lastAnimationName = currentAnimation.name;

                animTimer = 0;
                animationOver = true;
                if (AnimationOver != null) AnimationOver(currentAnimation);
            }
        }
        else
        {
            Debug.Log("Current animation == null.");
        }
    }

    public FrameAnimation GetAnimation(string animationName)
    {
        return animations.Find(x => x.name == animationName);
    }

    public void ChangeAnimation(string animationName, bool restartIfAlreadyPlaying = false)
    {
        FrameAnimation animation = GetAnimation(animationName);
        ChangeAnimation(animation, restartIfAlreadyPlaying);
    }

    public void ChangeAnimation(FrameAnimation animation, bool restartIfAlreadyPlaying = false)
    {
        if (animation != null)
        {
            if (currentAnimation != animation || restartIfAlreadyPlaying)
            {
                currentAnimation = animation;

                animTimer = 0;
                frameTimer = 0;
            }
        }
        else
        {
            Debug.Log("Animation not found.");
        }
    }
}
