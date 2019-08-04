using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FrameAnimator))]
public class Character : _MB, ICanTakeDamage
{
    public string charName;

    [Space]

    [SerializeField]
    protected StatesAndStatsSO statesSO;
    public StatesAndStatsSO StatesSO { get { return statesSO; } }

    [Space]

    [SerializeField]
    protected StatsSO statsSO;

    [Space]

    //[SerializeField]
    //protected SoundsSO soundsSO;
    //public SoundsSO GetSoundsSO { get { return soundsSO; } }


    [Space]

    [SerializeField]
    protected List<Stat> statList;// { get; protected set; }

    public List<CardEffect> cardEffects;

    public List<Buff> Buffs { get; protected set; }
    public AttackBuff CurrentAttackBuff;// { get; set; }

    public Vector2 DirectionalInput { get; protected set; }
    public Vector2 StrongInputs { get; protected set; }
    public virtual void ClearStrongInputs() { StrongInputs = Vector2.zero; }
    public Vector2 TiltInput { get; protected set; }

    public bool Jump { get; set; }
    public bool HoldJump { get; set; }
    public bool Attack { get; set; }
    public bool HoldAttack { get; set; }
    public bool Special { get; set; }
    public bool HoldSpecial { get; protected set; }
    public bool Shield;// { get; set; }
    public bool HoldShield;// { get; set; }

    public int jumpsUsed { get; set; }
    public bool canChangeDirctionInAir { get; set; }
    public bool isRunning { get; set; }
    public bool isTakingDamage { get; set; }
    public bool isInControl { get; set; }

    [Space]

    public ChrStateMachine ChrSM;
    public int Timer { get; set; }
    public int IdleTimer { get; set; }
    public float FrozenInputX { get; set; }
    public int LandingLag { get; set; }
    public bool ChangedDirection { get; set; }
    public int HitStunDuration;// { get; set; }
    public int HitFreezeDuration { get; set; }
    public Vector2 CollisionReflectVector { get; set; }
    public Vector2 HitstunVector { get; set; }
    public Vector2 AirDodgeVector { get; set; }
    public Vector2 LaunchVector { get; set; }
    public int AirdodgeCounter { get; set; }
    public float walkSpeedReduction { get; protected set; }
    public Vector2 GetGrabbedPosition { get; protected set; }

    public FrameAnimator Anim { get; protected set; }
    public SpriteRenderer Spr { get; protected set; }
    public HitboxManager HitM { get; protected set; }
    public HurtboxManager HurtM { get; protected set; }

