using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    private Vector2 movement = Vector2.zero;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 velocityHelper;
    private readonly float momentumOffset = 1.5f;

    public PlayerMoveState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
    }

    public override void Enter()
    {
        base.Enter();

        InputManager.Instance.onMove += HandleMove;
        movement = InputManager.Instance.MoveInput;
        player.PlayAnim("Movement", 0.1f);
    }

    public override void Update()
    {
        base.Update();

        Vector3 moveDir = player.LocomotionMode.GetDirection(movement).normalized;
        float moveSpeed = player.Context.isSprinting ? player.sprintSpeed : player.movementSpeed;
        Vector3 targetVelocity = moveDir * moveSpeed;

        //currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * player.acceleration);
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref velocityHelper, 0.2f);

        if (movement.sqrMagnitude < 0.1f)
        {
            if (currentVelocity.magnitude > player.movementSpeed + momentumOffset)
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
