using System;
using UnityEngine;
using UnityEngine.LightTransport;

[Serializable]
public sealed class AilmentApplyDefinition
{
    public InteractionTarget receiver = InteractionTarget.Target;

    public AilmentDefinition ailment;
    public InteractionMagnitude buildup = new InteractionMagnitude();

    [Range(0f, 1f)]
    public float chance = 1f;

    public bool Apply(InteractionContext context, InteractionResult result)
    {
        CharacterAttributes attributes = context.Get(receiver);

        if (attributes == null || ailment == null)
            return false;

        if (chance < 1f && UnityEngine.Random.value > chance)
            return false;

        float value = buildup.Resolve(context);

        if (value <= 0f)
            return false;

        attributes.Ailments.ApplyAilment(new AilmentApplication(ailment, value, context.Causer));

        result.AilmentsApplied.Add(new AilmentApplyResult(attributes, ailment, value));

        return true;
    }
}