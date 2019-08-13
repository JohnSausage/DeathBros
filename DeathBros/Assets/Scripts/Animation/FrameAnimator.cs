using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimator : _MB
{
    [SerializeField]
    protected bool UpdateAutomatically = true;

    public FrameAnimation currentAnimation;

    public List<FrameAnimation> animations;// { get; protected set; }
    public FrameAnimationsSO frameAnimationsSO;

    public float animationSpeed = 1;

    public SpriteRenderer Spr { get; protected set; }
    private HurtboxManager hubM;
    private HitboxManager hibM;
    private NES_BasicController2D ctr;
    private SpriteColorChanger sprCol;

    [SerializeField]
    protected List<Texture2D> texture2Ds;
    [SerializeField]
    protected List<Texture2D> coloredTexture2Ds;

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

        sprCol = GetComponent<SpriteColorChanger>();

        if (sprCol != null)
        {
            foreach (FrameAnimation anim in frameAnimationsSO.frameAnimations)
            {
                if (anim.frames.Count == 0)
                {
                    break;
                }

                if (anim.frames[0].sprite == null)
                {
                    break;
                }

                if (texture2Ds.Contains(anim.frames[0].sprite.texture) == false)
                {
                    texture2Ds.Add(anim.frames[0].sprite.texture);
                }
            }

            foreach (Texture2D tex in texture2Ds)
            {
                Texture2D coloredTexture = sprCol.GetColoredSprite(tex);

                coloredTexture2Ds.Add(coloredTexture);
            }
        }

        frameTimer = 0;
        animTimer = 0;

        //if (animations[0] != null)
        //    ChangeAnimation(animations[0]);

        if (frameAnimationsSO != null)
        {
            animations = new List<FrameAnimation>();

            foreach (FrameAnimation originalAnim in frameAnimationsSO.frameAnimations)
            {
                int textureIndex = 0;
                //looks thorugh the original textures to get the index of the coloredTextures
                if (texture2Ds.Contains(originalAnim.frames[0].sprite.texture))
                {
                    textureIndex = texture2Ds.IndexOf(originalAnim.frames[0].sprite.texture);
                }

                FrameAnimation frameAnimation = Instantiate(originalAnim);

                frameAnimation.name = originalAnim.name;

                if (sprCol != null)
                {

                    for (int i = 0; i < frameAnimation.frames.Count; i++)
                    {
                        string tempName = originalAnim.frames[i].sprite.name;
                        frameAnimation.frames[i].sprite = Sprite.Create(coloredTexture2Ds[textureIndex], originalAnim.frames[i].sprite.rect, new Vector2(0.5f, 0.5f), 16);
                        frameAnimation.frames[i].sprite.name = tempName;
                    }

                }

                animations.Add(frameAnimation);
            }


            //load first animation
            ChangeAnimation(animations[0]);
        }
    }

    public void ChangeFrameAnimationColor(string animationName, Color color)
    {

        if (sprCol == null)
        {
            return;
        }

        sprCol.Color1 = color;

        


        FrameAnimation frameAnimation = animations.Find(x => x.name == animationName);

        if (frameAnimation == null)
        {
            return;
        }

        FrameAnimation originalAnim = frameAnimationsSO.frameAnimations.Find(x => x.name == frameAnimation.name);

        if(originalAnim == null)
        {
            return;
        }

        int textureIndex = 0;

        //looks thorugh the original textures to get the index of the coloredTextures
        if (texture2Ds.Contains(originalAnim.frames[0].sprite.texture))
        {
            textureIndex = texture2Ds.IndexOf(originalAnim.frames[0].sprite.texture);
        }

        coloredTexture2Ds[textureIndex] = sprCol.GetColoredSprite(texture2Ds[textureIndex]);

        //frameAnimation.frames.Clear();

        for (int i = 0; i < originalAnim.frames.Count; i++)
            {
                string tempName = originalAnim.frames[i].sprite.name;
                frameAnimation.frames[i].sprite = Sprite.Create(coloredTexture2Ds[textureIndex], originalAnim.frames[i].sprite.rect, new Vector2(0.5f, 0.5f), 16);
                frameAnimation.frames[i].sprite.name = tempName;
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
        if (stopAnimation == true)
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

                if (currentAnimation.loop == true)
                {
                    animTimer = 0;
                }
                else
                {
                    animTimer = currentAnimation.frames.Count - 1;
                }

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
