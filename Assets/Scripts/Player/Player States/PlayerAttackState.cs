using System;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private ComboNode node;
    private Vector2 movement;
    private AttackInput attackInput;
    private bool comboAttempted = false;
    private float animNormalizedTime = 0f;
    private Animator animator;
    private float attackTimer = 0f;
    private float animDuration;
    private Vector2 smoothedMovement;
    private Vector2 movementVelocity;
    private bool isAttacking = false;
    private float hoverBaseHeight;
    private float hoverTime;

    public PlayerAttackState(StateMachine stateMachine, Character character, AttackInput attackInput) : base(stateMachine, character)
    {
        this.attackInput = attackInput;
    }

    public PlayerAttackState(StateMachine stateMachine, Character character, ComboNode node) : base(stateMachine, character)
    {
        this.node = node;
    }

    public override void Enter()
    {
        base.Enter();

        // First try to use the provided node, if any. If not, resolve based on attack input.
        if (node == null)
        {
            node = player.Context.attackResolver.comboGraph.GetEntryNode(player.Context, attackInput);

            if (node == null)
            {
                Debug.LogWarning("No valid entry node found for attack input: " + attackInput);
                SwitchToLocomotion();
                return;
            }
        }

        animator = player.Animator;

        // Handling air attack setup and Hover Float initialization
        hoverBaseHeight = player.transform.position.y;
        hoverTime = 0f;

        player.Context.GravityScale = 0f;
        player.verticalVelocity = 0f;

        // Animation Feel Adjustments
        player.FeetIKResolver.SetFeetIk(false);
        player.LocomotionMode.SetLocomotion(node.motionPolicy, node.rotationPolicy);

        player.PlayAnim(node.animationStateName, node.transitionTime);
        animDuration = GetStateDuration();

        // Reset combo and attack state
        comboAttempted = false;
        animNormalizedTime = 0f;
        isAttacking = false;
        
        inputManager.onAttack += HandleAttack;
        character.CurrentWeapon.OnHit += HandleHit;
    }

    public override void Update()
    {
        base.Update();

        if (node == null)
            return;

        attackTimer += Time.deltaTime;
        animNormalizedTime = Mathf.InverseLerp(0, animDuration, attackTimer);

        movement = inputManager.MoveInput;

        // Apply movement forces during the move window
        if (node.moveWindow.IsValid(animNormalizedTime))
        {
            float rate = movement.sqrMagnitude > smoothedMovement.sqrMagnitude ? player.acceleration : player.deceleration;
            smoothedMovement = Vector2.SmoothDamp(smoothedMovement, movement, ref movementVelocity, 1f / rate);

            float t = Mathf.InverseLerp(node.moveWindow.startTime, node.moveWindow.endTime, animNormalizedTime);
            float motionAlpha = node.animMotionSpeed.Evaluate(t);

            Vector3 direction = movement == Vector2.zero ? player.transform.forward : player.LocomotionMode.GetDirection(movement);
            direction.Normalize();

            float forwardDelta = (node.forwardAttackForce * motionAlpha) * Time.deltaTime;
            float upwardDelta = (node.upwardAttackForce * motionAlpha) * Time.deltaTime;

            player.LocomotionMode.AddImpulse(direction, forwardDelta);
            player.LocomotionMode.AddImpulse(player.transform.up, upwardDelta);
            //player.LocomotionMode.Rotate(smoothedMovement);
            SmoothRotate(smoothedMovement);
        }

        // Handle air attack hover float
        if (!player.Context.isGrounded && node.requiresAir)
        {
            ApplyHoverFloat();
        }

        // Handle attack hitbox activation
        if (node.attackWindow.IsValid(animNormalizedTime))
        {
            if (!isAttacking)
            {
                character.CurrentWeapon.StartAttack(node);
                isAttacking = true;
            }
        }
        else if (isAttacking)
        {
            character.CurrentWeapon.EndAttack();
            isAttacking = false;
        }

        // Handle combo input and chain attacks
        if (comboAttempted && node.comboWindow.IsValid(animNormalizedTime))
        {
            ComboNode nextNode = player.Context.attackResolver.Resolve(player.Context, attackInput, node);
            if (nextNode)
            {
                stateMachine.TransitionToState(new PlayerAttackState(stateMachine, player, nextNode));
                return;
            }
            comboAttempted = false;
        }

        // Transition back to locomotion after attack finishes or if player tries to move during cancel window
        if ((node.cancelWindow.IsValid(animNormalizedTime) && inputManager.MoveInput != Vector2.zero) || animNormalizedTime >= 1f)
        {
            SwitchToLocomotion();
        }
    }

    public override void Exit()
    {
        base.Exit();

            Debug.LogWarning("Exiting PlayerAttackState with null node reference.");
        if (node == null)
        {
            return;
        }

        inputManager.onAttack -= HandleAttack;
        character.CurrentWeapon.OnHit -= HandleHit;

        //player.SetGravity(true);
        player.Context.GravityScale = 1f;

        if (character.CurrentWeapon)
            character.CurrentWeapon.EndAttack();

        player.FeetIKResolver.SetFeetIk(true);
    }

    private void HandleAttack(AttackInput attackInput)
    {
        if (node.comboWindow.IsAccepted(animNormalizedTime, 1f))
        {
            this.attackInput = attackInput;
            comboAttempted = true;
        }
    }

    private void HandleHit(DamageEvent ev)
    {
        // TODO: Add a short cooldown timer to prevent multiple hits from the same attack from applying multiple rotations

        Vector3 attackDirection = ev.Defender.position - character.transform.position; 
        attackDirection.y = 0;

        Quaternion lookDirection = Quaternion.LookRotation(attackDirection);
        Quaternion delta = lookDirection * Quaternion.Inverse(character.transform.rotation);

        player.motionAccumulator.AddRotation(delta);
    }

    private void ApplyHoverFloat()
    {
        hoverTime += Time.deltaTime;

        float amplitude = node.amplitude;   
        float frequency = node.frequency;  

        float offset = Mathf.Sin(hoverTime * Mathf.PI * 2f * frequency) * amplitude;

        Vector3 pos = player.transform.position;
        pos.y = hoverBaseHeight + offset;

        player.motionAccumulator.AddExtraDelta(pos - player.transform.position);
    }


    public float GetStateDuration()
    {
        RuntimeAnimatorController rac = animator.runtimeAnimatorController;

        if (rac is AnimatorOverrideController overrideController)
        {
            AnimationClip overriddenClip = overrideController[node.animClip];

            if (overriddenClip != null)
                return overriddenClip.length;
        }

        return node.animClip.length;
    }

    private void SmoothRotate(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
            return;

        Vector3 desiredDir = player.LocomotionMode.GetDirection(input).normalized;

        float influenceTime = Mathf.InverseLerp(node.moveWindow.startTime, node.moveWindow.endTime, animNormalizedTime);

        float turnInfluence = node.turnInfluence.Evaluate(influenceTime);
        float turnSpeed = node.attackTurnSpeed * turnInfluence;

        Quaternion targetRotation = Quaternion.LookRotation(desiredDir);
        Quaternion newRotation = Quaternion.RotateTowards(
            player.transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime * 60f
        );

        Quaternion delta = Quaternion.Inverse(player.transform.rotation) * newRotation;
        player.motionAccumulator.AddRotation(delta);
    }
}
