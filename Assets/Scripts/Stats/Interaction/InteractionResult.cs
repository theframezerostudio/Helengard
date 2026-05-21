using System.Collections.Generic;

public sealed class InteractionResult
{
    public bool Blocked;

    public readonly List<ResourceChangeResult> ResourceChanges = new();
    public readonly List<EffectApplyResult> AppliedEffects = new();
    public readonly List<AilmentApplyResult> AilmentsApplied = new();

    public bool Succeeded => !Blocked;
}

public readonly struct ResourceChangeResult
{
    public readonly CharacterAttributes Receiver;
    public readonly ResourceDefinition Resource;
    public readonly ResourceChangeOperation Operation;
    public readonly InteractionChannelDefinition Channel;

    public readonly float BaseValue;
    public readonly float ResistanceMultiplier;
    public readonly bool Critical;
    public readonly float CriticalMultiplier;
    public readonly float FinalValue;

    public readonly bool Success;

    public ResourceChangeResult(
        CharacterAttributes receiver,
        ResourceDefinition resource,
        ResourceChangeOperation operation,
        InteractionChannelDefinition channel,
        float baseValue,
        float resistanceMultiplier,
        bool critical,
        float criticalMultiplier,
        float finalValue,
        bool success)
    {
        Receiver = receiver;
        Resource = resource;
        Operation = operation;
        Channel = channel;
        BaseValue = baseValue;
        ResistanceMultiplier = resistanceMultiplier;
        Critical = critical;
        CriticalMultiplier = criticalMultiplier;
        FinalValue = finalValue;
        Success = success;
    }
}

public readonly struct EffectApplyResult
{
    public readonly CharacterAttributes Receiver;
    public readonly EffectDefinition Definition;
    public readonly ActiveEffect Effect;

    public EffectApplyResult(CharacterAttributes receiver, EffectDefinition definition, ActiveEffect effect)
    {
        Receiver = receiver;
        Definition = definition;
        Effect = effect;
    }
}

public readonly struct AilmentApplyResult
{
    public readonly CharacterAttributes Receiver;
    public readonly AilmentDefinition Ailment;
    public readonly float Buildup;

    public AilmentApplyResult(CharacterAttributes receiver, AilmentDefinition ailment, float buildup)
    {
        Receiver = receiver;
        Ailment = ailment;
        Buildup = buildup;
    }
}