using UnityEngine;

public class EnemyRecoveryState : EnemyState
{
    private readonly ActionData actionData;
    private float startTime;

    public EnemyRecoveryState(StateMachine stateMachine, Character character, ActionData actionData) : base(stateMachine, character)
    {
        this.actionData = actionData;
    }

    public override void Enter()
    {
        base.Enter();

        character.PlayAnim(actionData.animState, 0.1f);
        startTime = Time.time;
    }

    public override void Update()
    {
        base.Update();
        
        if (startTime + actionData.duration <= Time.time)
        {
            Debug.Log("Enemy recovered from " + actionData.animState);
            // Use resolver to determine next state
            stateMachine.TransitionToState(enemy.LocomotionState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}