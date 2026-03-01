using UnityEngine;

/// <summary>
/// Calculates score for different attack types inside AttackSubState.
/// Utility-based, persona-driven, memory-aware.
/// </summary>
public class AttackScoreCalculator
{
    private AttackPersona persona;

    public AttackScoreCalculator(AttackPersona persona)
    {
        this.persona = persona;
    }

    // ============================================================
    // LIGHT ATTACK
    // ============================================================

    public float Light(AICombatData data, AICombatMemory memory)
    {
        var intent = CalculateIntents(data);

        float score =
            (data.AIUnderPressure * persona.lightPressureBonus) +
            (data.TargetIsRecovering * persona.lightRecoveryBonus) +
            (data.IdealRangeScore * persona.lightRangeWeight) +
            (data.AIStaminaNormalized * persona.lightStaminaWeight) +
            (data.AIHasInitiative * persona.initiativeBonus) +
            (GetFacingScore(data) * persona.facingBonus) +
            (intent.defensive * 0.6f);

        score += ApplyMemoryModifiers(memory);

        score -= AirbornePenalty(data);

        return Mathf.Max(0f, score);
    }

    // ============================================================
    // HEAVY ATTACK
    // ============================================================

    public float Heavy(AICombatData data, AICombatMemory memory)
    {
        var intent = CalculateIntents(data);

        float riskPenalty =
            data.AIHealthNormalized < 0.3f
            ? persona.heavyRiskPenaltyLowHP
            : 0f;

        float score =
            (data.TargetIsOpen * persona.heavyOpenBonus) +
            (data.TargetIsRecovering * persona.heavyRecoveryBonus) +
            (intent.aggressive * 0.8f) +
            ((1f - data.AIUnderPressure) * 0.6f) +
            (data.IdealRangeScore * 0.6f) +
            (data.AIHasInitiative * persona.initiativeBonus) +
            (GetFacingScore(data) * persona.facingBonus) -
            (data.AIUnderPressure * persona.heavyUnderPressurePenalty) -
            riskPenalty;

        score += ApplyMemoryModifiers(memory);

        score -= AirbornePenalty(data);

        return Mathf.Max(0f, score);
    }

    // ============================================================
    // LIGHT HOLD
    // ============================================================

    public float LightHold(AICombatData data, AICombatMemory memory)
    {
        var intent = CalculateIntents(data);

        float score =
            (data.TargetIsBlocking * 0.9f) +
            (intent.aggressive * persona.holdAggressionWeight) +
            (data.TargetIsOpen * 0.5f) +
            (data.AIStaminaNormalized * 0.5f) +
            (data.IdealRangeScore * 0.6f) +
            (data.AIHasInitiative * persona.initiativeBonus) +
            (GetFacingScore(data) * persona.facingBonus);

        score += ApplyMemoryModifiers(memory);

        score -= AirbornePenalty(data);

        return Mathf.Max(0f, score);
    }

    // ============================================================
    // HEAVY HOLD
    // ============================================================

    public float HeavyHold(AICombatData data, AICombatMemory memory)
    {
        var intent = CalculateIntents(data);

        float score =
            (data.TargetIsOpen * 1.2f) +
            (data.TargetIsRecovering * 1.5f) +
            (intent.aggressive * persona.holdAggressionWeight) +
            (data.AIStaminaNormalized * 0.7f) -
            (data.AIUnderPressure * persona.heavyUnderPressurePenalty) +
            (GetFacingScore(data) * persona.facingBonus);

        score += ApplyMemoryModifiers(memory);

        score -= AirbornePenalty(data);

        return Mathf.Max(0f, score);
    }

    // ============================================================
    // INTENT MODEL
    // ============================================================

    private (float aggressive, float defensive, float opportunistic)
        CalculateIntents(AICombatData data)
    {
        float aggressive =
            data.AIStaminaNormalized *
            (1f - data.AIUnderPressure);

        float defensive =
            data.AIUnderPressure *
            (1f - data.AIStaminaNormalized);

        float opportunistic =
            data.TargetVulnerableWindow *
            data.IdealRangeScore;

        return (aggressive, defensive, opportunistic);
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private float GetFacingScore(AICombatData data)
    {
        // FacingAlignment is -1 (behind target) to 1 (in front)
        return Mathf.Clamp01((data.FacingAlignment + 1f) * 0.5f);
    }

    private float AirbornePenalty(AICombatData data)
    {
        return data.AIIsAirborne > 0.5f
            ? persona.airbornePenalty
            : 0f;
    }

    private float ApplyMemoryModifiers(AICombatMemory memory)
    {
        float score = 0f;

        score -= memory.ConsecutiveMissedAttacks * persona.missPenalty;
        score += memory.ConsecutiveSuccessfulAttacks * persona.successMomentumBonus;

        return score;
    }
}