using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FrameAnimator))]
public class Character : _MB, ICanTakeDamage
{
    public string charName;

    //public string soundFolderName;

    [Space]

    [SerializeField]
    protected StatesAndStatsSO statesSO;
    public StatesAndStatsSO StatesSO { get { return statesSO; } }

    public StatesAndStatsPlayerSO PlayerStatesSO { get { return (StatesAndStatsPlayerSO)statesSO; } }

    [SerializeField]
    protected List<CS_AttackSO> attackSOs;

    [Space]

    [SerializeField]
    protected StatsSO statsSO;

    [Space]

    [SerializeField]
    protected SoundsSO soundsSO;
    public SoundsSO GetSoundsSO { get { return soundsSO; } }


    //[SerializeField]
    //protected CStateParamtetersSO cStateParamtetersSO;
    //public CStateParamtetersSO CStateParamtetersSO { get { return cStateParamtetersSO; } }

    //public int csTimer { get; set; }
    //public float csDirection { get; set; }

    [Space]

    [SerializeField]
    protected List<Stat> statList;// { get; protected set; }

    public List<CardEffect> cardEffects;

    public List<Buff> Buffs { get; protected set; }

    public Vector2 DirectionalInput { get; protected set; }
    public Vector2 StrongInputs { get; protected set; }
    public virtual void ClearStrongInputs() { StrongInputs = Vector2.zero; }
    public Vector2 TiltInput { get; protected set; }

    public bool Jump { get; set; }
    public bool HoldJump { get; protected set; }
    public bool Attack { get; set; }
    public bool HoldAttack { get; set; }
    public bool Special { get; set; }
    public bool HoldSpecial { get; protected set; }
    public bool Shield { get; set; }
    public bool HoldShield { get; protected set; }

    //public float jumpStrength = 20;
    //public int jumps = 2;
    public int jumpsUsed { get; set; }
    public bool canChangeDirctionInAir { get; set; }

    [Space]

    public ChrStateMachine ChrSM;
    public int Timer { get; set; }
    public int IdleTimer { get; set; }
    public float FrozenInputX { get; set; }
    public int LandingLag { get; set; }
    public bool ChangedDirection { get; set; }

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

    public Vector2 Position
    {
        //get { return (Vector2)transform.position; }
        get { return (Vector2)Ctr.Col.bounds.center; }
    }

    public List<CState> cStates;// { get; protected set; }

    [Space]

    //public Stats stats;

    public float currentHealth;
    public float currentSouls = 1;
    public float soulMeter = 50;
    public float soulMeterMax = 100;
    public float soulMeterBalanceRate = 0.25f;

    public Damage currentDamage { get; protected set; }
    public Vector2 currentKnockback { get; protected set; }
    public EAttackType currentAttackType { get; set; }
    public float HealthPercent { get { return (currentHealth / GetCurrentStatValue("MaxHealth")); } }

    public bool shielding = false;
    public bool IsDead { get { return (currentHealth <= 0); } }
    public bool dead { get; protected set; }

    public Queue<int> hitIDs = new Queue<int>();

    public event Action<Character, Damage> AEnemyHit;
    public event Action<Damage> ATakesDamage;
    public static event Action<Damage, Character> ATakesDamageAll;
    public event Action<bool> AComboOver;


    public override void Init()
    {
        base.Init();

        InitStats();

        if (GetCurrentStatValue("CanChangeDirectionInAir", false) != 0) canChangeDirctionInAir = true;



        cStates = new List<CState>();

        CSMachine = new StateMachine();
        Anim = GetComponent<FrameAnimator>();
        Anim.Init();

        Spr = GetComponent<SpriteRenderer>();
        if (Spr == null)
        {
            Spr = GetComponentInChildren<SpriteRenderer>();
        }

        Ctr = GetComponent<Controller2D>();

        HitM = GetComponent<HitboxManager>();
        HurtM = GetComponent<HurtboxManager>();


        statesSO.InitStates(this);

        CStates_InitExitStates();

        foreach (var attackSO in attackSOs)
        {
            attackSO.InitState(this);
        }

        cardEffects = new List<CardEffect>();

        Buffs = new List<Buff>();

        ChrSM = new ChrStateMachine();
        ChrSM.ChangeState(this, StaticStates.idle);
    }

    public override void LateInit()
    {
        base.LateInit();

        //InitStats();

        if (soundsSO != null) soundsSO.LoadSounds();

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
        ChrSM.Update(this);

        //CSMachine.Update();

        //stats.FixedUpdate();

        UpdateStats();

        UpdatesStatsForCtr();
        Ctr.ManualFixedUpdate();

        currentDamage = null;
        Jump = false;

        Attack = false;
    }

    protected virtual void UpdatesStatsForCtr()
    {
        Ctr.movespeed = GetCurrentStatValue("Movespeed");//stats.movespeed.CurrentValue;
        Ctr.gravity = GetCurrentStatValue("Gravity");//stats.gravity.CurrentValue;
    }

    public void Spawn(Vector2 position)
    {

    }

    public void RaiseComboOverEvent()
    {
        if (AComboOver != null) AComboOver(true);
    }

    #region stats

    protected virtual void InitStats()
    {
        statList = new List<Stat>();

        //statsSO.Init(statList);

        for (int i = 0; i < statsSO.stats.Count; i++)
        {
            statList.Add(statsSO.stats[i].Clone());
        }

        currentHealth = GetCurrentStatValue("MaxHealth");
    }

