using UnityEngine;

public class EnemySuspendedState : EnemyState
{
    private float startTime;
    private float duration;

    public EnemySuspendedState(StateMachine stateMachine, Character character, float duration) : base(stateMachine, character)
    {
        this.duration = duration;
    }

    public override void Enter()
    {
        base.Enter();

        startTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (startTime + duration <= Time.time)
        {
            character.Unsuspend();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}