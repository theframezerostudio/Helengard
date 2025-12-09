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
    }

    private void HandleMove(Vector2 movementInput)
    {
        Debug.Log("Idle State - HandleMove called with input: " + movementInput.sqrMagnitude);
        if (movementInput.sqrMagnitude > 0.1f)
        {
            stateMachine.TransitionToState(player.MoveState);
        }
    }
}
