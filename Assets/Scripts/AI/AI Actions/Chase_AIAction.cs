using UnityEngine;

public class Chase_AIAction : AIAction
{
    [SerializeField] private float stoppingDistance;

    private AgentMotionHandler motionHandler;
    private Transform target;

    public override void Enter(Character Owner, StateContext stateContext)
    {
        owner = Owner;
        context = stateContext;
        target = context.Target.transform;

        motionHandler = stateContext.MotionHandler;

        motionHandler.SetStoppingDistance(stoppingDistance);
        motionHandler.SetDestination(target.position);
    }

    public override void Tick()
    {
        motionHandler.SetDestination(target.position);

        Vector2 intent = motionHandler.GetMoveIntent();
        owner.Animator.SetAnim("Forward", intent.magnitude, 0.1f);

        Quaternion deltaRotation = motionHandler.GetRotationDelta();
        owner.motionAccumulator.AddRotation(deltaRotation);
    }

    public override void Exit()
    {

    }
}
