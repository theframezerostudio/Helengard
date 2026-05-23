using System;
using UnityEngine;

[Serializable]
public sealed class CriticalDefinition
{
    public bool enabled;

    [Header("Chance")]
    [Range(0f, 1f)]
    public float baseChance;

    public StatDefinition sourceChanceStat;
    public float sourceChanceMultiplier = 1f;

    public StatDefinition targetResistanceStat;
    public float targetResistanceMultiplier = 1f;

    [Header("Damage Multiplier")]
    public float baseMultiplier = 1.5f;

    public StatDefinition sourceMultiplierStat;
    public float sourceMultiplierStatMultiplier = 1f;

    public bool Roll(InteractionContext context, out float multiplier)
    {
        multiplier = 1f;

        if (!enabled)
            return false;

        float chance = baseChance;

        if (sourceChanceStat != null && context.Source != null)
            chance += context.Source.Stats.GetValue(sourceChanceStat) * sourceChanceMultiplier;

        if (targetResistanceStat != null && context.Target != null)
            chance -= context.Target.Stats.GetValue(targetResistanceStat) * targetResistanceMultiplier;

        chance = Mathf.Clamp01(chance);

        if (UnityEngine.Random.value > chance)
            return false;

        multiplier = baseMultiplier;

        if (sourceMultiplierStat != null && context.Source != null)
            multiplier += context.Source.Stats.GetValue(sourceMultiplierStat) * sourceMultiplierStatMultiplier;

        multiplier = Mathf.Max(1f, multiplier);

        return true;
    }
}