    public virtual void ModSouls(float value)
    {
        currentSouls += value;

        currentSouls = Mathf.Clamp(currentSouls, 0, GetCurrentStatValue("MaxSouls"));
    }

    protected void ModHealth(float value)
    {
        currentHealth += value;

        currentHealth = Mathf.Clamp(currentHealth, 0, GetCurrentStatValue("MaxHealth"));
    }

    protected void UpdateStats()
    {
        for (int i = 0; i < statList.Count; i++)
        {
            statList[i].FixedUpdate();
        }
    }

    public Stat GetStat(string statName)
    {
        return statList.Find(x => x.statName == statName);
    }

    public float GetCurrentStatValue(string statName, bool getDebugMessage = true)
    {
        Stat stat = GetStat(statName);

        if (stat != null) return stat.CurrentValue;

        else
        {
            if (getDebugMessage) Debug.Log(statName + " not found");

            return 0;
        }
    }

    #endregion

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
        if (AEnemyHit != null) AEnemyHit(enemy, damage);
    }

    public virtual void GetHit(Damage damage)
    {
        if (!hitIDs.Contains(damage.hitID))
        {
            hitIDs.Enqueue(damage.hitID);
            if (hitIDs.Count > 10) hitIDs.Dequeue();

            TakeDamage(damage);

            if (damage.ApplyStatMod != null)
                damage.ApplyStatMod.ApplyToCharacter(this);
        }
    }

    protected virtual void TakeDamage(Damage damage)
    {
        if (!dead)
        {
            if (shielding)
            {
                damage.damageNumber *= 0.25f;

                RaiseTakeDamageEvents(damage);

                AudioManager.PlaySound("NES_hit1");

                currentDamage = damage;

                if (damage.Owner != null)
                {
                    damage.Owner.HitEnemy(this, damage);
                }

                currentHealth -= damage.damageNumber;
            }
            else
            {
                RaiseTakeDamageEvents(damage);

                AudioManager.PlaySound("NES_hit1");
                Flash(EffectManager.ColorHit, 3);

                currentDamage = damage;

                if (damage.Owner != null)
                {
                    damage.Owner.HitEnemy(this, damage);
                }

                currentKnockback = damage.Knockback(transform.position, GetCurrentStatValue("Weight"), (currentHealth / GetCurrentStatValue("MaxHealth")));

                currentHealth -= damage.damageNumber;
            }
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    protected void RaiseTakeDamageEvents(Damage damage)
    {
        if (ATakesDamage != null) ATakesDamage(damage);
        if (ATakesDamageAll != null) ATakesDamageAll(damage, this);
    }

    public virtual void Die()
    {
        //Debug.Log(this.name + " died");

        CSMachine.ChangeState(GetState(typeof(CS_Die)));
    }

    public virtual void Dead()
    {
        dead = true;
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

    //public void LoadSoundFiles()
    //{
    //    if (soundFolderName != "")
    //    {
    //        UnityEngine.Object[] objects = Resources.LoadAll(soundFolderName);
    //
    //        foreach (var o in objects)
    //        {
    //            Sound s = new Sound();
    //            s.clip = (AudioClip)o;
    //            s.name = o.name;
    //            AudioManager.AddSound((s));
    //        }
    //    }
    //}

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

    public virtual bool CheckForThrowAttacks()
    {
        return false;
    }

    public virtual void ModSoulMeter(float value)
    {
        soulMeter += value;

        soulMeter = Mathf.Clamp(soulMeter, 0, 100);
    }

    public void Flash(Color color, int durationInFrames)
    {
        StartCoroutine(CFlash(color, durationInFrames));
    }

    private IEnumerator CFlash(Color color, int durationFrames)
    {
        Material oldMaterial = Spr.material;

        Spr.material = EffectManager.ColorShaderMaterial;
        Spr.material.color = color;

        yield return new WaitForSeconds(durationFrames / 60f);

        Spr.material = EffectManager.DefaultMaterial;
    }
    /*
    public float GetCardEffect_DamageMultiplier(EAttackType attackType)
    {
        float multi = 1f;

        for (int i = 0; i < cardEffects.Count; i++)
        {
            if (cardEffects[i].GetType() == typeof(CardEffect_SingleAttackStrength))
            {
                CardEffect_SingleAttackStrength effect = (CardEffect_SingleAttackStrength)cardEffects[i];
                if (attackType == effect.attackType)
                    multi *= effect.damageMultiplier;
            }

            else if (cardEffects[i].GetType() == typeof(CardEffect_AllAttackStrength))
            {
                CardEffect_AllAttackStrength effect = (CardEffect_AllAttackStrength)cardEffects[i];
                if (Damage.CheckIfDamageApplies(attackType, effect.attackClass))
                    multi *= effect.damageMultiplier;
            }
        }

        return multi;
    }
    */

    public virtual void SCS_CheckForAerials()
    {

    }

    public virtual void SCS_CheckForGroundAttacks()
    {

    }

    public virtual void SCS_CheckIfGrounded()
    {
        if (Ctr.IsGrounded == false)
        {
            ChrSM.ChangeState(this, StaticStates.jumping);
        }
    }

    public virtual void SCS_CheckIfLanding()
    {
        if (Ctr.IsGrounded == true)
        {
            ChrSM.ChangeState(this, StaticStates.landing);
        }
    }
}

public interface ICanTakeDamage
{
    void GetHit(Damage damage);
}