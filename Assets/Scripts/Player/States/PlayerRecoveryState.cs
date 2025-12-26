using System;
using UnityEngine;

public class PlayerRecoveryState : PlayerState
{
    private float stateTimer;
    private readonly ActionData actionData;

    public PlayerRecoveryState(StateMachine stateMachine, Character character, ActionData actionData) : base(stateMachine, character)
    {
        this.actionData = actionData;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0;
        player.PlayAnim(actionData.animState, 0.1f);
    }

    public override void Update()
    {
        base.Update();

        stateTimer += Time.deltaTime;
        if (InputManager.Instance.MoveInput.sqrMagnitude > 0 && actionData.cancelWindow.IsValid(stateTimer))
        {
            SwitchToLocomotion();
            return;
        }

        if (stateTimer >= actionData.duration)
        {
            SwitchToLocomotion();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
