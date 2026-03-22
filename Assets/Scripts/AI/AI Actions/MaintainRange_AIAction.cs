using UnityEngine;

public class MaintainRange_AIAction : AIAction
{
    [SerializeField] private float minRange;
    [SerializeField] private float maxRange;

    private AgentMotionHandler motionHandler;
    private Transform target;

    public override void Enter(Character owner, StateContext stateContext)
    {
        this.owner = owner;
        context = stateContext;

        target = context.Target.transform;
        motionHandler = context.MotionHandler;

        motionHandler.rotationMode = RotationMode.FaceTarget;
    }

    public override void Tick()
    {
        float distance = Vector3.Distance(owner.transform.position, target.position);

        if (distance > maxRange)
        {
            motionHandler.SetDestination(target.position);
            motionHandler.SetStoppingDistance(maxRange);
        }
        else if (distance < minRange)
        {
            Vector3 dir = (owner.transform.position - target.position).normalized;
            Vector3 retreatPos = owner.transform.position + dir * (minRange - distance);

            motionHandler.SetDestination(retreatPos);
            motionHandler.SetStoppingDistance(0f);
        }

        Vector2 intent = motionHandler.GetMoveIntent();
        owner.Animator.SetFloat("Speed", intent.magnitude, 0.1f, Time.deltaTime);

        Quaternion deltaRotation = motionHandler.GetRotationDelta();
        owner.motionAccumulator.AddRotation(deltaRotation);
    }

    public override void Exit() 
    {
        motionHandler.rotationMode = RotationMode.FaceMovement;
    }
}