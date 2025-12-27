using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private ComboNode node;
    private float stateTimer;
    private Vector2 movement;

    public PlayerAttackState(StateMachine stateMachine, Character character, ComboNode node) : base(stateMachine, character)
    {
        this.node = node;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 0;

        player.PlayAnim(node.animationStateName, node.transitionTIme);

        inputManager.onAttack += HandleAttack;
    }

    public override void Update()
    {
        base.Update();

        movement = inputManager.MoveInput;
        movement = movement == Vector2.zero ? Vector2.up : movement;

        if (node.moveWindow.IsValid(stateTimer))
        {
            player.LocomotionMode.Move(movement, node.forwardAttackForce);
            player.LocomotionMode.Rotate(movement);
        }
    }

    public override void Exit()
    {
        base.Exit();

        inputManager.onAttack -= HandleAttack;
    }

    private void HandleAttack(AttackInput attackInput)
    {
        if (!node.comboWindow.IsValid(stateTimer))
            return;

        ComboNode nextNode = player.Context.attackResolver.Resolve(player.Context, attackInput, node);

        if (nextNode)
        {
            stateMachine.TransitionToState(new PlayerAttackState(stateMachine, player, nextNode));
        }
    }
}
