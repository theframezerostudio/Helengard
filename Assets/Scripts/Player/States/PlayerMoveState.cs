using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    private Vector2 movement = Vector2.zero;

    private readonly float momentumOffset = 1.5f;

    public PlayerMoveState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.LocomotionMode.SetLocomotion(MovementMotionPolicy.FullRootMotion, RotationMotionPolicy.YawOnly);

        InputManager.Instance.onMove += HandleMove;
        movement = InputManager.Instance.MoveInput;

        player.PlayAnim("Movement", 0.1f);
    }

    public override void Update()
    {
        base.Update();

        float moveSpeed = player.Context.isSprinting ? player.sprintSpeed : player.movementSpeed;

        if (movement.sqrMagnitude < 0.1f)
        {
            if (player.Context.horizontalVelocity.sqrMagnitude > 20f)
            {
                stateMachine.TransitionToState(new PlayerRecoveryState(stateMachine, player, player.ActionProvider.sprintStop));
            }
            else
            {
                stateMachine.TransitionToState(player.IdleState);
            }

            return;
        }

        player.LocomotionMode.Rotate(movement);
        player.LocomotionMode.Move(movement, moveSpeed * movement.magnitude);
        player.LocomotionMode.PlayAnimation(movement);
    }

    public override void Exit()
    {
        base.Exit();

        InputManager.Instance.onMove -= HandleMove;
        player.LocomotionMode.StopAnimation();
    }

    private void HandleMove(Vector2 dir)
    {
        movement = dir;
    }
}
