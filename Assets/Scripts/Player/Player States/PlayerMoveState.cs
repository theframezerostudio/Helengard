using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    private Vector2 movement = Vector2.zero;
    private Vector2 smoothedMovement;
    private Vector2 movementVelocity;

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
        float rate = movement.sqrMagnitude > smoothedMovement.sqrMagnitude ? player.acceleration : player.deceleration;

        smoothedMovement = Vector2.SmoothDamp(smoothedMovement, movement, ref movementVelocity, 1f / rate);

        if (movement.sqrMagnitude < 0.1f)
        {
            if (player.Context.Velocity.sqrMagnitude > 165f)
            {
                stateMachine.TransitionToState(new PlayerRecoveryState(stateMachine, player, player.ActionProvider.sprintStop));
            }
            else
            {
                stateMachine.TransitionToState(player.IdleState);
            }

            return;
        }

        player.LocomotionMode.Rotate(smoothedMovement);
        player.LocomotionMode.Move(smoothedMovement, moveSpeed * smoothedMovement.magnitude);
        player.LocomotionMode.PlayAnimation(smoothedMovement);
    }

    public override void Exit()
    {
        base.Exit();

        InputManager.Instance.onMove -= HandleMove;
        player.LocomotionMode.StopAnimation();
    }

    private void HandleMove(Vector2 dir)
    {
        movement = Vector2.ClampMagnitude(dir, 1f);
    }
}
