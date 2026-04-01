using System;
using UnityEngine;

public class PlayerState : BaseState
{
    protected readonly Player player;
    protected readonly Camera mainCamera;
    protected readonly InputManager inputManager;
    //protected readonly CombatDataAggregator dataAggregator;

    private CharacterContext context;

    public PlayerState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
        player = character as Player;
        mainCamera = Camera.main;
        inputManager = InputManager.Instance;
        context = character.Context;
        //dataAggregator = character.Context.dataAggregator;
    }

    #region State Cycle
    public override void Enter()
    {
        player.Target.onHit += HandleHit;
        inputManager.permissionManager.OnPermissionChanged += PermissionCheck;
    }

    public override void Update()
    {

    }

    public override void LateUpdate()
    {
        character.motionAccumulator.Consume(out Vector3 moveDelta, out Quaternion rotDelta);
        
        Vector3 velocity = moveDelta / Time.deltaTime;

        Vector3 roundedVelocity = new Vector3(
        Mathf.Round(velocity.x * 100f) / 100f,
        Mathf.Round(velocity.y * 100f) / 100f,
        Mathf.Round(velocity.z * 100f) / 100f
        );

        context.Velocity = roundedVelocity;

        player.DeltaMove(moveDelta);
        player.DeltaRotate(rotDelta);
    }

    public override void Exit()
    {
        player.Target.onHit -= HandleHit;
        inputManager.permissionManager.OnPermissionChanged -= PermissionCheck;
    }
    #endregion

    #region Trigger Functions
    public override void OnTriggerEnter(Collider other)
    {
    }

    public override void OnTriggerExit(Collider other)
    {
    }

    public override void OnTriggerStay(Collider other)
    {
    }
    #endregion
    
    private void PermissionCheck(AbilityTag tag, bool isAllowed)
    {
        Debug.Log($"Permission changed for {tag}: {(isAllowed ? "Allowed" : "Denied")}");
        if (!isAllowed && tag == RequiredAbility)
        {
            stateMachine.ForceState(player.IdleState);
        }
    }

    private void HandleHit(DamageEvent ev)
    {
        stateMachine.TransitionToState(new PlayerHitState(stateMachine, character, ev), true);
    }

    protected void SwitchToLocomotion()
    {
        if (!player.Context.isGrounded)
        {
            stateMachine.ForceState(new PlayerAirState(stateMachine, player, null));
            return;
        }
        
        if (InputManager.Instance.MoveInput == Vector2.zero)
        {
            stateMachine.ForceState(player.IdleState);
        }
        else
        {
            stateMachine.ForceState(player.MoveState);
        }
    }
}
