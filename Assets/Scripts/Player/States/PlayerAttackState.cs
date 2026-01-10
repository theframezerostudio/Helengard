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

        animator = player.Animator;

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

        player.LocomotionMode.SetLocomotion(node.motionPolicy, node.rotationPolicy);

        player.PlayAnim(node.animationStateName, node.transitionTime);
        animDuration = GetStateDuration();
        comboAttempted = false;
        animNormalizedTime = 0f;

        inputManager.onAttack += HandleAttack;
    }

    public override void Update()
    {
        base.Update();

        attackTimer += Time.deltaTime;
        animNormalizedTime = Mathf.InverseLerp(0, animDuration, attackTimer);

        movement = inputManager.MoveInput;

        if (node.moveWindow.IsValid(animNormalizedTime))
        {
            float t = Mathf.InverseLerp(node.moveWindow.startTime, node.moveWindow.endTime, animNormalizedTime);
            float motionAlpha = node.animMotionSpeed.Evaluate(t);

            Vector3 direction = movement == Vector2.zero ? player.transform.forward : player.LocomotionMode.GetDirection(movement);
            direction.Normalize();

            float forwardDelta = (node.forwardAttackForce * motionAlpha) * Time.deltaTime;
            float upwardDelta = (node.upwardAttackForce * motionAlpha) * Time.deltaTime;

            player.LocomotionMode.AddImpulse(direction, forwardDelta);
            player.LocomotionMode.AddImpulse(player.transform.up, upwardDelta);

            player.LocomotionMode.Rotate(movement);
        }

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

        if ((node.cancelWindow.IsValid(animNormalizedTime) && inputManager.MoveInput != Vector2.zero) || animNormalizedTime >= 1f)
        {
            SwitchToLocomotion();
        }
    }

    public override void Exit()
    {
        base.Exit();

        inputManager.onAttack -= HandleAttack;
    }

    private void HandleAttack(AttackInput attackInput)
    {
        if (node.comboWindow.IsAccepted(animNormalizedTime, 0.2f))
        {
            this.attackInput = attackInput;
            comboAttempted = true;
        }
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
}
