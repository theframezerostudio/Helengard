using UnityEngine;

[CreateAssetMenu(menuName = "AI/Attack Persona")]
public class AttackPersona : ScriptableObject
{
    [Header("General")]
    public float facingBonus = 0.6f;
    public float initiativeBonus = 0.5f;
    public float airbornePenalty = 0.7f;

    [Header("Light Attack Weights")]
    public float lightPressureBonus = 0.7f;
    public float lightRecoveryBonus = 0.4f;
    public float lightRangeWeight = 0.8f;
    public float lightStaminaWeight = 0.4f;

    [Header("Heavy Attack Weights")]
    public float heavyOpenBonus = 1.2f;
    public float heavyRecoveryBonus = 1.4f;
    public float heavyRiskPenaltyLowHP = 0.4f;
    public float heavyUnderPressurePenalty = 0.8f;

    [Header("Hold Variants")]
    public float holdAggressionWeight = 0.9f;

    [Header("Memory Influence")]
    public float missPenalty = 0.5f;
    public float successMomentumBonus = 0.4f;
}