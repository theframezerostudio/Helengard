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
        player.Context.OnGuard += HandleGuard;
    }

    private void HandleGuard(bool isGuarding)
    {
        Debug.Log("Handle Guard in Player State: " + isGuarding);
        if (isGuarding)
        {
            stateMachine.TransitionToState(player.GuardState);
        }
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        player.Context.OnGuard -= HandleGuard;
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
}
