using UnityEngine;

public enum AirStateType
{
    Rising,
    Falling,
}

public class PlayerAirState : PlayerState
{
    public override int Priority => 5;

    private readonly InputManager inputManager;
    private AirStateType AirState;
    private float verticalVelocity;
    private float gravity;

    public PlayerAirState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
        inputManager = InputManager.Instance;
    }

    public override void Enter()
    {
        base.Enter();

        player.PlayAnim("Jump");
        verticalVelocity = player.jumpForce;
        gravity = player.gravity;   

        AirState = AirStateType.Rising;
    }

    public override void Update()
    {
        base.Update();

        verticalVelocity += gravity * Time.deltaTime;

        if (verticalVelocity < 0)
        {
            AirState = AirStateType.Falling;
        }

        Vector2 movement = inputManager.MoveInput;
        Vector3 forward = (mainCamera.transform.forward).normalized;
        Vector3 right = (mainCamera.transform.right).normalized;

        forward.y = 0;
        right.y = 0;

        Vector3 moveDir = (movement.x * right) + (movement.y * forward);

        player.LocomotionMode.Rotate(moveDir);
        player.LocomotionMode.Move(moveDir, player.movementSpeed * player.airControlPercent);

        player.Move(Vector3.up, verticalVelocity);

        HandleLand();
    }

    private void HandleLand()
    {
        if (stateMachine.IsTransitioningState) return;

        if (AirState == AirStateType.Falling && player.IsGrounded())
        {
            stateMachine.TransitionToState(player.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
