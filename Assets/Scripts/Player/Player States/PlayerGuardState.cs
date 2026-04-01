using UnityEngine;

public class PlayerGuardState : PlayerGroundedState
{
    private float stateTimer;
    private bool isPerfectGuarding;

    public override AbilityTag? RequiredAbility => AbilityTag.Guard;

    public PlayerGuardState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.PlayAnim("Guard", 0.1f);
        stateTimer = Time.time;

        player.Context.dataAggregator.SetDefending(true);

        isPerfectGuarding = true;
        player.Context.isPerfectGuarding = true;
    }

    public override void Update()
    {
        if (!player.Context.IsGuarding)
        {
            SwitchToLocomotion();
            return;
        }

        if (isPerfectGuarding && Time.time > stateTimer + player.perfectGuardWindow)
        {
            isPerfectGuarding = false;
            player.Context.isPerfectGuarding = false;
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.Context.dataAggregator.SetDefending(false);
        player.Context.isPerfectGuarding = false;
    }
}
