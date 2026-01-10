using System;
using UnityEngine;

public class PlayerState : BaseState
{
    protected readonly Player player;
    protected readonly Camera mainCamera;
    protected readonly InputManager inputManager;

    public PlayerState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
        player = character as Player;
        mainCamera = Camera.main;
        inputManager = InputManager.Instance;
    }

    public override void Enter()
    {
    }

    public override void Update()
    {

    }

    public override void LateUpdate()
    {
        player.Context.MotionAccumulator.Consume(out Vector3 moveDelta, out Quaternion rotDelta);

        float dt = Time.deltaTime;
        if (dt > 0f)
        {
            player.Context.horizontalVelocity = moveDelta / dt;
        }

        player.DeltaMove(moveDelta);
        player.DeltaRotate(rotDelta);
    }

    public override void Exit()
    {
    }

    public override void OnTriggerEnter(Collider other)
    {
    }

    public override void OnTriggerExit(Collider other)
    {
    }

    public override void OnTriggerStay(Collider other)
    {
    }

    protected void SwitchToLocomotion()
    {
        if (!player.Context.isGrounded)
        {
            stateMachine.TransitionToState(player.AirState);
            return;
        }
        
        if (InputManager.Instance.MoveInput == Vector2.zero)
        {
            stateMachine.TransitionToState(player.IdleState);
        }
        else
        {
            stateMachine.TransitionToState(player.MoveState);
        }
    }
}
