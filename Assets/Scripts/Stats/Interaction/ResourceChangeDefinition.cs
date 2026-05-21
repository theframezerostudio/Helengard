using System;
using UnityEngine;

public enum ResourceChangeOperation
{
    Consume,
    Restore,
    Set,
    Fill,
    Empty
}

[Serializable]
public sealed class ResourceChangeDefinition
{
    public InteractionTarget receiver = InteractionTarget.Target;

    public ResourceDefinition resource;
    public ResourceChangeOperation operation;

    [Header("Resolution")]
    public InteractionMagnitude magnitude = new InteractionMagnitude();
    public InteractionChannelDefinition channel;

    [Header("Resistance")]
    public bool applyResistance = true;

    [Header("Critical")]
    public CriticalDefinition critical = new CriticalDefinition();

    [Header("Rules")]
    public bool required;
    public bool ignoreNegative = true;

    public bool Apply(InteractionContext context, InteractionResult result)
    {
        CharacterAttributes attributes = context.Get(receiver);

        if (attributes == null || resource == null)
        {
            if (required)
                result.Blocked = true;

            return false;
        }

        float baseValue = magnitude.Resolve(context);

        if (ignoreNegative && baseValue < 0f)
            baseValue = 0f;

        float resistanceMultiplier = 1f;

        if (applyResistance && channel != null)
            resistanceMultiplier = channel.ResolveResistanceMultiplier(attributes);

        float multiplier = 1f;
        bool isCritical = critical != null && critical.Roll(context, channel, out multiplier);
        float criticalMultiplier = isCritical ? multiplier : 1f;

        float finalValue = baseValue * resistanceMultiplier * criticalMultiplier;

        bool success = ApplyResourceChange(attributes, finalValue);

        result.ResourceChanges.Add(new ResourceChangeResult(
            attributes,
            resource,
            operation,
            channel,
            baseValue,
            resistanceMultiplier,
            isCritical,
            criticalMultiplier,
            finalValue,
            success));

        if (!success && required)
            result.Blocked = true;

        return success;
    }

    private bool ApplyResourceChange(CharacterAttributes attributes, float value)
    {
        switch (operation)
        {
            case ResourceChangeOperation.Consume:
                return attributes.Resources.TryConsume(resource, value);

            case ResourceChangeOperation.Restore:
                attributes.Resources.Restore(resource, value);
                return true;

            case ResourceChangeOperation.Set:
                attributes.Resources.Get(resource)?.SetCurrent(value);
                return true;

            case ResourceChangeOperation.Fill:
                attributes.Resources.Get(resource)?.Fill();
                return true;

            case ResourceChangeOperation.Empty:
                attributes.Resources.Get(resource)?.Empty();
                return true;

            default:
                return false;
        }
    }
}