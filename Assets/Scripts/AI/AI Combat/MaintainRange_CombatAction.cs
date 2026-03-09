using UnityEngine;

public class MaintainRange_CombatAction : CombatSubAction
{
    [SerializeField] private float minRange;
    [SerializeField] private float maxRange;
    private float errorMargin = 0.2f;

    private AgentMotionHandler motionHandler;
    private Transform target;
    public override void Enter()
    {
        base.Enter();

        minRange = 0f;
        maxRange = combatData.DesiredRange;

        owner.PlayAnim("Movement", 0.1f);

        target = combatData.Target.transform;
        motionHandler = stateContext.MotionHandler;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Tick()
    {
        //float distance = Vector3.Distance(owner.transform.position, target.position);

        if (combatData.Distance > maxRange)
        {
            motionHandler.SetDestination(target.position);
            motionHandler.SetStoppingDistance(maxRange);
        }
        else if (combatData.Distance < minRange)
        {
            Vector3 dir = (owner.transform.position - target.position).normalized;
            Vector3 retreatPos = owner.transform.position + dir * (minRange - combatData.Distance);

            motionHandler.SetDestination(retreatPos);
            motionHandler.SetStoppingDistance(0f);
        }

        Vector2 intent = motionHandler.GetMoveIntent();
        owner.Animator.SetFloat("Speed", intent.magnitude, 0.1f, Time.deltaTime);

        Quaternion deltaRotation = motionHandler.GetRotationDelta();
        owner.motionAccumulator.AddRotation(deltaRotation);
    }

    public override float Evaluate(CombatPersona persona)
    {
        if (combatData.Distance <= combatData.DesiredRange + errorMargin)
            return 0f;

        Debug.Log(combatData.Distance  +" < " +  combatData.DesiredRange);
        return 1000f;
    }
}
