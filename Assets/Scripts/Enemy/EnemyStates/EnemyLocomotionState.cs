using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotionState : EnemyState
{
    private readonly AgentMotionHandler motionHandler;
    private readonly NavMeshAgent agent;

    public EnemyLocomotionState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
        motionHandler = enemy.motionHandler;
        agent = enemy.agent;
    }

    public override void Enter()
    {
        base.Enter();

        //agent.SetDestination(enemy.temporaryTargetForNow.position);
        character.PlayAnim("Movement", 0.1f);
    }

    public override void Update()
    {
        base.Update();

        //agent.SetDestination(enemy.temporaryTargetForNow.position);

        Vector2 intent = motionHandler.GetMoveIntent();
        character.Animator.SetFloat("Speed", intent.magnitude, 0.1f, Time.deltaTime);

        Quaternion deltaRotation = motionHandler.GetRotationDelta();
        character.motionAccumulator.AddRotation(deltaRotation);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
