using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private Vector2 movement = Vector2.zero;
    private readonly Camera mainCamera;

    private Vector3 currentVelocity = Vector3.zero;

    public PlayerMoveState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
        mainCamera = Camera.main;
    }

    public override void Enter()
    {
        base.Enter();

        InputManager.Instance.onMove += HandleMove;
        movement = InputManager.Instance.MoveInput;
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

        HandleRotation(moveDir);

        currentVelocity = Vector3.Lerp(currentVelocity, moveDir, Time.deltaTime * player.acceleration);
        player.Move(currentVelocity, player.movementSpeed);
        player.SetAnim("Speed", currentVelocity.magnitude, 0.1f);
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

    private void HandleRotation(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        player.transform.rotation = Quaternion.Slerp(
            player.transform.rotation,
            targetRot,
            Time.deltaTime * player.rotationDamping
        );
    }

}
