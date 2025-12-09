using UnityEngine;

public class PlayerState : BaseState
{
    protected readonly Player player;

    public PlayerState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
        player = character as Player;
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
}
