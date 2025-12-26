using System;
using UnityEngine;

public class PlayerState : BaseState
{
    protected readonly Player player;
    protected readonly Camera mainCamera;

    public PlayerState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
        player = character as Player;
        mainCamera = Camera.main;
    }

    public override void Enter()
    {
    }

    public override void Update()
    {

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
