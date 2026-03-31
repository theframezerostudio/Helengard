using UnityEngine;

public class PlayerRecoveryState: PlayerState
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

        if (actionData.animState != null)
            character.PlayAnim(actionData.animState, 0.1f);

        character.Context.dataAggregator.SetInRecovery(true);

        inputManager.onJump += HandleJump;
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

        character.Context.dataAggregator.SetInRecovery(false);

        inputManager.onJump -= HandleJump;
    }
}
