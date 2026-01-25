using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //enemy.PlayAnim("Idle", 0.3f);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
