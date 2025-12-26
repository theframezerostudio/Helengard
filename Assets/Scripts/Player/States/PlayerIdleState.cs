using System;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
    }

    public override void Enter()
    {
        base.Enter();

        InputManager.Instance.onMove += HandleMove;
        player.PlayAnim("Movement", 0.3f);
    }

    public override void Update()
    {
        base.Update();

        player.SetAnim("Speed", 0f, 0.3f);
    }

    public override void Exit()
    {
        base.Exit();

        InputManager.Instance.onMove -= HandleMove;
    }

    private void HandleMove(Vector2 movementInput)
    {
        if (movementInput.sqrMagnitude > 0.1f)
        {
            stateMachine.TransitionToState(player.MoveState);
        }
    }
}
