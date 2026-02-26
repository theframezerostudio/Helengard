using UnityEngine;
using UnityEngine.AI;

public class Chase_AIAction : AIAction
{
    [SerializeField] private float stoppingDistance;

    private AgentMotionHandler motionHandler;
    private NavMeshAgent agent;
    private Transform target;

    public override void Enter(Character Owner, StateContext stateContext)
    {
        owner = Owner;
        context = stateContext;
        target = context.Target.transform;

        motionHandler = stateContext.MotionHandler;
        agent = stateContext.Agent;

        agent.stoppingDistance = stoppingDistance;

        agent.SetDestination(target.position);
    }

    public override void Tick()
    {
        agent.SetDestination(target.position);

        Vector2 intent = motionHandler.GetMoveIntent();
        owner.Animator.SetFloat("Speed", intent.magnitude, 0.1f, Time.deltaTime);

        Quaternion deltaRotation = motionHandler.GetRotationDelta();
        owner.motionAccumulator.AddRotation(deltaRotation);
    }

    public override void Exit()
    {

    }
}
