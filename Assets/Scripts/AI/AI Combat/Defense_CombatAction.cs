using UnityEngine;

public class Defense_CombatAction : CombatSubAction
{
    private AgentMotionHandler motionHandler;

    public override void Enter()
    {
        motionHandler = stateContext.MotionHandler;

        owner.PlayAnim("Guard", 0.1f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Tick()
    {
        base.Tick();
        
        Quaternion deltaRot = motionHandler.GetRotationDelta();
        owner.motionAccumulator.AddRotation(deltaRot);
    }

    public override float Evaluate(CombatPersona persona)
    {
        float score = persona.defendBase;

        bool isCurrent = combatMemory.CurrentState == this;

        bool inFront = combatData.AIIsBehindTarget == 0;
        bool close = combatData.DistanceNormalized < 0.2f;

        if (inFront && close && combatData.TargetIsLightAttacking > 0f)
            score += persona.defendVsLightBonus;

        if (combatData.TargetIsAttacking > 0f)
        {
            float buildup = combatData.TargetTimeInAttackState * persona.defenseAttackBuildupRate;

            buildup = Mathf.Min(buildup, persona.defenseAttackBuildupMax);

            score += buildup;
        }

        if (isCurrent)
        {
            score += persona.defendEntryBonus;

            float decay = persona.defendDecayRate;

            if (combatData.TargetIsOpen > 0f || combatData.TargetIsBlocking > 0f)
                decay += persona.defendDecayWhenSafe;

            if (combatData.TargetIsAttacking > 0f)
                decay -= persona.defendDecayWhenThreat;

            score -= stateTimer * decay;
        }

        return score;
    }
}
