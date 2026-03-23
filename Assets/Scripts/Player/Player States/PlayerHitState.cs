using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHitState : PlayerState
{
    private DamageEvent damageEvent;

    public PlayerHitState(StateMachine stateMachine, Character character, DamageEvent ev) : base(stateMachine, character)
    {
        damageEvent = ev;
    }

    public override void Enter()
    {
        base.Enter();

        player.LocomotionMode.SetLocomotion(
            MovementMotionPolicy.NoRootMotion, RotationMotionPolicy.NoRotation);

        player.ReactionController.HandleHit(damageEvent);

        inputManager.onAttack += HandleAttack;
        inputManager.onMove += HandleMove;
    }

    public override void Exit()
    {
        base.Exit();

        inputManager.onAttack -= HandleAttack;
        inputManager.onMove -= HandleMove;

        player.LocomotionMode.RestoreLocomotion();
    }

    public override void Update()
    {
        base.Update();

        if (!player.ReactionController.IsReacting)
            SwitchToLocomotion();
    }

    private void HandleAttack(AttackInput input)
    {
        if (player.ReactionController.TryCancel())
        {
            stateMachine.TransitionToState(new PlayerAttackState(stateMachine, character, input));
        }
    }

    private void HandleMove(Vector2 vector)
    {
        if (player.ReactionController.TryCancel())
            SwitchToLocomotion();
    }
}
