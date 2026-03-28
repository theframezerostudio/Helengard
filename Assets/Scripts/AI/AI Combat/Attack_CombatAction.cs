using System;
using UnityEngine;

public class Attack_CombatAction : CombatSubAction
{
    [SerializeField] private AttackPersona attackPersona;

    [SerializeField, ReadOnly] 
    private AICombatDecision combatDecision;

    private AgentMotionHandler motionHandler;

    private Weapon weapon;
    private ComboNode node;
    private Animator animator;
    private FrameWindow lockWindow;
    
    private float attackTimer;
    private float animNormalizedTime;
    private float animDuration;
    private bool isAttacking;
    private bool attackSuccessful = false;

    // TODO: For debuuging only, remove later
    [SerializeField, ReadOnly] private AttackInput attackInput;

    private void Awake()
    {
        combatDecision = new AICombatDecision(attackPersona);
    }

    public override void Enter()
    {
        motionHandler = stateContext.MotionHandler;
        weapon = owner.CurrentWeapon;

        lockWindow = new FrameWindow(0.2f, 0.7f);

        motionHandler.rotationMode = RotationMode.FaceTarget;
        combatDecision.Initialize(owner.Context);

        AttackInput entryInput = combatDecision.DecideEntry(combatData, combatMemory, weapon.comboGraph);
        node = entryInput != AttackInput.None
            ? weapon.InitiateAttack(owner.Context, entryInput)
            : null;

        weapon.OnHit += OnWeaponHit;

        SetupAttack();
    }

    private void SetupAttack()
    {
        if (node == null)
            return;

        if (isAttacking)
            EndAttack();

        combatData.MinDesiredRange = 0f;
        combatData.MaxDesiredRange = node.attackRange;

        // TODO: For debuuging only, remove later
        attackInput = node.input;

        animator = owner.Animator.GetAnimator();

        owner.Context.GravityScale = 0f;
        owner.verticalVelocity = 0f;
        owner.FeetIKResolver.SetFeetIk(false);

        owner.PlayAnim(node.animationStateName, node.transitionTime);

        animDuration = GetStateDuration();
        attackTimer = 0f;
        animNormalizedTime = 0f;
        isAttacking = false;
        attackSuccessful = false;

        bool isLightAttack = node.input == AttackInput.Light || node.input == AttackInput.LightHold;
        owner.Context.dataAggregator.SetAttacking(true, isLightAttack);
    }

    public override void Tick()
    {
        base.Tick();

        if (node == null)
            return;

        if (stateTimer < 0.1f && combatData.Distance > node.attackRange)
            return;

        if (!IsLocked && lockWindow.IsValid(animNormalizedTime))
        {
            Lock();
        }
        else if (IsLocked && !lockWindow.IsValid(animNormalizedTime))
        {
            Unlock();
        }

        attackTimer += Time.deltaTime;
        animNormalizedTime = Mathf.Clamp01(attackTimer / animDuration);

        motionHandler.SetDestination(combatData.Target.transform.position);

        if (node.moveWindow.IsValid(animNormalizedTime))
        {
            // Impulse based on facing direction and move speed curve
        }

        //Quaternion deltaRot = motionHandler.GetRotationDelta();
        //owner.motionAccumulator.AddRotation(deltaRot);

        if (node.attackWindow.IsValid(animNormalizedTime))
        {
            if (!isAttacking)
            {
                weapon.StartAttack(node);
                isAttacking = true;
            }
        }
        else if (isAttacking)
        {
            EndAttack();
        }

        if (node.comboWindow.IsValid(animNormalizedTime))
        {
            Unlock();

            AttackInput chainInput = combatDecision.DecideChain(combatData, combatMemory, node, animNormalizedTime);
            if (chainInput != AttackInput.None)
            {
                node = weapon.NextAttack(owner.Context, chainInput, node);
                SetupAttack();
                return;
            }
        }

        if (animNormalizedTime >= 1f)
        {
            Unlock();

            AttackInput entryInput = combatDecision.DecideEntry(combatData, combatMemory, weapon.comboGraph);
            if (entryInput != AttackInput.None)
            {
                node = weapon.InitiateAttack(owner.Context, entryInput);
                SetupAttack();
                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        owner.Context.dataAggregator.SetAttacking(false);
        weapon.OnHit -= OnWeaponHit;

        motionHandler.rotationMode = RotationMode.FaceMovement;

        if (isAttacking)
        {
            EndAttack();
        }

        combatMemory.ResetAttackStreak();
    }

    private void OnWeaponHit(DamageEvent ev)
    {
        attackSuccessful = true;
        
        owner.Context.dataAggregator.SetAttackStatus(true);
        combatMemory.AttackConnected();
    }

    private void EndAttack()
    {
        weapon.EndAttack();
        isAttacking = false;

        if (!attackSuccessful)
            combatMemory.AttackMiised();
    }

    private float GetStateDuration()
    {
        RuntimeAnimatorController rac = animator.runtimeAnimatorController;

        if (rac is AnimatorOverrideController aoc)
        {
            AnimationClip clip = aoc[node.animClip];
            if (clip != null)
                return clip.length;
        }

        return node.animClip.length;
    }

    public override float Evaluate(CombatPersona persona)
    {
        float score = persona.attackBase;

        bool isCurrent = combatMemory.CurrentState == this;

        if (isCurrent)
        {
            score += persona.attackEntryBonus;

            float decay = persona.attackDecayRate;

            if (combatData.TargetIsAttacking > 0f)
                decay += persona.attackDecayVsThreat;

            if (combatMemory.ConsecutiveSuccessfulAttacks > 0)
                decay -= persona.attackDecayOnHit;

            score -= stateTimer * decay;
        }

        if (combatData.AIIsBehindTarget == 0)
            score += persona.attackBackBonus;

        score += combatData.TargetIsOpen * persona.attackVsOpenBonus;
        score += combatData.TargetIsBlocking * persona.attackVsBlockBonus;

        return score;
    }
}