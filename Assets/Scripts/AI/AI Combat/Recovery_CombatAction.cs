using UnityEngine;

public class Recovery_CombatAction : CombatSubAction
{
    private AgentMotionHandler motionHandler;
    private Transform target;
    private int moveDir = 0;
    private float repathTimer = 0;

    public override void Enter()
    {
        motionHandler = stateContext.MotionHandler;
        target = combatData.Target.transform;

        combatData.MinDesiredRange = 3f;
        combatData.MaxDesiredRange = 6f;

        repathTimer = 0;

        motionHandler.rotationMode = RotationMode.FaceTarget;

        // Decide movement
        moveDir = DecideStrafe(); // -1 = left, 0 = stay, 1 = right

        owner.PlayAnim("Movement", 0.1f);
        //owner.PlayAnim("Recover", 0.4f);
    }

    public override void Exit()
    {
        // TODO: Pause Memory update on Interrupt
        // Hot Fix
        if (stateTimer > 0.2f)
        {
            combatMemory.LastRecoverExitTime = Time.time;
        }
        motionHandler.rotationMode = RotationMode.FaceMovement;

        base.Exit();
    }

    public override void Tick()
    {
        base.Tick();

        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            repathTimer = 1f; 

            Vector3 toTarget = (target.position - transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, toTarget);
            Vector3 left = -right;

            float strafeDistance = 4f;
            Vector3 destination = transform.position;

            if (moveDir == -1)
                destination = transform.position + left * strafeDistance;
            else if (moveDir == 1)
                destination = transform.position + right * strafeDistance;
            else
                destination = transform.position;

            motionHandler.SetDestination(destination);
            motionHandler.SetStoppingDistance(0.1f);
        }
    }

    // TODO: Bias Weights can be added for increasing chances of a certain output
    private int DecideStrafe()
    {
        float num = Random.Range(-1f, 1f);

        if (num < -0.3f)
            num = 1;
        else if (num > 0.3f) 
            num = -1;
        else
            num = 0;

        return (int)num;
    }

    public override float Evaluate(CombatPersona persona)
    {
        float score = persona.recoverBase;

        bool isCurrent = combatMemory.CurrentState == this;

        if (!isCurrent)
        {
            float idleTime = Time.time - combatMemory.LastRecoverExitTime;
            score += idleTime * persona.recoverTimeGrowth;
        }
        else
        {
            if (combatData.AIIsTargeted == 1)
                score -= persona.recoverDecayOnHit;

            if (combatData.Distance < 0.2f
                && combatData.TargetIsAttacking == 1
                && combatData.AIIsBehindTarget == 0)
            {
                score -= combatData.TargetIsHeavyAttacking
                    == 1 ? persona.recoverDecayOnHeavyAttacks : persona.recoverDecayOnLightAttacks;
            } 

            score += persona.recoverEntryBonus;
            score -= stateTimer * persona.recoverDecayRate;
        }

        return score;
    }
}
