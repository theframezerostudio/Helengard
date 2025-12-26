using UnityEngine;

public class PlayerDashState : PlayerState
{
    public override int Priority => 20;
    public override bool IsCancellable => false;
    private Vector2 moveInput;
    private float stateTimer;

    public PlayerDashState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.PlayAnim("Dash", 0.05f);

        IsCompleted = false;
        stateTimer = Time.time;
        moveInput = InputManager.Instance.MoveInput;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer + player.dashDuration < Time.time)
        {
            IsCompleted = true;
            SwitchToLocomotion();
            return;
        }

        player.LocomotionMode.Move(moveInput, player.dashSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
