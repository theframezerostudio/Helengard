using UnityEngine;

public class PlayerDodgeState : PlayerState
{
    public override AbilityTag? RequiredAbility => AbilityTag.Move;

    public PlayerDodgeState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
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
}
