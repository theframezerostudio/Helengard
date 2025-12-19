using System;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
    }

    public override void Enter()
    {
        base.Enter();

        InputManager.Instance.onMove += HandleMove;
        InputManager.Instance.onJump += HandleJumpInput;
        player.PlayAnim("Movement", 0.1f);
    }

    public override void Update()
    {
        base.Update();
        player.SetAnim("Speed", 0f, 0.1f);
    }

    public override void Exit()
    {
        base.Exit();

        InputManager.Instance.onMove -= HandleMove;
        InputManager.Instance.onJump -= HandleJumpInput;
    }

    private void HandleMove(Vector2 movementInput)
    {
        if (movementInput.sqrMagnitude > 0.1f)
        {
            stateMachine.TransitionToState(player.MoveState);
        }
    }

    private void HandleJumpInput()
    {
        stateMachine.TransitionToState(player.AirState);
    }
}
