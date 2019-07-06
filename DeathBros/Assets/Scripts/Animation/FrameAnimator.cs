using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimator : _MB
{
    [SerializeField]
    protected bool UpdateAutomatically = true;

    public FrameAnimation currentAnimation;

    public List<FrameAnimation> animations { get; protected set; }
    public FrameAnimationsSO frameAnimationsSO;

    public float animationSpeed = 1;

    public SpriteRenderer Spr { get; protected set; }
    private HurtboxManager hubM;
    private HitboxManager hibM;
    private NES_BasicController2D ctr;

    public event Action<Vector2, Vector2> SpawnProjectile;
    public event Action<FrameAnimation> AnimationOver;

    private float frameTimer;
    private int animTimer;

    public bool animationOver { get; protected set; }
    public string lastAnimationName { get; protected set; }
    public bool stopAnimation { get; set; }

    public override void Init()
    {
        base.Init();

        Spr = GetComponent<SpriteRenderer>();
        if (Spr == null)
        {
            Spr = GetComponentInChildren<SpriteRenderer>();
        }

        hubM = GetComponent<HurtboxManager>();
        hibM = GetComponent<HitboxManager>();
        ctr = GetComponent<NES_BasicController2D>();

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
        if (UpdateAutomatically)
        {
            ManualUpdate();
        }
    }

    public void ManualUpdate()
    {
        if(stopAnimation == true)
        {
            Spr.sprite = null;
            return;
        }

        animationOver = false;

        if (currentAnimation != null)
        {
            Frame currentFrame = currentAnimation.frames[animTimer];

            Spr.sprite = currentFrame.sprite;

            // set Hurtboxes
            if (hubM != null)
            {
                hubM.SetHurtboxes(currentFrame);
            }

            // set Hitboxes
            if (hibM != null)
            {
                hibM.DrawHitboxes(currentFrame);
            }

            // set Controller movement
            if (ctr != null)
            {
                Vector2 forceMovement = currentFrame.forceMovement;
                Vector2 addMovement = currentFrame.addMovment;

                if (Spr.flipX)
                {
                    forceMovement.x *= -1;
                    addMovement.x *= -1;
                }
                ctr.ForceMovement += forceMovement;
                ctr.AddMovement = addMovement;

                if (currentFrame.spawnProjectile != Vector2.zero)
                {
                    Character chr = GetComponent<Character>();
                    if (chr != null)
                    {
                        chr.RaiseSpawnProjectileEvent(chr, currentFrame.spawnProjectile);
                    }


                    if (SpawnProjectile != null) SpawnProjectile(transform.position, currentFrame.spawnProjectile);
                }

                ctr.ResetVelocity = currentFrame.resetVelocity;
            }

            //set spawn items
            if (currentFrame.spawnHoldItem != null)
            {
                Player player = GetComponent<Player>();
                if (player != null)
                {
                    GameObject holdItem = Instantiate(currentFrame.spawnHoldItem, transform);
                    Item pickUpItem = holdItem.GetComponent<Item>();

                    if (pickUpItem is ICanBePickedUp)
                    {

                        //player.ReleaseHoldItem();
                        //player.SetHoldItem(pickUpItem);
                    }

                }
                else
                {
                    GameObject spawnItem = Instantiate(currentFrame.spawnHoldItem, transform.position, Quaternion.identity);
                    DamagingCollider damagingCollider = spawnItem.GetComponent<DamagingCollider>();

                    if (damagingCollider == null) return;

                    damagingCollider.damage.Owner = player;
                    damagingCollider.damage.attackType = EAttackType.Item;
                }
            }

            // set sound
            if (frameTimer == 0 && currentFrame.soundName != "")
            {
                AudioManager.PlaySound(currentFrame.soundName, Spr.transform.position);
            }


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
