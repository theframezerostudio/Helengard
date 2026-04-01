using UnityEngine;

public class PlayerDashState : PlayerState
{
    public override int Priority => 20;
    public override bool IsCancellable => false;
    private Vector2 moveInput;
    private float stateTimer;

    public override AbilityTag? RequiredAbility => AbilityTag.Move;

    public PlayerDashState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.PlayAnim("Dash", 0.05f);

        player.Context.isDashing = true;

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

        player.LocomotionMode.AddImpulse(moveInput, player.dashSpeed * Time.deltaTime);
    }

    public override void Exit()
    {
        base.Exit();

        player.Context.isDashing = false;
    }
}
