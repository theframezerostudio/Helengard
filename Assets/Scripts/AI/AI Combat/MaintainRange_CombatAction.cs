using UnityEngine;

/// <summary>
/// Maintain AI range from Target in Combat Mode (Combat Scheduler).
/// Range is based on Combat (<param name="DesiredRange"></param) set by Combat Actions.
/// </summary>
public class MaintainRange_CombatAction : CombatSubAction
{
    [SerializeField] private float minRange;
    [SerializeField] private float maxRange;
    [SerializeField] private float errorMargin = 0.2f;
    [SerializeField] private float cooldown = 0.2f;

    private AgentMotionHandler motionHandler;
    private Transform target;

    private float exitTime; 
        
    public override void Enter()
    {
        base.Enter();

        owner.PlayAnim("Movement", 0.1f);

        target = combatData.Target.transform;
        motionHandler = stateContext.MotionHandler;
    }

    public override void Exit()
    {
        base.Exit();

        exitTime = Time.time;
        motionHandler.canRotate = true;
    }

    public override void Tick()
    {
        //float distance = Vector3.Distance(owner.transform.position, target.position);

        if (combatData.Distance > maxRange)
        {
            motionHandler.canRotate = true;

            motionHandler.SetDestination(target.position);
            motionHandler.SetStoppingDistance(maxRange);
        }
        else if (combatData.Distance < minRange)
        {
            motionHandler.canRotate = false;

            Vector3 dir = (owner.transform.position - target.position).normalized;
            float retreatDistance = (minRange - combatData.Distance) + 0.3f;
            Vector3 retreatPos = owner.transform.position + dir * retreatDistance;

            motionHandler.SetDestination(retreatPos);
            motionHandler.SetStoppingDistance(0f);
        }

        //float intent = motionHandler.GetMoveIntent().magnitude;

        //owner.Animator.SetFloat("Speed", intent, 0.1f, Time.deltaTime);
    }

    public override float Evaluate(CombatPersona persona)
    {
        //Debug.Log("Distance : " + combatData.Distance);

        if (Time.time < exitTime + cooldown)
            return 0f;

        minRange = combatData.MinDesiredRange;
        maxRange = combatData.MaxDesiredRange;

        bool inRange = combatData.Distance >= minRange &&
            combatData.Distance <= maxRange + errorMargin;
        
        if (inRange)
            return 0f;

        //Debug.Log(combatData.Distance + " < " + combatData.MaxDesiredRange);
        return 1000f;
    }
}
