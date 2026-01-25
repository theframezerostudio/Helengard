using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotionState : EnemyState
{
    private AgentMotionHandler motionHandler;
    private NavMeshAgent agent;

    public EnemyLocomotionState(StateMachine stateMachine, Character character) : base(stateMachine, character)
    {
        motionHandler = enemy.motionHandler;
        agent = enemy.agent;
    }

    public override void Enter()
    {
        base.Enter();

        agent.SetDestination(enemy.temporaryTargetForNow.position);
    }

    public override void Update()
    {
        base.Update();

        agent.SetDestination(enemy.temporaryTargetForNow.position);

        Vector2 intent = motionHandler.GetMoveIntent();
        character.Animator.SetFloat("Speed", intent.magnitude, 0.1f, Time.deltaTime);

        Debug.Log(intent.magnitude);

        Quaternion deltaRotation = motionHandler.GetRotationDelta();
        character.motionAccumulator.AddRotation(deltaRotation);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
