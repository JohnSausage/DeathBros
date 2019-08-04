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

    public int Gold { get { return GameManager.SaveData.goldAmount; } protected set { GameManager.SaveData.goldAmount = value; } }

    public float ComboPower { get; protected set; }
    protected bool[] cardPowerActivated = new bool[5];

    public float pickUpRadius = 1.5f;

    public LayerMask enemyMask;
    public LayerMask interactionMask;
    public LayerMask pickUpMask;

    public bool Grab { get; set; }
    public bool Interact { get; protected set; }

    public Item holdItem { get; protected set; }
    public bool hasItem { get { return holdItem != null; } }

    [SerializeField]
    protected List<ComboCardDataSO> comboCards;


    //public static event Action<float> PlayerHealthChanged;
    public static event Action<Character, Damage> EnemyHit;

    public event Action<float> AChangeComboPower;
    public event Action APlayerInteract;

    public event Action<int> APlayerGoldUpdate;

    public event Action APlayerRespawn;

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


    public override void Restart()
    {
        base.Restart();

        ComboPower = 50; //@@@ set to 0 later
    }

    protected override void Start()
    {
        base.Start();

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Vector2 loadedSpawn = new Vector2(GameManager.Instance.saveData.spawnX, GameManager.Instance.saveData.spawnY);

        if (loadedSpawn != Vector2.zero)
        {
            transform.position = loadedSpawn;
        }

        CameraController.Black();
        SCS_ChangeState(StaticStates.spawn);
        CameraController.Position = Position;
    }

    public void Respawn()
    {
        Restart();
        transform.position = new Vector2(GameManager.Instance.saveData.spawnX, GameManager.Instance.saveData.spawnY);
        SpawnPlayer();

        if (APlayerRespawn != null)
        {
            APlayerRespawn();
        }
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

        if (InputManager.BufferdDown("Interact"))
        {
            if (APlayerInteract != null)
            {
                APlayerInteract();
            }

            InputManager.ClearBuffer();
            Interact = true;
        }
        else
        {
            Interact = false;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ModifiyComboPower(-0.01f);
        ModHealth(+0.01f);

        ManageCardBuffs();

        CheckForAutoPickUp();
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
                currentAttackType = EAttackType.DashAtk;
            }
            else if (DirectionalInput == Vector2.zero)
            {
                ChrSM.ChangeState(this, jabAtk);
                currentAttackType = EAttackType.Jab1;
            }
            else if (Mathf.Abs(DirectionalInput.x) >= 0.1f)
            {
                ChrSM.ChangeState(this, fTiltAtk);
                currentAttackType = EAttackType.FTilt;
            }
            else if (DirectionalInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uTiltAtk);
                currentAttackType = EAttackType.UTilt;
            }
            else if (DirectionalInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dTiltAtk);
                currentAttackType = EAttackType.DTilt;
            }
        }
        else if (TiltInput != Vector2.zero)
        {
            if (isRunning)
            {
                ChrSM.ChangeState(this, dashAtk);
                currentAttackType = EAttackType.DashAtk;
            }
            else if (Mathf.Abs(TiltInput.x) > 0.5f)
            {
                Direction = TiltInput.x;

                ChrSM.ChangeState(this, fTiltAtk);
                currentAttackType = EAttackType.FTilt;
            }
            else if (TiltInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uTiltAtk);
                currentAttackType = EAttackType.UTilt;
            }
            else if (TiltInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dTiltAtk);
                currentAttackType = EAttackType.DTilt;
            }
        }

        SCS_CheckForSpecials();

        if (Grab)
        {
            currentAttackType = EAttackType.Grab;
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
                currentAttackType = EAttackType.NAir;
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f && Mathf.Sign(DirectionalInput.x) == Direction)
            {
                ChrSM.ChangeState(this, fAirAtk);
                currentAttackType = EAttackType.FAir;
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f && Mathf.Sign(DirectionalInput.x) != Direction)
            {
                ChrSM.ChangeState(this, bAirAtk);
                currentAttackType = EAttackType.BAir;
            }
            else if (DirectionalInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uAirAtk);
                currentAttackType = EAttackType.UAir;
            }
            else if (DirectionalInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dAirAtk);
                currentAttackType = EAttackType.DAir;
            }
        }

        if (TiltInput != Vector2.zero)
        {
            if (Mathf.Abs(TiltInput.x) > 0.5f && Mathf.Sign(TiltInput.x) == Direction)
            {
                ChrSM.ChangeState(this, fAirAtk);
                currentAttackType = EAttackType.FAir;
            }
            else if (Mathf.Abs(TiltInput.x) > 0.5f && Mathf.Sign(TiltInput.x) != Direction)
            {
                ChrSM.ChangeState(this, bAirAtk);
                currentAttackType = EAttackType.BAir;
            }
            else if (TiltInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uAirAtk);
                currentAttackType = EAttackType.UAir;
            }
            else if (TiltInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dAirAtk);
                currentAttackType = EAttackType.DAir;
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
                currentAttackType = EAttackType.NSpec;
            }
            else if (DirectionalInput.y >= 0.5f)
            {
                CheckForSpecialAttack(uSpecAtk, uSpecCount);
                currentAttackType = EAttackType.USpec;
            }
            else if (DirectionalInput.y <= -0.5f)
            {
                CheckForSpecialAttack(dSpecAtk, dSpecCount);
                currentAttackType = EAttackType.DSpec;
            }
            else if (Mathf.Abs(DirectionalInput.x) > 0.5f)
            {
                CheckForSpecialAttack(sSpecAtk, sSpecCount);
                currentAttackType = EAttackType.FSpec;
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
                currentAttackType = EAttackType.None;
            }
            else if (DirectionalInput.y > 0.5f)
            {
                ChrSM.ChangeState(this, uTiltAtk);
                currentAttackType = EAttackType.None;
            }
            else if (DirectionalInput.y < -0.5f)
            {
                ChrSM.ChangeState(this, dTiltAtk);
                currentAttackType = EAttackType.None;
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

        if (Interact)
        {
            CheckForInteraction();
        }
    }


    protected void CheckForInteraction()
    {
        RaycastHit2D interactionCheck = Physics2D.CircleCast(Position, pickUpRadius, Vector2.zero, 0, interactionMask);

        if (interactionCheck)
        {
            ICanInteract canInteract = interactionCheck.transform.GetComponent<ICanInteract>();

            if (canInteract != null)
            {
                canInteract.StartInteraction(this);
            }
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

        if (Interact)
        {
            CheckForInteraction();
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

    public override void SCS_OnSpawn()
    {
        base.SCS_OnSpawn();

        CameraController.LoadScreenOff();
    }

    public void SetSpecialAttack(StaticAttackStateSpecial attackSO, ESpecial type)
    {
        if (attackSO == null)
        {
            return;
        }

        switch (type)
        {
            case ESpecial.NEUTRAL:
                {
                    nSpecAtk = attackSO.CreateAttackState(type);
                    break;
                }

            case ESpecial.SIDE:
                {
                    sSpecAtk = attackSO.CreateAttackState(type);
                    break;
                }

            case ESpecial.UP:
                {
                    uSpecAtk = attackSO.CreateAttackState(type);
                    break;
                }

            case ESpecial.DOWN:
                {
                    dSpecAtk = attackSO.CreateAttackState(type);
                    break;
                }

            default:
                {
                    break;
                }
        }
    }

    public void SetSpecialAttack(int index, StaticAttackStateSpecial attackSO)
    {
        if (attackSO == null)
        {
            return;
        }

        switch (index)
        {
            case 0:
                {
                    nSpecAtk = attackSO.CreateAttackState(ESpecial.NEUTRAL);
                    break;
                }

            case 1:
                {
                    sSpecAtk = attackSO.CreateAttackState(ESpecial.SIDE);
                    break;
                }

            case 2:
                {
                    uSpecAtk = attackSO.CreateAttackState(ESpecial.UP);
                    break;
                }

            case 3:
                {
                    dSpecAtk = attackSO.CreateAttackState(ESpecial.DOWN);
                    break;
                }

            default:
                {
                    break;
                }
        }
    }

    protected void CheckForAutoPickUp()
    {
        RaycastHit2D pickUpCheck = Physics2D.CircleCast(Position, pickUpRadius / 2f, Vector2.zero, 0, pickUpMask);

        if (pickUpCheck)
        {
            IAutoPickup autoPickUpItem = pickUpCheck.transform.GetComponent<IAutoPickup>();

            if (autoPickUpItem != null)
            {
                autoPickUpItem.GetPickedUp(this);
            }
        }
    }



    public void AddGold(int amount)
    {
        Gold += amount;

        if (APlayerGoldUpdate != null) APlayerGoldUpdate(Gold);
    }

    public void AddSkillCard(int cardIndex)
    {
        if (cardIndex >= GameManager.SaveData.skillAvailable.Length)
        {
            return;
        }

        GameManager.SaveData.skillAvailable[cardIndex] = true;
    }

    public override Damage GetModifiedDamage(Damage damage)
    {
        Damage returnDamage = damage;

        foreach (ComboCardDataSO card in comboCards)
        {
            returnDamage = card.ModifyDamage(returnDamage);
        }

        return returnDamage;
    }
}
