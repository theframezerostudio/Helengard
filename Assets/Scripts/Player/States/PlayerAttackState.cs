using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private ComboNode node;
    private Vector2 movement;
    private AttackInput attackInput;
    private bool comboAttempted = false;
    private bool hasAttackStarted;
    private float animNormalizedTime = 0f;
    private Animator animator;

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

        hasAttackStarted = false;

        player.PlayAnim(node.animationStateName, node.transitionTime);

        comboAttempted = false;
        animNormalizedTime = 0f;
        inputManager.onAttack += HandleAttack;
    }

    public override void Update()
    {
        base.Update();

        CheckAnimationState();

        if (!hasAttackStarted)
            return;

        movement = inputManager.MoveInput;

        animNormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

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

        if (node.cancelWindow.IsValid(animNormalizedTime) || animNormalizedTime > 1f)
        {
            SwitchToLocomotion();
        }
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        if (node.moveWindow.IsValid(animNormalizedTime))
        {
            Vector3 direction = movement == Vector2.zero ? player.transform.forward.normalized : player.LocomotionMode.GetDirection(movement).normalized;
            player.Context.MotionAccumulator.AddExtraDelta(node.forwardAttackForce * Time.deltaTime * direction);
            player.LocomotionMode.Rotate(movement);
        }

        player.Context.MotionAccumulator.Consume(node.motionPolicy, node.rotationPolicy, player.transform, out Vector3 move, out Quaternion rot);
        player.DeltaMove(move);
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

    private void CheckAnimationState()
    {
        if (hasAttackStarted)
            return;

        if (animator.IsInTransition(0))
            return;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(node.animationStateName))
        {
            hasAttackStarted = true;
        }
    }
}
