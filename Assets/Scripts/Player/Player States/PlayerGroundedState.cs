using System;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected float lastTimeDashed = Mathf.NegativeInfinity;
    
    public PlayerGroundedState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
    }

    public override void Enter()
    {
        base.Enter();

        InputManager.Instance.onJump += HandleJump;
        InputManager.Instance.onDash += HandleDash;
        InputManager.Instance.onGuard += HandleGuard;
        InputManager.Instance.onAttack += HandleAttack;
    }

    private void HandleGuard(bool guardActive)
    {
        if (guardActive)
        {
            stateMachine.TransitionToState(player.GuardState);
        }
    }

    private void HandleDash()
    {
        if (player.Context.CanDash())
        {
            player.Context.abilitySystem.UseAbility(AbilityType.Dash);
            stateMachine.TransitionToState(player.DashState);
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

    private void HandleAttack(AttackInput input)
    {
        stateMachine.TransitionToState(new PlayerAttackState(stateMachine, character, input));
    }

    public override void Update()
    {
        base.Update();

        if (!player.Context.isGrounded)
        {
            if (player.verticalVelocity < player.groundSnapForce + -0.5f && player.Context.UngroundedTime > 0.05f)
            {
                Debug.Log(player.verticalVelocity + " " + player.Context.UngroundedTime);
                stateMachine.TransitionToState(new PlayerAirState(stateMachine, character));
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        InputManager.Instance.onJump -= HandleJump;
        InputManager.Instance.onDash -= HandleDash;
        InputManager.Instance.onGuard -= HandleGuard;
        InputManager.Instance.onAttack -= HandleAttack;
    }
}
