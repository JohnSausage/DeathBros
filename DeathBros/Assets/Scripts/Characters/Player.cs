using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public StaticAttackStateSO jab;
    public StaticAttackStateSO dTilt;
    public StaticAttackStateSO uTilt;
    public StaticAttackStateSO fTilt;
    public StaticAttackStateSO dash;

    public StaticAttackStateSO grab;

    public StaticAttackStateSO nAir;
    public StaticAttackStateSO fAir;
    public StaticAttackStateSO bAir;
    public StaticAttackStateSO dAir;
    public StaticAttackStateSO uAir;


    public StaticAttackStateSpecial nSpec;
    public StaticAttackStateSpecial sSpec;
    public StaticAttackStateSpecial uSpec;
    public StaticAttackStateSpecial dSpec;

    public SCS_Attack jabAtk;
    public SCS_Attack dTiltAtk;
    public SCS_Attack uTiltAtk;
    public SCS_Attack fTiltAtk;
    public SCS_Attack dashAtk;

    public SCS_Attack grabAtk;

    public SCS_Attack nAirAtk;
    public SCS_Attack fAirAtk;
    public SCS_Attack bAirAtk;
    public SCS_Attack dAirAtk;
    public SCS_Attack uAirAtk;

    public SCS_SpecialAttack nSpecAtk;
    public SCS_SpecialAttack sSpecAtk;
    public SCS_SpecialAttack uSpecAtk;
    public SCS_SpecialAttack dSpecAtk;

    protected int nSpecCount;
    protected int sSpecCount;
    protected int uSpecCount;
    protected int dSpecCount;

    public float ComboPower { get; protected set; }
    protected bool[] cardPowerActivated = new bool[5];

    public float pickUpRadius = 1.5f;

    public LayerMask enemyMask;

    public bool Grab { get; set; }

    public Item holdItem { get; protected set; }
    public bool hasItem { get { return holdItem != null; } }


    //public static event Action<float> PlayerHealthChanged;
    public static event Action<Character, Damage> EnemyHit;

    public event Action<float> AChangeComboPower;

    public override void ClearStrongInputs()
    {
        base.ClearStrongInputs();
        InputManager.ClearBuffer();
    }

    public override void Init()
    {
        base.Init();

        Ctr.WallslideSpeed = GetCurrentStatValue("WallslideSpeed");


        jabAtk = jab.CreateAttackState();
        dTiltAtk = dTilt.CreateAttackState();
        uTiltAtk = uTilt.CreateAttackState();
        fTiltAtk = fTilt.CreateAttackState();
        dashAtk = dash.CreateAttackState();

        grabAtk = grab.CreateAttackState();


        nAirAtk = nAir.CreateAttackState();
        fAirAtk = fAir.CreateAttackState();
        bAirAtk = bAir.CreateAttackState();
        dAirAtk = dAir.CreateAttackState();
        uAirAtk = uAir.CreateAttackState();


        nSpecAtk = nSpec.CreateAttackState(ESpecial.NEUTRAL);
        sSpecAtk = sSpec.CreateAttackState(ESpecial.SIDE);
        uSpecAtk = uSpec.CreateAttackState(ESpecial.UP);
        dSpecAtk = dSpec.CreateAttackState(ESpecial.DOWN);

        AEnemyHit += OnEnemyHit;
        ComboPower = 50; //@@@ set to 0 later

        walkSpeedReduction = 0.5f;
    }

    protected void OnEnemyHit(Character enemy, Damage damage)
    {
        //ModifiyComboPower(damage.damageNumber);
    }

    public void ModifiyComboPower(float value)
    {
        ComboPower += value;
        ComboPower = Mathf.Clamp(ComboPower, 0, 110);

        for (int i = 0; i < cardPowerActivated.Length; i++)
        {
            cardPowerActivated[i] = false;
        }

        if (ComboPower >= 20)
        {
            cardPowerActivated[0] = true;
        }
        if (ComboPower >= 40)
        {
            cardPowerActivated[1] = true;
        }
        if (ComboPower >= 60)
        {
            cardPowerActivated[2] = true;
        }
        if (ComboPower >= 80)
        {
            cardPowerActivated[3] = true;
        }
        if (ComboPower >= 100)
        {
            cardPowerActivated[4] = true;
        }

        if (AChangeComboPower != null) AChangeComboPower(ComboPower);
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
        StrongInputs = InputManager.StrongInput;
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

        ModifiyComboPower(-0.01f);
        ModHealth(+0.01f);

        ManageCardBuffs();
    }

    public override void UpdateInputs()
    {
        DirectionalInput = InputManager.Direction;
        GetInputs();
    }

    public override void HitEnemy(Character enemy, Damage damage)
    {
        base.HitEnemy(enemy, damage);

        if (EnemyHit != null) EnemyHit(enemy, damage);
    }

    //protected override void TakeDamage(Damage damage)
    //{
    //    if (!dead)
    //    {
    //        if (shielding)
    //        {
    //            damage.damageNumber *= 0.25f;


    //            RaiseTakeDamageEvents(damage);

    //            AudioManager.PlaySound("NES_hit1");

    //            currentDamage = damage;

    //            if (damage.Owner != null)
    //            {
    //                damage.Owner.HitEnemy(this, damage);
    //            }
    //        }
    //        else
    //        {
    //            RaiseTakeDamageEvents(damage);

    //            AudioManager.PlaySound("NES_hit1");
    //            Flash(EffectManager.ColorHit, 3);

    //            currentDamage = damage;

    //            if (damage.Owner != null)
    //            {
    //                damage.Owner.HitEnemy(this, damage);
    //            }

    //            currentKnockback = damage.Knockback(transform.position, GetCurrentStatValue("Weight"), (HealthPercent));

    //            currentHealth -= damage.damageNumber;
    //        }
    //        if (currentHealth <= 0)
    //        {
    //            Die();
    //        }
    //    }
    //}


    //protected bool CheckForItemPickUp()
    //{
    //    if (holdItem == null)
    //    {
    //        RaycastHit2D itemCheck = Physics2D.CircleCast(transform.position, pickUpRadius, Vector2.zero, 0, enemyMask);

    //        if (itemCheck)
    //        {
    //            Interactable interactable = itemCheck.transform.GetComponentInParent<Interactable>();
    //            if (interactable != null)
    //            {
    //                InputManager.ClearBuffer();
    //                interactable.StartInteraction(this);
    //                return true;
    //            }

    //            Item pickedItem = itemCheck.transform.GetComponentInParent<Item>();
    //            if (pickedItem != null)
    //            {
    //                if (pickedItem is ICanBePickedUp)
    //                {
    //                    InputManager.ClearBuffer();
    //                    SetHoldItem(pickedItem);
    //                    return true;
    //                }
    //            }
    //        }
    //    }

    //    return false;
    //}

    public bool ThrowItem(Vector2 throwVelocity)
    {
        //if (holdItem != null)
        //{
        //    Vector2 itemVelocity;

        //    if (throwVelocity == Vector2.zero)
        //    {
        //        itemVelocity = Vector2.zero;
        //    }
        //    else if (Mathf.Abs(throwVelocity.x) > Mathf.Abs(throwVelocity.y))
        //    {
        //        itemVelocity = new Vector2(Mathf.Sign(throwVelocity.x), 0.25f);
        //    }
        //    else
        //    {
        //        itemVelocity = new Vector2(0, Mathf.Sign(throwVelocity.y));
        //    }

        //    holdItem.Velocity = itemVelocity.normalized * 25f;
        //    ReleaseHoldItem();

        //    InputManager.ClearBuffer();
        //    //CSMachine.ChangeState(advancedMovementStates.throwItem);
        //    return true;
        //}
        //return false;

        return false;
    }

    //public void SetHoldItem(Item item)
    //{
    //    holdItem = item;
    //    holdItem.IsSimulated = false;
    //    holdItem.Owner = this;
    //    holdItem.transform.SetParent(transform);
    //    holdItem.transform.localPosition = Vector3.zero;
    //    holdItem.Damage.Owner = this;
    //}

    //public void ReleaseHoldItem()
    //{
    //    if (hasItem)
    //    {
    //        holdItem.transform.SetParent(null);
    //        holdItem.IsSimulated = true;
    //        holdItem.GenerateID();
    //        holdItem = null;
    //    }
    //}


    public override void SCS_CheckForGroundAttacks()
    {

        if (Attack)
        {
            if (isRunning)
            {
                ChrSM.ChangeState(this, dashAtk);
            }
            else if (DirectionalInput == Vector2.zero)
            {
                ChrSM.ChangeState(this, jabAtk);
            }
            else if (Mathf.Abs(DirectionalInput.x) >= 0.1f)
            {
                ChrSM.ChangeState(this, fTiltAtk);
            }
            else if (DirectionalInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uTiltAtk);
            }
            else if (DirectionalInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dTiltAtk);
            }
        }
        else if (TiltInput != Vector2.zero)
        {
            if (isRunning)
            {
                ChrSM.ChangeState(this, dashAtk);
            }
            else if (Mathf.Abs(TiltInput.x) > 0.5f)
            {
                Direction = TiltInput.x;

                ChrSM.ChangeState(this, fTiltAtk);
            }
            else if (TiltInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uTiltAtk);
            }
            else if (TiltInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dTiltAtk);
            }
        }

        SCS_CheckForSpecials();

        if (Grab)
        {
            ChrSM.ChangeState(this, grabAtk);
        }
    }


    public override void SCS_CheckForAerials()
    {
        if (Attack)
        {
            if (DirectionalInput == Vector2.zero)
            {
                ChrSM.ChangeState(this, nAirAtk);
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f && Mathf.Sign(DirectionalInput.x) == Direction)
            {
                ChrSM.ChangeState(this, fAirAtk);
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f && Mathf.Sign(DirectionalInput.x) != Direction)
            {
                ChrSM.ChangeState(this, bAirAtk);
            }
            else if (DirectionalInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uAirAtk);
            }
            else if (DirectionalInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dAirAtk);
            }
        }

        if (TiltInput != Vector2.zero)
        {
            if (Mathf.Abs(TiltInput.x) > 0.5f && Mathf.Sign(TiltInput.x) == Direction)
            {
                ChrSM.ChangeState(this, fAirAtk);
            }
            else if (Mathf.Abs(TiltInput.x) > 0.5f && Mathf.Sign(TiltInput.x) != Direction)
            {
                ChrSM.ChangeState(this, bAirAtk);
            }
            else if (TiltInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uAirAtk);
            }
            else if (TiltInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dAirAtk);
            }
        }

        SCS_CheckForSpecials();
    }

    protected void CheckForSpecialAttack(SCS_SpecialAttack specialAtk, int specCount)
    {
        if (specCount < specialAtk.aerialLimit || specialAtk.aerialLimit == 0)
        {
            if (ComboPower >= specialAtk.comboPowerCost)
            {
                ModifiyComboPower(-specialAtk.comboPowerCost);
                ChrSM.ChangeState(this, specialAtk);
            }
        }
    }

    protected void SCS_CheckForSpecials()
    {
        if (Special)
        {
            if (DirectionalInput == Vector2.zero)
            {
                CheckForSpecialAttack(nSpecAtk, nSpecCount);
            }
            else if (DirectionalInput.y >= 0.5f)
            {
                CheckForSpecialAttack(uSpecAtk, uSpecCount);
            }
            else if (DirectionalInput.y <= -0.5f)
            {
                CheckForSpecialAttack(dSpecAtk, dSpecCount);
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f)
            {
                CheckForSpecialAttack(sSpecAtk, sSpecCount);
            }
        }
    }

    public override bool SCS_CheckForThrowAttacks()
    {

        if (DirectionalInput != Vector2.zero)
        {
            if (Mathf.Abs(DirectionalInput.x) > 0.5f)
            {
                Direction = DirectionalInput.x;

                ChrSM.ChangeState(this, fTiltAtk);
            }
            else if (DirectionalInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uTiltAtk);
            }
            else if (DirectionalInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dTiltAtk);
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public override void SCS_CheckForTech()
    {
        if (HoldShield)
        {
            HitStunDuration = 0;
            Ctr.InControl = true;

            //RaiseComboOverEvent();

            //if (Ctr.lastCollisionAngle <= 45)
            if (Ctr.IsGrounded)
            {
                if (Mathf.Abs(DirectionalInput.x) > 0.5f)
                {
                    SCS_ChangeState(StaticStates.roll);
                }
                else SCS_ChangeState(StaticStates.standUp);
            }
            //else if (Ctr.lastCollisionAngle == 90)
            else if (Ctr.OnWall)
            {
                if (HoldJump)
                {
                    SCS_ChangeState(StaticStates.walljumpStart);
                }
                else
                {
                    SCS_ChangeState(StaticStates.jumping);
                }
            }
            else
            {
                SCS_ChangeState(StaticStates.jumping);
            }
        }
    }

    public override void SCS_CheckForAerialTech()
    {
        if (HoldShield)
        {
            //RaiseComboOverEvent();

            if (HoldJump || DirectionalInput.y >= 0.5f)
            {
                SCS_ChangeState(StaticStates.walljumpStart);
            }
            else
            {
                SCS_ChangeState(StaticStates.jumping);
            }
        }
    }

    public override void SCS_GetUpAfterHitLanded()
    {
        if (DirectionalInput.y > 0.5f)
        {
            SCS_ChangeState(StaticStates.standUp);

        }
        else if (Mathf.Abs(DirectionalInput.x) > 0.5f)
        {
            SCS_ChangeState(StaticStates.roll);
        }
    }

    public override void SCS_CheckForIdleOptions()
    {
        if (DirectionalInput.y < -0.5f)
        {
            SCS_ChangeState(StaticStates.crouch);
        }

        if (StrongInputs.x != 0)
        {
            SCS_ChangeState(StaticStates.dash);
        }
        else if (Mathf.Abs(DirectionalInput.x) != 0)
        {
            if (Mathf.Abs(DirectionalInput.x) >= 0.4f)
            {
                SCS_ChangeState(StaticStates.dash);
            }
            else
            {
                SCS_ChangeState(StaticStates.walking);
            }
        }

        if (HoldShield)
        {
            SCS_ChangeState(StaticStates.shield);
        }

        if (Jump)
        {
            SCS_ChangeState(StaticStates.jumpsquat);
        }
    }

    public override void SCS_CheckForWalkingOptions()
    {
        if (Mathf.Abs(DirectionalInput.x) == 0f || Mathf.Sign(DirectionalInput.x) != Direction)
        {
            if (isRunning)
            {
                SCS_ChangeState(StaticStates.skid);
            }
            else
            {
                SCS_ChangeState(StaticStates.idle);
            }
        }

        if (HoldShield)
        {
            SCS_ChangeState(StaticStates.shield);
        }

        if (Jump)
        {
            SCS_ChangeState(StaticStates.jumpsquat);
        }

        if (DirectionalInput.y < -0.5f)
        {
            SCS_ChangeState(StaticStates.crouch);
        }

        if (Timer >= 15)
        {
            if (Mathf.Abs(DirectionalInput.x) >= 0.45f)
            {
                SCS_ChangeState(StaticStates.running);
            }
        }
    }

    public override void SCS_CheckForLandingOptions()
    {
        if (DirectionalInput.y < 0)
        {
            SCS_ChangeState(StaticStates.crouch);
        }
        else
        {
            if (Mathf.Abs(StrongInputs.x) == 1)
            {
                SCS_ChangeState(StaticStates.dash);
            }
            else if (DirectionalInput.x == 0)
            {
                SCS_ChangeState(StaticStates.idle);
            }
            else if (Mathf.Abs(DirectionalInput.x) <= 0.5f)
            {
                SCS_ChangeState(StaticStates.walking);
            }
            else
            {
                SCS_ChangeState(StaticStates.dash);
            }
        }
    }

    public override void SCS_CountSpecial(ESpecial type)
    {
        switch (type)
        {
            case ESpecial.NEUTRAL:
                {
                    nSpecCount++;
                    break;
                }
            case ESpecial.SIDE:
                {
                    sSpecCount++;
                    break;
                }
            case ESpecial.UP:
                {
                    uSpecCount++;
                    break;
                }
            case ESpecial.DOWN:
                {
                    dSpecCount++;
                    break;
                }
            default:
                break;
        }
    }

    public override void SCS_RaiseLandingEvent()
    {
        base.SCS_RaiseLandingEvent();

        nSpecCount = 0;
        sSpecCount = 0;
        uSpecCount = 0;
        dSpecCount = 0;
    }

    protected void ManageCardBuffs()
    {
        if (cardPowerActivated[0])
        {
            dTiltAtk.attackBuff.damageMulti = 2;//@@@ remove after testing
        }
        else
        {
            dTiltAtk.attackBuff.damageMulti = 1;
        }

        if (cardPowerActivated[1])
        {

        }
        else
        {

        }

    }
}
