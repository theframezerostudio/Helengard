using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private Vector2 movement = Vector2.zero;

    private Vector3 currentVelocity = Vector3.zero;

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

        if (movement.sqrMagnitude < 0.1f)
        {
            stateMachine.TransitionToState(player.IdleState);
            return;
        }

        Vector3 forward = (mainCamera.transform.forward).normalized;
        Vector3 right = (mainCamera.transform.right).normalized;

        forward.y = 0;
        right.y = 0;


        Vector3 moveDir = (movement.x * right) + (movement.y * forward);

        currentVelocity = Vector3.Lerp(currentVelocity, moveDir, Time.deltaTime * player.acceleration);
        float targetSpeed = player.Context.isSprinting ? player.sprintSpeed : player.movementSpeed;

        player.LocomotionMode.Rotate(moveDir);
        player.LocomotionMode.Move(currentVelocity, targetSpeed);
        player.LocomotionMode.PlayAnimation(currentVelocity);
    }

    public override void Exit()
    {
        base.Exit();

        InputManager.Instance.onMove -= HandleMove;
    }

    private void HandleMove(Vector2 dir)
    {
        movement = dir;
    }
}
