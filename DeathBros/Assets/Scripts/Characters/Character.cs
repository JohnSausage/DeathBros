using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FrameAnimator))]
public class Character : _MB
{
    public string charName;

    public string soundFolderName;

    public Vector2 DirectionalInput { get; protected set; }
    public Vector2 StrongInputs;// { get; protected set; }

    public bool Jump { get; protected set; }
    public bool HoldJump { get; protected set; }
    public bool Attack { get; protected set; }
    public bool HoldAttack { get; protected set; }

    //public float jumpStrength = 20;
    //public int jumps = 2;
    public int jumpsUsed { get; set; }

    [Space]

    public StateMachine CSMachine;// { get; protected set; }
    public FrameAnimator Anim { get; protected set; }
    public SpriteRenderer Spr { get; protected set; }
    public Controller2D Ctr { get; protected set; }

    //public CStates_Movement movementStates;
    //public CStates_AdvancedMovement advancedMovementStates;

    public List<CState> cStates;

    [Space]

    public Stats stats;

    public Damage currentDamae { get; protected set; }

    public Queue<int> hitIDs = new Queue<int>();

    /*
    public bool IsFlipped
    {
        set
        {
            Spr.flipX = value;
        }
        get
        {
            return Spr.flipX;
        }
    }
    */
    public override void Init()
    {
        base.Init();

        CSMachine = new StateMachine();
        Anim = GetComponent<FrameAnimator>();
        Anim.Init();

        Spr = GetComponent<SpriteRenderer>();
        Ctr = GetComponent<Controller2D>();

        stats.Init();
        //movementStates.Init(this);
        //advancedMovementStates.Init(this);
        //CSMachine.ChangeState(advancedMovementStates.idle);
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
        StrongInputs = Vector2.zero;
        if (Mathf.Abs(Ctr.input.x - DirectionalInput.x) > 0.1f && Mathf.Abs(DirectionalInput.x) > 0.8f)
        {
            StrongInputs = new Vector2(DirectionalInput.x - Ctr.input.x, 0);
        }

        CSMachine.Update();

        stats.FixedUpdate();

        UpdatesStatsForCtr();
        Ctr.ManualFixedUpdate();

        currentDamae = null;
        Jump = false;

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

    public virtual void TakeDamage(Damage damage)
    {
        currentDamae = damage;

        CState currentState = (CState)CSMachine.CurrentState;
        currentState.TakeDamage(damage);

        float knockbackGrowth = (1 - stats.currentHealth / stats.maxHealth.CurrentValue) * damage.knockbackGrowth;

        stats.currentHealth -= damage.damageNumber;

        Vector2 knockback = new Vector2(damage.knockBackDirection.normalized.x, damage.knockBackDirection.normalized.y) * (damage.baseKnockback + knockbackGrowth);
        Ctr.knockback = knockback;

        if (stats.currentHealth < 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log(this.name + " died");

        Destroy(gameObject);
    }

    public virtual void SetInputs()
    {
        Ctr.input = DirectionalInput;
    }

    public virtual void SetInputs(Vector2 inputs)
    {
        DirectionalInput = inputs;
        SetInputs();
    }

    public virtual void SetInputs(float reduceControl)
    {
        DirectionalInput *= reduceControl;
        SetInputs();
    }

    /*
    public virtual void CS_CheckForJump()
    {
        if (jumpsUsed < jumps)
        {
            if (Ctr.grounded)
                CSMachine.ChangeState(advancedMovementStates.jumpsquat);
            else
                CSMachine.ChangeState(advancedMovementStates.doubleJumpsquat);
        }
    }

    public virtual void CS_StartJump()
    {
        Ctr.jumpVelocity = jumpStrength;
        CSMachine.ChangeState(advancedMovementStates.jumping);
    }
     */

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
            if(cStates[i] is CS_Attack)
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
        if (Ctr.grounded)
        {
            //CSMachine.ChangeState(advancedMovementStates.landing);
            CSMachine.ChangeState(GetState(typeof(CS_Landing)));
        }
    }

    public virtual void CS_CheckIfStillGrounded()
    {
        if (!Ctr.grounded)
        {
            //CSMachine.ChangeState(advancedMovementStates.jumping);
            CSMachine.ChangeState(GetState(typeof(CS_Jumping)));
        }
    }

    public virtual void CS_SetIdle()
    {
        //CSMachine.ChangeState(advancedMovementStates.idle);
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
}
