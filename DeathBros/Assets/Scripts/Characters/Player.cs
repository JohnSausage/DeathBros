using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    //public Stat wallSlideSpeed = new Stat("WallslideSpeed", 5);

    public CStates_AdvancedMovement advancedMovementStates;
    public CStates_Attack attackStates;

    public float soulCharge = 0;

    //public Stat maxSouls;
    //public Stat currentSouls;
    //protected int maxSouls = 3;
    //protected int currentSouls;
    //public int CurrentSouls { get { return (int)stats.FindStat("currentSouls").CurrentValue; } }

    [SerializeField]
    protected int soulBalanceDelayDuration = 60;
    private int soulBalanceDelayTimer = 0;

    public int soulBank = 0;

    public float pickUpRadius = 1.5f;

    public LayerMask enemyMask;

    public bool Grab { get; set; }

    public Item holdItem { get; protected set; }
    public bool hasItem { get { return holdItem != null; } }

    public float SoulPercent { get { return soulMeter / soulMeterMax; } }

    public static event Action<float> PlayerHealthChanged;
    public static event Action<Character, Damage> EnemyHit;
    public event Action<float> ASoulsChanged;
    public event Action<int> ASoulBankPlus;
    public event Action<float> ASoulMeterChanged;

    public override void ClearStrongInputs()
    {
        base.ClearStrongInputs();
        InputManager.ClearBuffer();
    }

    public override void Init()
    {
        base.Init();

        //soundFolderName = "Sounds/Player/";

        //advancedMovementStates.Init(this);
        //CSMachine.ChangeState(SCS_Idle.InstanceP);


        //attackStates.Init(this);



        Ctr.wallslideSpeed = GetCurrentStatValue("WallslideSpeed");

        ComboCounter.AComboIsOver += AddHealthAfterCombo;
    }

    void Update()
    {
        //Test----------------------------------------------------------------------------------------------------------------------

        if (Input.GetKeyDown(KeyCode.V))
        {
            StatMod slow = new StatMod(2f, false, true, 60, "Movespeed");
            slow.ApplyToCharacter(this);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            CardEffect_SingleAttackStrength cardEffect = new CardEffect_SingleAttackStrength();
            cardEffect.attackType = EAttackType.UTilt;
            cardEffect.damageMultiplier = 2f;

            cardEffects.Add(cardEffect);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            CardEffect_StatMod cardEffect = new CardEffect_StatMod();
            cardEffect.attackType = EAttackType.Jab1;
            cardEffect.statMod = new StatMod(0.5f, false, true, 600, "Movespeed");

            cardEffects.Add(cardEffect);
        }

        //Test----------------------------------------------------------------------------------------------------------------------


        DirectionalInput = InputManager.Direction;
        StrongInputs = InputManager.Smash;
        TiltInput = InputManager.CStick;

        if (Mathf.Abs(DirectionalInput.x) < 0.25f) DirectionalInput = new Vector2(0, DirectionalInput.y);

        if (InputManager.BufferdDown("Attack")) Attack = true;
        else Attack = false;


        if (InputManager.Attack.GetButton()) HoldAttack = true;
        else HoldAttack = false;

        if (InputManager.BufferdDown("Special")) Special = true;
        else Special = false;


        if (InputManager.Special.GetButton()) HoldSpecial = true;
        else HoldSpecial = false;

        if (InputManager.BufferdDown("Jump") || InputManager.BufferdDown("Jump2") || (StrongInputs.y > 0) && InputManager.TapJump == true) Jump = true;
        else
        {
            //reset at the end of FixedUpdate to not miss any inputs
            //Jump = false;
        }

        if (InputManager.Jump.GetButton() || InputManager.Jump2.GetButton()) HoldJump = true;
        else HoldJump = false;


        if ((InputManager.BufferdDown("Shield"))) Shield = true;
        else Shield = false;

        if (InputManager.Shield.GetButton()) HoldShield = true;
        else HoldShield = false;

        if (InputManager.BufferdDown("Grab")) Grab = true;
        else Grab = false;

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        BalanceSoulMeter();
    }

    protected override void InitStats()
    {
        base.InitStats();

        if (GetCurrentStatValue("MaxSouls") != 0)
        {
            currentSouls = 0;
            ModSouls(GetCurrentStatValue("MaxSouls"));
        }
    }


    public override void ModSouls(float value)
    {
        base.ModSouls(value);

        if (ASoulsChanged != null) ASoulsChanged((int)currentSouls);
    }

    protected void BalanceSoulMeter()
    {
        if (currentSouls > GetCurrentStatValue("MaxSouls"))
        {
            ModSouls(-1);
        }

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

        AddSoulAfterHit(damage);

        if (EnemyHit != null) EnemyHit(enemy, damage);

        //StartCoroutine(IFreezePlayerOnHit(damage));
    }
    /*
    private IEnumerator IFreezePlayerOnHit(Damage damage)
    {
        Ctr.freeze = true;
        for (int i = 0; i < damage.damageNumber + 3; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        Ctr.freeze = false;
    }
    */
    private void AddHealthAfterCombo(float comboScore)
    {
        ModSoulMeter(comboScore);
    }

    public override void GetHit(Damage damage)
    {
        base.GetHit(damage);

        if (PlayerHealthChanged != null) PlayerHealthChanged(soulMeter / soulMeterMax);
    }

    protected override void TakeDamage(Damage damage)
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

                ModSoulMeter(-damage.damageNumber);
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

                currentKnockback = damage.Knockback(transform.position, GetCurrentStatValue("Weight"), (SoulPercent));

                ModSoulMeter(-damage.damageNumber);
            }
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void AddSoulAfterHit(Damage damage)
    {
        ModSoulMeter(damage.damageNumber / 5);
    }


    public override void ModSoulMeter(float value)
    {
        if (soulMeter < soulMeterMax / 2) //only wait to fill up again, but drain soul immediately if more than half full
        {
            soulBalanceDelayTimer = soulBalanceDelayDuration;
        }
        else
        {
            soulBalanceDelayTimer = 0;
        }

        soulMeter += value;

        if (soulMeter > soulMeterMax)
        {
            soulBank += (int)(soulMeter - soulMeterMax);

            if (ASoulBankPlus != null) ASoulBankPlus(soulBank);
        }

        if (soulMeter <= 0)
        {
            ModSouls(-1);

            if (currentSouls != 0)
                soulMeter = soulMeterMax;

            if (currentSouls <= 0)
            {
                Die();
            }
        }


        if (ASoulMeterChanged != null) ASoulMeterChanged(soulMeter);
    }

    protected bool CheckForItemPickUp()
    {
        if (holdItem == null)
        {
            RaycastHit2D itemCheck = Physics2D.CircleCast(transform.position, pickUpRadius, Vector2.zero, 0, enemyMask);

            if (itemCheck)
            {
                Interactable interactable = itemCheck.transform.GetComponentInParent<Interactable>();
                if (interactable != null)
                {
                    InputManager.ClearBuffer();
                    interactable.StartInteraction(this);
                    return true;
                }

                Item pickedItem = itemCheck.transform.GetComponentInParent<Item>();
                if (pickedItem != null)
                {
                    if (pickedItem is ICanBePickedUp)
                    {
                        InputManager.ClearBuffer();
                        SetHoldItem(pickedItem);
                        return true;
                    }
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
        holdItem.Damage.Owner = this;
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
                if (HoldShield)
                {
                    CSMachine.ChangeState(GetAttackState(EAttackType.Grab));
                }
                else if (DirectionalInput == Vector2.zero)
                {
                    CSMachine.ChangeState(GetAttackState(EAttackType.Jab1));
                }
                else if (Mathf.Abs(DirectionalInput.x) > 0.9f)
                {
                    CSMachine.ChangeState(GetAttackState(EAttackType.DashAtk));
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

        if (Grab)
        {
            CSMachine.ChangeState(GetAttackState(EAttackType.Grab));
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
                Direction = DirectionalInput.x;

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

    public override bool CheckForThrowAttacks()
    {
        if (StrongInputs.sqrMagnitude == 1f)
        {
            if (Mathf.Abs(StrongInputs.x) > 0.5f)
            {
                if (Mathf.Sign(StrongInputs.x) == Direction) CSMachine.ChangeState(GetAttackState(EAttackType.FTilt));
                else CSMachine.ChangeState(GetAttackState(EAttackType.Jab1));

                return true;
            }
            else if (StrongInputs.y > 0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.UTilt));
                return true;
            }
            else if (StrongInputs.y < -0.5f)
            {
                CSMachine.ChangeState(GetAttackState(EAttackType.DTilt));
                return true;
            }
        }
        return false;
    }


}