    public NES_BasicController2D Ctr;

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
        get { return Ctr.Position; }
    }

    [Space]
    public float currentHealth;

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
    public event Action<Damage> AGetsHit;
    public static event Action<Damage, Character> ATakesDamageAll;
    public event Action<bool> AComboOver;
    public event Action AIsDead;
    public event Action<Vector2> AIsDying;

    public event Action<Character, Damage> ACharacterTakesDasmage;

    public event Action<Character, Vector2> ASpawnProjectile;
    public event Action AIsLanding;

    public override void Init()
    {
        base.Init();

        Ctr = GetComponent<NES_BasicController2D>();

        if (GetCurrentStatValue("CanChangeDirectionInAir", false) != 0) canChangeDirctionInAir = true;

        Anim = GetComponent<FrameAnimator>();
        Anim.Init();

        Spr = GetComponent<SpriteRenderer>();
        if (Spr == null)
        {
            Spr = GetComponentInChildren<SpriteRenderer>();
        }

        HitM = GetComponent<HitboxManager>();
        HurtM = GetComponent<HurtboxManager>();


        cardEffects = new List<CardEffect>();
        Buffs = new List<Buff>();


        ChrSM = new ChrStateMachine();
        ChrSM.ChangeState(this, StaticStates.idle);

        Restart();
    }

    public virtual void Restart()
    {
        InitStats();

        isInControl = true;

        walkSpeedReduction = 1;
    }

    protected virtual void FixedUpdate()
    {
        if (AirdodgeCounter > 0)
        {
            AirdodgeCounter--;
        }

        ChrSM.Update(this);

        UpdateStats();

        UpdatesStatsForCtr();

        Anim.ManualUpdate();

        currentDamage = null;
        Jump = false;
        Attack = false;
    }

    protected virtual void UpdatesStatsForCtr()
    {
        Ctr.Movespeed = GetCurrentStatValue("Movespeed");//stats.movespeed.CurrentValue;
        Ctr.Airspeed = GetCurrentStatValue("Airspeed");
        Ctr.Gravity = GetCurrentStatValue("Gravity");//stats.gravity.CurrentValue;
    }

    public virtual void SCS_OnSpawn()
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

        for (int i = 0; i < statsSO.stats.Count; i++)
        {
            statList.Add(statsSO.stats[i].Clone());
        }

        currentHealth = GetCurrentStatValue("MaxHealth");
    }

    public void ModHealth(float value)
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

        if(damage.damageType == EDamageType.Grab)
        {
            GrabEnemy(enemy);
            enemy.GetGrabbed(this, damage.position + Vector2.right * Direction);
        }
    }

    public virtual void GetHit(Damage damage)
    {
        if (!hitIDs.Contains(damage.hitID))
        {
            hitIDs.Enqueue(damage.hitID);
            if (hitIDs.Count > 10) hitIDs.Dequeue();

            TakeDamage(damage);

            if (damage.ApplyStatMod != null)
            {
                damage.ApplyStatMod.ApplyToCharacter(this);
            }
        }
    }

    public virtual void TakeDamage(Damage damage)
    {
        if (AGetsHit != null) AGetsHit(damage);

        if (damage.damageType == EDamageType.Trigger) // e.g. used to trigger the explosion of projectiles
        {
            return;
        }

        if (dead)
        {
            return;
        }

        if (damage.damageNumber == 0)
        {
            currentDamage = damage;

            if (damage.Owner != null)
            {
                damage.Owner.HitEnemy(this, damage);
            }

            //RaiseTakeDamageEvents(damage);
        }
        else
        {
            if (shielding)
            {
                damage.damageNumber *= 0.25f;

                AudioManager.PlaySound("NES_hit1");

                currentDamage = damage;

                if (damage.Owner != null)
                {
                    damage.Owner.HitEnemy(this, damage);
                }

                ModHealth(-damage.damageNumber);

                RaiseTakeDamageEvents(damage);
            }
            else
            {
                AudioManager.PlaySound("NES_hit1");
                Flash(EffectManager.ColorHit, 3);

                currentDamage = damage;

                if (damage.Owner != null)
                {
                    damage.Owner.HitEnemy(this, damage);
                }

                // don't calculate a knockback if knockback is zero
                if (damage.baseKnockback != 0)
                {
                    currentKnockback = damage.Knockback(Position, GetCurrentStatValue("Weight"), (currentHealth / GetCurrentStatValue("MaxHealth")));
                }

                ModHealth(-damage.damageNumber);

                OnTakeDamage(damage);

                RaiseTakeDamageEvents(damage);
            }
        }
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    protected virtual void OnTakeDamage(Damage damage)
    {
        HitFreezeDuration = (int)(damage.damageNumber * 0.25f);
        HitFreezeDuration = Mathf.Clamp(HitFreezeDuration, 5, 30);

        HitStunDuration += damage.HitStunFrames(HealthPercent);
    }

    protected void RaiseTakeDamageEvents(Damage damage)
    {
        if (ATakesDamage != null) ATakesDamage(damage);
        if (ATakesDamageAll != null) ATakesDamageAll(damage, this);
        if (ACharacterTakesDasmage != null) ACharacterTakesDasmage(this, damage);
    }

    protected virtual void GrabEnemy(Character enemy)
    {
        SCS_ChangeState(StaticStates.grab);
    }

    public virtual void GetGrabbed(Character enemy, Vector2 getGrabbedPosition)
    {
        SCS_ChangeState(StaticStates.getGrabbed);
        GetGrabbedPosition = getGrabbedPosition;
    }

    public virtual void Die()
    {
        dead = true;
        SCS_ChangeState(StaticStates.die);

        if (AIsDying != null) AIsDying(transform.position);
    }

    public virtual void SCS_Dead()
    {
        dead = true;

        if (AIsDead != null) AIsDead();
    }

    public virtual void UpdateInputs()
    {

    }

    public virtual void GetInputs()
    {
        Ctr.DirectionalInput = DirectionalInput;
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

    //public CState GetState(Type type)
    //{
    //    CState returnState = null;

    //    //for (int i = 0; i < cStates.Count; i++)
    //    //{
    //    //    if (cStates[i].GetType() == type)
    //    //        returnState = cStates[i];
    //    //}
    //    return returnState;
    //}

    //public CS_Attack GetAttackState(EAttackType attackType)
    //{
    //    CS_Attack returnState = null;

    //    for (int i = 0; i < cStates.Count; i++)
    //    {
    //        if (cStates[i] is CS_Attack)
    //        {
    //            CS_Attack checkAttackType = (CS_Attack)cStates[i];

    //            if (checkAttackType.attackType == attackType)
    //            {
    //                returnState = (CS_Attack)cStates[i];
    //            }
    //        }
    //    }
    //    return returnState;
    //}

    //public virtual void CS_CheckLanding()
    //{
    //    if (Ctr.IsGrounded)
    //    {
    //        CSMachine.ChangeState(GetState(typeof(CS_Landing)));
    //    }
    //}

    //public virtual void CS_CheckIfStillGrounded()
    //{
    //    if (!Ctr.IsGrounded)
    //    {
    //        CSMachine.ChangeState(GetState(typeof(CS_Jumping)));
    //    }
    //}

    //public virtual void CS_SetIdle()
    //{
    //    CSMachine.ChangeState(GetState(typeof(CS_Idle)));
    //}

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

    //public virtual bool CheckForTiltAttacks()
    //{
    //    return false;
    //}

    //public virtual bool CheckForSoulAttacks()
    //{
    //    return false;
    //}

    //public virtual bool CheckForSpecialAttacks()
    //{
    //    return false;
    //}

    //public virtual bool CheckForAerialAttacks()
    //{
    //    return false;
    //}

    //public virtual bool CheckForThrowAttacks()
    //{
    //    return false;
    //}

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

    public virtual bool SCS_CheckForThrowAttacks()
    {
        return false;
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

    public void SCS_Idle()
    {
        ChrSM.ChangeState(this, StaticStates.idle);
    }

    public void SCS_ChangeState(SCState newState)
    {
        ChrSM.ChangeState(this, newState);
    }

    public virtual void SCS_CheckForTech()
    {

    }

    public virtual void SCS_CheckForAerialTech()
    {

    }

    public virtual void SCS_GetUpAfterHitLanded()
    {
        SCS_ChangeState(StaticStates.standUp);
    }

    public virtual void SCS_CheckForIdleOptions()
    {
        if (Mathf.Abs(DirectionalInput.x) != 0)
        {
            SCS_ChangeState(StaticStates.walking);
        }

        if (Jump)
        {
            SCS_ChangeState(StaticStates.jumpsquat);
        }
    }

    public virtual void SCS_CheckForWalkingOptions()
    {
        if (Mathf.Abs(DirectionalInput.x) == 0f || Mathf.Sign(DirectionalInput.x) != Direction)
        {
            SCS_ChangeState(StaticStates.idle);
        }

        if (Jump)
        {
            SCS_ChangeState(StaticStates.jumpsquat);
        }
    }

    public virtual void SCS_CheckForLandingOptions()
    {
        if (DirectionalInput.y < 0)
        {
            SCS_ChangeState(StaticStates.crouch);
        }
        else
        {
            if (DirectionalInput.x == 0)
            {
                SCS_ChangeState(StaticStates.idle);
            }
            else
            {
                SCS_ChangeState(StaticStates.walking);
            }
        }
    }

    public virtual void RaiseSpawnProjectileEvent(Character chr, Vector2 position)
    {
        if (ASpawnProjectile != null) ASpawnProjectile(this, position);
    }

    public virtual void SCS_SpawnProjetile(NES_Projectile projectile, Vector2 spawnOffset)
    {
        Vector2 localSpawnOffset = new Vector2(spawnOffset.x * Direction, spawnOffset.y);

        NES_Projectile proj = Instantiate(projectile.gameObject, Position + localSpawnOffset, Quaternion.identity).GetComponent<NES_Projectile>();
        proj.Velocity = new Vector2(proj.Velocity.x * Direction, proj.Velocity.y);
        proj.SetOwner(this);
        proj.GetComponent<SpriteRenderer>().flipX = Spr.flipX; // Otherwise damge direction is wrong
    }

    public virtual void SCS_RaiseLandingEvent()
    {
        AirdodgeCounter = 0;

        if (AIsLanding != null) AIsLanding();
    }

    public virtual void SCS_CountSpecial(ESpecial type)
    {

    }

    public virtual Damage GetModifiedDamage(Damage damage)
    {
        return damage;
    }
}

public interface ICanTakeDamage
{
    void GetHit(Damage damage);
}

public enum EAttackType { Jab1, FTilt, DTilt, UTilt, DashAtk, NAir, FAir, DAir, UAir, BAir, FSoul, DSoul, USoul, Jab2, NSpec, DSpec, USpec, FSpec, None, Item, Hazard, Grab }