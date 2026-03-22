using UnityEngine;

public class Dodge_CombatAction : CombatSubAction
{
    public override void Enter()
    {
        combatData.MinDesiredRange = 0f;
        combatData.MaxDesiredRange = float.PositiveInfinity;

        owner.PlayAnim("Dodge");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Tick()
    {
        base.Tick();
    }

    public override float Evaluate(CombatPersona persona)
    {
        float score = persona.dodgeBase;

        bool isCurrent = combatMemory.CurrentState == this;

        bool inFront = combatData.AIIsBehindTarget == 0;
        bool close = combatData.DistanceNormalized < 0.2f;

        if (inFront && close && combatData.TargetIsHeavyAttacking > 0f)
            score += persona.dodgeVsHeavyBonus;

        if (combatData.TargetIsAttacking > 0f)
        {
            float buildup = combatData.TargetTimeInAttackState * persona.dodgeAttackBuildupRate;

            buildup = Mathf.Min(buildup, persona.dodgeAttackBuildupMax);

            score += buildup;
        }

        if (isCurrent)
        {
            score += persona.dodgeEntryBonus;

            float decay = persona.dodgeDecayRate;

            if (combatData.TargetIsOpen > 0f || combatData.TargetIsBlocking > 0f)
                decay += persona.dodgeDecayWhenSafe;

            if (combatData.TargetIsAttacking > 0f)
                decay -= persona.dodgeDecayWhenThreat;

            score -= stateTimer * decay;
        }

        return score;
    }
}
