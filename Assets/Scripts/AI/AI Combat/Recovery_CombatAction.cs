using UnityEngine;

public class Recovery_CombatAction : CombatSubAction
{
    public override void Enter()
    {
        owner.PlayAnim("Recover", 0.4f);
    }

    public override void Exit()
    {
        base.Exit();

        combatMemory.LastRecoverExitTime = Time.time;
    }

    public override void Tick()
    {
        base.Tick();
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
