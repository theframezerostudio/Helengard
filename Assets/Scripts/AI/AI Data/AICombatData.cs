using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class AICombatData
{
    public Target Target;

    // =========================
    // SNAPSHOT REFERENCES
    // =========================
    public CombatSnapshot AISnapshot { get; private set; }
    public CombatSnapshot TargetSnapshot { get; private set; }

    // =========================
    // SPATIAL DATA (Normalized)
    // =========================
    public float Distance;
    public float DistanceNormalized;           
    public float IdealRangeScore;              
    public float RelativeSpeedNormalized;      

    public float FacingAlignment;              
    public float AIIsBehindTarget;             
    public float TargetIsBehindAI;             

    // =========================
    // AI STATUS
    // =========================
    public float AIHealthNormalized = 1f;
    public float AIStaminaNormalized = 1f;
    public float AIIsAirborne;
    public float AIIsAttacking;
    public float AIIsBlocking;
    public float AIIsTargeted;

    // =========================
    // TARGET STATUS
    // =========================
    public float TargetHealthNormalized = 1f;

    public float TargetIsAttacking;
    public float TargetIsLightAttacking;
    public float TargetIsHeavyAttacking;

    public float TargetTimeInAttackState;
    public float TargetIsBlocking;
    public float TargetIsRecovering;
    public float TargetIsOpen;

    // =========================
    // TACTICAL SIGNALS
    // =========================
    public float AIUnderPressure;              
    public float AIHasInitiative;              
    public float TargetVulnerableWindow;       
    public float ThreatLevel;                  

    // =========================
    // CONFIG
    // =========================
    public float MaxCombatRange = 6f;
    public float IdealCombatRangeRatio = 0.7f;
    public float DesiredRange = 2f;

    private Coroutine resetHitCoroutine;

    // ==============================================================
    // BUILD
    // ==============================================================

    public void Build(CombatSnapshot aiData, CombatSnapshot targetData)
    {
        AISnapshot = aiData;
        TargetSnapshot = targetData;

        BuildSpatial(aiData, targetData);
        BuildAIStatus(aiData);
        BuildTargetStatus(targetData);
        BuildTacticalSignals(aiData, targetData);
    }

    // ==============================================================
    // SPATIAL
    // ==============================================================

    private void BuildSpatial(CombatSnapshot ai, CombatSnapshot target)
    {
        float dist = Vector3.Distance(ai.position, target.position);
        Distance = dist;
        DistanceNormalized = Mathf.Clamp01(dist / MaxCombatRange);

        float idealRange = MaxCombatRange * IdealCombatRangeRatio;
        IdealRangeScore = 1f - Mathf.Clamp01(Mathf.Abs(dist - idealRange) / MaxCombatRange);

        Vector3 relativeVel = ai.velocity - target.velocity;
        RelativeSpeedNormalized = Mathf.Clamp01(relativeVel.magnitude / 10f);

        // Facing
        Vector3 toAI = (ai.position - target.position).normalized;
        Vector3 toTarget = (target.position - ai.position).normalized;

        Vector3 targetForward = target.forward;
        Vector3 aiForward = ai.forward;

        float dotTarget = Vector3.Dot(targetForward, toAI);
        float dotAI = Vector3.Dot(aiForward, toTarget);

        FacingAlignment = dotTarget; // -1 to 1

        AIIsBehindTarget = dotTarget < -0.5f ? 1f : 0f;
        TargetIsBehindAI = dotAI < -0.5f ? 1f : 0f;
    }

    // ==============================================================
    // AI STATUS
    // ==============================================================

    private void BuildAIStatus(CombatSnapshot ai)
    {
        AIHealthNormalized = Mathf.Clamp01(ai.hp);
        AIStaminaNormalized = Mathf.Clamp01(ai.stamina);

        AIIsAirborne = ai.velocity.y > 0.1f ? 1f : 0f;
        AIIsAttacking = ai.isAttacking ? 1f : 0f;
        AIIsBlocking = ai.isDefending ? 1f : 0f;
        AIIsTargeted = ai.isGettingTargeted ? 1f : 0f;
    }

    // ==============================================================
    // TARGET STATUS
    // ==============================================================

    private void BuildTargetStatus(CombatSnapshot target)
    {
        TargetHealthNormalized = Mathf.Clamp01(target.hp);

        TargetIsAttacking = target.isAttacking ? 1f : 0f;
        TargetTimeInAttackState = target.timeInAttackState;
        TargetIsLightAttacking = target.isLightAttacking ? 1f : 0f;
        TargetIsHeavyAttacking = target.isHeavyAttacking ? 1f : 0f;

        TargetIsBlocking = target.isDefending ? 1f : 0f;
        TargetIsRecovering = target.isInRecovery ? 1f : 0f;

        TargetIsOpen =
            (!target.isDefending &&
             !target.isAttacking &&
             !target.isInRecovery) ? 1f : 0f;
    }

    // ==============================================================
    // TACTICAL SIGNALS
    // ==============================================================

    private void BuildTacticalSignals(CombatSnapshot ai, CombatSnapshot target)
    {
        float dist = Vector3.Distance(ai.position, target.position);

        // Pressure
        AIUnderPressure =
            (target.isAttacking && dist < MaxCombatRange * 0.4f) ? 1f : 0f;

        // Initiative
        AIHasInitiative =
            (ai.isAttacking && !target.isAttacking) ? 1f : 0f;

        // Vulnerability
        TargetVulnerableWindow =
            (target.isInRecovery || TargetIsOpen > 0.5f) ? 1f : 0f;

        // Threat level (weighted composite)
        ThreatLevel =
            (TargetIsHeavyAttacking * 0.6f) +
            (TargetIsLightAttacking * 0.3f) +
            (AIUnderPressure * 0.5f);

        ThreatLevel = Mathf.Clamp01(ThreatLevel);
    }
}