using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private Animator animator;
    private ComboNode node;
    private AttackInput attackInput;

    private Vector2 movement;
    private Vector2 smoothedMovement;
    private Vector2 movementVelocity;

    private float animNormalizedTime = 0f;
    private float attackTimer = 0f;
    private float animDuration;
    private float hoverBaseHeight;
    private float hoverTime;

    private bool isAttacking = false;
    private bool comboAttempted = false;

    private Transform motionWarpTarget;

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
            // Checking if air attacks are Possible
            if (!character.Context.isGrounded)
            {
                if (character.Context.airComboDone)
                {
                    SwitchToLocomotion();
                    return;
                }
                else
                {
                    character.Context.airComboDone = true;
                }
            }

            node = player.CurrentWeapon.InitiateAttack(player.Context, attackInput);

            if (node == null)
            {
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

        bool isLightAttack = node.input == AttackInput.Light || node.input == AttackInput.LightHold;
        character.Context.dataAggregator.SetAttacking(true, isLightAttack);

        motionWarpTarget = TargetResolver.ResolveTarget(player, inputManager.MoveInput, node.attackRange,
            character.CurrentWeapon.attackLayer);

        ApplyMotionWarpDash();

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

            //Vector3 direction = movement == Vector2.zero ? player.transform.forward : player.LocomotionMode.GetDirection(movement);
            Vector3 direction;

            if (motionWarpTarget != null)
            {
                direction = motionWarpTarget.position - player.transform.position;
                direction.y = 0;
            }
            else
            {
                direction = movement == Vector2.zero
                    ? player.transform.forward
                    : player.LocomotionMode.GetDirection(movement);
            }

            direction.Normalize();

            direction.Normalize();

            float forwardDelta = (node.forwardAttackForce * motionAlpha) * Time.deltaTime;
            float upwardDelta = (node.upwardAttackForce * motionAlpha) * Time.deltaTime;

            player.LocomotionMode.AddImpulse(direction, forwardDelta);
            player.LocomotionMode.AddImpulse(player.transform.up, upwardDelta);
            //player.LocomotionMode.Rotate(smoothedMovement);

            RotateToTarget();
            //SmoothRotate(smoothedMovement);
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
            ComboNode nextNode = player.CurrentWeapon.NextAttack(player.Context, attackInput, node);
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
            character.Context.dataAggregator.SetAttacking(false);
        }
    }

    public override void Exit()
    {
        base.Exit();

        if (node == null)
        {
            character.Context.dataAggregator.SetAttacking(false);
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
        // TODO: Remove
    }

    private void ApplyMotionWarpDash()
    {
        if (motionWarpTarget == null) return;

        Vector3 toTarget = motionWarpTarget.position - player.transform.position;
        toTarget.y = 0;

        float distance = toTarget.magnitude;

        float idealRange = node.attackRange;
        float warpRange = idealRange * 1.3f;

        if (distance > idealRange && distance < warpRange)
        {
            float dash = distance - idealRange * 0.8f;

            player.LocomotionMode.AddImpulse(
                toTarget.normalized,
                dash
            );
        }
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

    private void RotateToTarget()
    {
        Vector3 desiredDir;

        if (motionWarpTarget != null)
        {
            desiredDir = motionWarpTarget.position - player.transform.position;
            desiredDir.y = 0;
        }
        else if (smoothedMovement.sqrMagnitude > 0.01f)
        {
            desiredDir = player.LocomotionMode.GetDirection(smoothedMovement);
        }
        else
        {
            return;
        }

        desiredDir.Normalize();

        Quaternion targetRot = Quaternion.LookRotation(desiredDir);

        Quaternion newRot = Quaternion.RotateTowards(
            player.transform.rotation,
            targetRot,
            node.attackTurnSpeed * Time.deltaTime * 60f
        );

        Quaternion delta = Quaternion.Inverse(player.transform.rotation) * newRot;
        player.motionAccumulator.AddRotation(delta);
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
