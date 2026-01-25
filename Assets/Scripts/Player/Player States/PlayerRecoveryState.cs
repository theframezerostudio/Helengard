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

        player.LocomotionMode.SetLocomotion(MovementMotionPolicy.FullRootMotion, RotationMotionPolicy.YawOnly);

        stateTimer = 0;

        player.PlayAnim(actionData.animState, 0.1f);

        inputManager.onJump += HandleJump;
    }

    public override void Update()
    {
        base.Update();

        stateTimer += Time.deltaTime;
        if (InputManager.Instance.MoveInput.sqrMagnitude > 0 && actionData.cancelWindow.IsValid(stateTimer))
        {
            Debug.Log("Recovery cancelled into locomotion.");
            SwitchToLocomotion();
            return;
        }

        if (stateTimer >= actionData.duration)
        {
            Debug.Log("Recovery completed, switching to locomotion.");
            SwitchToLocomotion();
        }
    }

    private void HandleJump()
    {
        if (player.Context.abilitySystem.TryUse(AbilityType.Jump))
        {
            player.Context.abilitySystem.UseAbility(AbilityType.Jump);

            JumpProfile jumpProfile = player.Context.jumpResolver.Resolve(player.Context);

            stateMachine.TransitionToState(new PlayerAirState(stateMachine, character, jumpProfile));
        }
    }

    public override void Exit()
    {
        base.Exit();

        inputManager.onJump -= HandleJump;
    }
}
