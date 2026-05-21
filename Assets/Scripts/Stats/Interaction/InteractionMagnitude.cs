using System;
using UnityEngine;
using UnityEngine.LightTransport;

[Serializable]
public sealed class InteractionMagnitude
{
    public float flatValue;

    public StatDefinition sourceStat;
    public float sourceStatMultiplier;

    public StatDefinition targetStat;
    public float targetStatMultiplier;

    public bool usePowerMultiplier = true;

    public bool clampMin;
    public float minValue;

    public bool clampMax;
    public float maxValue;

    public float Resolve(InteractionContext context)
    {
        float value = flatValue;

        if (sourceStat != null && context.Source != null)
            value += context.Source.Stats.GetValue(sourceStat) * sourceStatMultiplier;

        if (targetStat != null && context.Target != null)
            value += context.Target.Stats.GetValue(targetStat) * targetStatMultiplier;

        if (usePowerMultiplier)
            value *= context.PowerMultiplier;

        if (clampMin)
            value = Mathf.Max(value, minValue);

        if (clampMax)
            value = Mathf.Min(value, maxValue);

        return value;
    }
}