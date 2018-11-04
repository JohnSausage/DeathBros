﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public Stat wallSlideSpeed = new Stat("WallslideSpeed", 5);

    public CStates_AdvancedMovement advancedMovementStates;
    public CStates_Attack attackStates;

    public bool Special { get; protected set; }
    public bool HoldSpecial { get; protected set; }

    public float soulCharge = 0;

    protected int maxSouls = 3;
    protected int currentSouls;
    public int CurrentSouls { get { return currentSouls; } }

    [SerializeField]
    protected int soulBalanceDelayDuration = 60;
    private int soulBalanceDelayTimer = 0;

    public float pickUpRadius = 1.5f;

    public LayerMask enemyMask;

    public Item holdItem { get; protected set; }
    public bool hasItem { get { return holdItem != null; } }

    public float SoulPercent { get { return soulMeter / soulMeterMax; } }

    public static event Action<float> PlayerHealthChanged;
    public static event Action<Character, Damage> EnemyHit;
    public event Action<int> ESoulsChanged;

    public override void Init()
    {
        base.Init();
        soundFolderName = "Sounds/Player/";

        advancedMovementStates.Init(this);
        attackStates.Init(this);

        stats.AddStat(wallSlideSpeed);

        CStates_InitExitStates();

        ComboCounter.ComboIsOver += AddHealthAfterCombo;

        currentSouls = maxSouls;
    }

    void Update()
    {
        DirectionalInput = InputManager.Direction;
        StrongInputs = InputManager.Smash;
        TiltInput = InputManager.CStick;

        if (Mathf.Abs(DirectionalInput.x) < 0.2f) DirectionalInput = new Vector2(0, DirectionalInput.y);

        if (InputManager.BufferdDown("Attack")) Attack = true;
        else Attack = false;


        if (InputManager.Attack.GetButton()) HoldAttack = true;
        else HoldAttack = false;

        if (InputManager.BufferdDown("Special")) Special = true;
        else Special = false;


        if (InputManager.Special.GetButton()) HoldSpecial = true;
        else HoldSpecial = false;

        if (InputManager.BufferdDown("Jump") || StrongInputs.y > 0) Jump = true;
        else
        {
            //reset at the end of FixedUpdate to not miss any inputs
            //Jump = false;
        }

        if (InputManager.Jump.GetButton()) HoldJump = true;
        else HoldJump = false;


        if ((InputManager.BufferdDown("Shield"))) Shield = true;
        else Shield = false;

        if (InputManager.Shield.GetButton()) HoldShield = true;
        else HoldShield = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        BalanceSoulMeter();
    }

    protected void BalanceSoulMeter()
    {
        if (soulBalanceDelayTimer > 0) soulBalanceDelayTimer--;

        if (soulBalanceDelayTimer == 0)
        {
            if (soulMeter > 50)
            {
                soulMeter -= soulMeterBalanceRate;

                if (soulMeter < 50) soulMeter = 50;
            }
            else
            {
                soulMeter += soulMeterBalanceRate;

                if (soulMeter > 50) soulMeter = 50;
            }

            soulMeter = Mathf.Clamp(soulMeter, 0, 100);
        }
    }

    public override void HitEnemy(Character enemy, Damage damage)
    {
        base.HitEnemy(enemy, damage);

        if (EnemyHit != null) EnemyHit(enemy, damage);
    }

    private void AddHealthAfterCombo(float damageNumber)
    {
        stats.currentHealth += damageNumber;

        if (stats.currentHealth > stats.maxHealth.CurrentValue)
            stats.currentHealth = stats.maxHealth.CurrentValue;

        if (PlayerHealthChanged != null) PlayerHealthChanged(stats.currentHealth / stats.maxHealth.CurrentValue);
    }

    public override void GetHit(Damage damage)
    {
        base.GetHit(damage);

        //if (PlayerHealthChanged != null) PlayerHealthChanged(stats.currentHealth / stats.maxHealth.CurrentValue);
        if (PlayerHealthChanged != null) PlayerHealthChanged(soulMeter / soulMeterMax);
    }

    protected override void TakeDamage(Damage damage)
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

                //stats.currentHealth -= damage.damageNumber;
                ModSoulMeter(-damage.damageNumber);
            }
            if (stats.currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public override void ModSoulMeter(float value)
    {
        soulBalanceDelayTimer = soulBalanceDelayDuration;

        soulMeter += value;

        if (soulMeter <= 0)
        {
            currentSouls--;
            soulMeter = soulMeterMax;

            if (ESoulsChanged != null) ESoulsChanged(currentSouls);

            if (currentSouls <= 0)
            {
                Die();
            }
        }
    }


    protected bool CheckForItemPickUp()
    {
        if (holdItem == null)
        {
            RaycastHit2D itemCheck = Physics2D.CircleCast(transform.position, pickUpRadius, Vector2.zero, 0, enemyMask);

            if (itemCheck)
            {
                Item pickedItem = itemCheck.transform.GetComponentInParent<Item>();
                if (pickedItem != null)
                {
                    InputManager.ClearBuffer();
                    SetHoldItem(pickedItem);
                    return true;
                }
            }
        }

        return false;
    }

    public bool ThrowItem(Vector2 throwVelocity)
    {
        if (holdItem != null)
        {
            Vector2 itemVelocity;

            if (throwVelocity == Vector2.zero)
            {
                itemVelocity = Vector2.zero;
            }
            else if (Mathf.Abs(throwVelocity.x) > Mathf.Abs(throwVelocity.y))
            {
                itemVelocity = new Vector2(Mathf.Sign(throwVelocity.x), 0.25f);
            }
            else
            {
                itemVelocity = new Vector2(0, Mathf.Sign(throwVelocity.y));
            }

            holdItem.Velocity = itemVelocity.normalized * 25f;
            ReleaseHoldItem();

            InputManager.ClearBuffer();
            CSMachine.ChangeState(advancedMovementStates.throwItem);
            return true;
        }
        return false;
    }

    public void SetHoldItem(Item item)
    {
        holdItem = item;
        holdItem.IsSimulated = false;
        holdItem.Owner = this;
        holdItem.transform.SetParent(transform);
        holdItem.transform.localPosition = Vector3.zero;
    }

    public void ReleaseHoldItem()
    {
        if (hasItem)
        {
            holdItem.transform.SetParent(null);
            holdItem.IsSimulated = true;
            holdItem.GenerateID();
            holdItem = null;
        }
    }

    public override bool CheckForTiltAttacks()
    {
        if (Attack)
        {
            if (ThrowItem(DirectionalInput))
            {
                return true;
            }
            else if (CheckForItemPickUp())
            {
                return true;
            }
            else
            {
                if (DirectionalInput == Vector2.zero)
                {
                    CSMachine.ChangeState(GetAttackState(EAttackType.Jab1));
                }
                else if (Mathf.Abs(DirectionalInput.x) > 0.5f)
                {
                    CSMachine.ChangeState(GetAttackState(EAttackType.FTilt));
                }
                else if (DirectionalInput.y > 0.5f)
                {
                    CSMachine.ChangeState(GetAttackState(EAttackType.UTilt));
                }
                else if (DirectionalInput.y < -0.5f)
                {
                    CSMachine.ChangeState(GetAttackState(EAttackType.DTilt));
                }
            }

            return true;
        }

        if (TiltInput != Vector2.zero)
        {
            if (!ThrowItem(TiltInput))
            {
                if (Mathf.Abs(TiltInput.x) > 0.5f)
                {
                    Direction = TiltInput.x;

                    CSMachine.ChangeState(GetAttackState(EAttackType.FTilt));
                }
                else if (TiltInput.y > 0.5f)
                {
                    CSMachine.ChangeState(GetAttackState(EAttackType.UTilt));
                }
                else if (TiltInput.y < -0.5f)
                {
                    CSMachine.ChangeState(GetAttackState(EAttackType.DTilt));
                }
            }
            return true;
        }


        return false;
    }

    public override bool CheckForSoulAttacks()
    {
        Vector2 smash = InputManager.Smash;

        if (Attack && smash != Vector2.zero)
        {
            if (ThrowItem(smash)) return true;


            if (Mathf.Abs(smash.x) > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FSoul));
            }
            else if (smash.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.USoul));
            }
            else if (smash.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DSoul));
            }

            return true;
        }

        else return false;
    }

    public override bool CheckForAerialAttacks()
    {
        if (Attack)
        {
            if (CheckForItemPickUp()) return true;
            if (ThrowItem(DirectionalInput)) return true;

            if (DirectionalInput == Vector2.zero)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.NAir));
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f && Mathf.Sign(DirectionalInput.x) == Direction)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FAir));
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f && Mathf.Sign(DirectionalInput.x) != Direction)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.BAir));
            }
            else if (DirectionalInput.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.UAir));
            }
            else if (DirectionalInput.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DAir));
            }

            return true;
        }

        if (TiltInput != Vector2.zero)
        {
            if (CheckForItemPickUp()) return true;
            if (ThrowItem(TiltInput)) return true;

            if (Mathf.Abs(TiltInput.x) > 0.5f && Mathf.Sign(TiltInput.x) == Direction)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FAir));
            }
            else if (Mathf.Abs(TiltInput.x) > 0.5f && Mathf.Sign(TiltInput.x) != Direction)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.BAir));
            }
            else if (TiltInput.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.UAir));
            }
            else if (TiltInput.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DAir));
            }

            return true;
        }

        if (CheckForSpecialAttacks()) return true;

        return false;
    }

    public override bool CheckForSpecialAttacks()
    {
        if (Special)
        {
            if (DirectionalInput == Vector2.zero)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.NSpec));
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.FSpec));
            }
            else if (DirectionalInput.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.USpec));
            }
            else if (DirectionalInput.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DSpec));
            }
            return true;
        }
        return false;
    }
}
