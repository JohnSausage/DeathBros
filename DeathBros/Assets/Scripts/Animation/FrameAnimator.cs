using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(HurtboxManager))]
[RequireComponent(typeof(HitboxManager))]
public class FrameAnimator : _MB
{
    public FrameAnimation currentAnimation;

    public List<FrameAnimation> animations { get; protected set; }
    public FrameAnimationsSO frameAnimationsSO;

    public float animationSpeed = 1;

    private SpriteRenderer spr;
    private HurtboxManager hubM;
    private HitboxManager hibM;
    private Controller2D ctr;

    public event Action<Vector2, Vector2> SpawnProjectile;
    public event Action<FrameAnimation> AnimationOver;

    private float frameTimer;
    private int animTimer;

    public bool animationOver { get; protected set; }

    public override void Init()
    {
        base.Init();

        spr = GetComponent<SpriteRenderer>();
        hubM = GetComponent<HurtboxManager>();
        hibM = GetComponent<HitboxManager>();
        ctr = GetComponent<Controller2D>();

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
        animationOver = false;

        if (currentAnimation != null)
        {
            Frame currentFrame = currentAnimation.frames[animTimer];

            spr.sprite = currentFrame.sprite;
            hubM.SetHurtboxes(currentFrame);
            hibM.DrawHitboxes(currentFrame);

            Vector2 forceMovement = currentFrame.forceMovement;
            Vector2 addMovement = currentFrame.addMovment;

            if (spr.flipX)
            {
                forceMovement.x *= -1;
                addMovement.x *= -1;
            }
            ctr.forceMovement += forceMovement;
            ctr.addMovement = addMovement;

            if (currentFrame.spawnProjectile != Vector2.zero)
            {
                if (SpawnProjectile != null) SpawnProjectile(transform.position, currentFrame.spawnProjectile);
            }

            ctr.resetVelocity = currentFrame.resetVelocity;

            if (frameTimer == 0 && currentFrame.soundName != "")
            {
                AudioManager.PlaySound(currentFrame.soundName);
            }

            frameTimer += animationSpeed;

            if (frameTimer >= currentAnimation.frames[animTimer].duration)
            {
                animTimer++;
                frameTimer = 0;
            }

            if (animTimer >= currentAnimation.frames.Count)
            {
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
        Debug.Log(animationName);
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
