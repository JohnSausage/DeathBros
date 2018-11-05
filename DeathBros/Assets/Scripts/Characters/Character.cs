using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FrameAnimator))]
public class Character : _MB, ICanTakeDamage
{
    public string charName;

    public string soundFolderName;

    public Vector2 DirectionalInput { get; protected set; }
    public Vector2 StrongInputs { get; protected set; }
    public Vector2 TiltInput { get; protected set; }

    public bool Jump { get; protected set; }
    public bool HoldJump { get; protected set; }
    public bool Attack { get; protected set; }
    public bool HoldAttack { get; protected set; }
    public bool Special { get; protected set; }
    public bool HoldSpecial { get; protected set; }
    public bool Shield { get; protected set; }
    public bool HoldShield { get; protected set; }

    //public float jumpStrength = 20;
    //public int jumps = 2;
    public int jumpsUsed { get; set; }

    [Space]

    public StateMachine CSMachine;// { get; protected set; }
    public FrameAnimator Anim { get; protected set; }
    public SpriteRenderer Spr { get; protected set; }
    public Controller2D Ctr { get; protected set; }
    public HitboxManager HitM { get; protected set; }
    public HurtboxManager HurtM { get; protected set; }

    public float Direction
    {
        get { return Spr.flipX ? -1 : 1; }
        set
        {
            Ctr.FaceDirection = Mathf.Sign(value);
            Spr.flipX = (Mathf.Sign(value) == -1);
        }
    }

    public List<CState> cStates;

    [Space]

    public Stats stats;

    public float soulMeter = 50;
    public float soulMeterMax = 100;
    public float soulMeterBalanceRate = 0.25f;

    public Damage currentDamage { get; protected set; }
    public Vector2 currentKnockback { get; protected set; }

    public bool shielding = false;
    public bool IsDead { get { return (stats.currentHealth <= 0); } }

    public Queue<int> hitIDs = new Queue<int>();

    public event Action<Damage> TakesDamage;
    public static event Action<Damage, Vector2> TakesDamageAll;

    public override void Init()
    {
        base.Init();

        CSMachine = new StateMachine();
        Anim = GetComponent<FrameAnimator>();
        Anim.Init();

        Spr = GetComponent<SpriteRenderer>();
        Ctr = GetComponent<Controller2D>();

        HitM = GetComponent<HitboxManager>();
        HurtM = GetComponent<HurtboxManager>();

        stats.Init();
    }

    public virtual void CStates_InitExitStates()
    {
        foreach (CState cs in cStates)
        {
            cs.InitExitStates();
        }
    }

    protected virtual void FixedUpdate()
    {
        CSMachine.Update();

        stats.FixedUpdate();

        UpdatesStatsForCtr();
        Ctr.ManualFixedUpdate();

        currentDamage = null;
        Jump = false;
        Attack = false;
    }

    protected virtual void UpdatesStatsForCtr()
    {
        Ctr.movespeed = stats.movespeed.CurrentValue;
        Ctr.gravity = stats.gravity.CurrentValue;
    }

    public void Spawn(Vector2 position)
    {

    }

    public void AddHitIDToQueue(int id)
    {
        hitIDs.Enqueue(id);

        if (hitIDs.Count > 10)
        {
            hitIDs.Dequeue();
        }
    }

    public bool ContainsHidID(int id)
    {
        return hitIDs.Contains(id);
    }

    public virtual void HitEnemy(Character enemy, Damage damage)
    {

    }

    public virtual void GetHit(Damage damage)
    {
        if (!hitIDs.Contains(damage.hitID))
        {
            hitIDs.Enqueue(damage.hitID);
            if (hitIDs.Count > 10) hitIDs.Dequeue();

            TakeDamage(damage);
        }
    }

    protected virtual void TakeDamage(Damage damage)
    {
        if (!IsDead)
        {
            if (!shielding)
            {
                RaiseTakeDamageEvents(damage);

                AudioManager.PlaySound("hit1");

                currentDamage = damage;

                if (damage.Owner != null)
                {
                    damage.Owner.HitEnemy(this, damage);
                }

                currentKnockback = damage.Knockback(transform.position, stats.weight.CurrentValue, (stats.currentHealth / stats.maxHealth.CurrentValue));

                stats.currentHealth -= damage.damageNumber;
            }
            if (stats.currentHealth <= 0)
            {
                Die();
            }
        }
    }

    protected void RaiseTakeDamageEvents(Damage damage)
    {
        if (TakesDamage != null) TakesDamage(damage);
        if (TakesDamageAll != null) TakesDamageAll(damage, transform.position);
    }

    public virtual void Die()
    {
        Debug.Log(this.name + " died");

        CSMachine.ChangeState(GetState(typeof(CS_Die)));
    }

    public virtual void Dead()
    {

    }

    public virtual void GetInputs()
    {
        Ctr.input = DirectionalInput;
    }

    public virtual void SetInputs(Vector2 inputs)
    {
        DirectionalInput = inputs;
        GetInputs();
    }

    public virtual void ModInputs(float multiply)
    {
        DirectionalInput *= multiply;
        GetInputs();
    }

    public virtual void SetAttack(bool value)
    {
        Attack = value;
    }

    public CState GetState(Type type)
    {
        CState returnState = null;

        for (int i = 0; i < cStates.Count; i++)
        {
            if (cStates[i].GetType() == type)
                returnState = cStates[i];
        }
        return returnState;
    }

    public CS_Attack GetAttackState(EAttackType attackType)
    {
        CS_Attack returnState = null;

        for (int i = 0; i < cStates.Count; i++)
        {
            if (cStates[i] is CS_Attack)
            {
                CS_Attack checkAttackType = (CS_Attack)cStates[i];

                if (checkAttackType.attackType == attackType)
                {
                    returnState = (CS_Attack)cStates[i];
                }
            }
        }
        return returnState;
    }

    public virtual void CS_CheckLanding()
    {
        if (Ctr.IsGrounded)
        {
            CSMachine.ChangeState(GetState(typeof(CS_Landing)));
        }
    }

    public virtual void CS_CheckIfStillGrounded()
    {
        if (!Ctr.IsGrounded)
        {
            CSMachine.ChangeState(GetState(typeof(CS_Jumping)));
        }
    }

    public virtual void CS_SetIdle()
    {
        CSMachine.ChangeState(GetState(typeof(CS_Idle)));
    }

    public void LoadSoundFiles()
    {
        if (soundFolderName != "")
        {
            UnityEngine.Object[] objects = Resources.LoadAll(soundFolderName);

            foreach (var o in objects)
            {
                Sound s = new Sound();
                s.clip = (AudioClip)o;
                s.name = o.name;
                AudioManager.AddSound((s));
            }
        }
    }

    public virtual bool CheckForTiltAttacks()
    {
        return false;
    }

    public virtual bool CheckForSoulAttacks()
    {
        return false;
    }

    public virtual bool CheckForSpecialAttacks()
    {
        return false;
    }

    public virtual bool CheckForAerialAttacks()
    {
        return false;
    }

    public virtual void ModSoulMeter(float value)
    {
        soulMeter += value;

        soulMeter = Mathf.Clamp(soulMeter, 0, 100);
    }
}

public interface ICanTakeDamage
{
    void GetHit(Damage damage);
}