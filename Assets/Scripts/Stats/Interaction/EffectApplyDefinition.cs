using System;
using UnityEngine;
using UnityEngine.LightTransport;

[Serializable]
public sealed class EffectApplyDefinition
{
    public InteractionTarget receiver = InteractionTarget.Target;

    public EffectDefinition effect;

    [Range(0f, 1f)]
    public float chance = 1f;

    public bool Apply(InteractionContext context, InteractionResult result)
    {
        CharacterAttributes attributes = context.Get(receiver);

        if (attributes == null || effect == null)
            return false;

        if (chance < 1f && UnityEngine.Random.value > chance)
            return false;

        ActiveEffect appliedEffect = attributes.Effects.ApplyEffect(effect);

        result.AppliedEffects.Add(new EffectApplyResult(attributes, effect, appliedEffect));

        return appliedEffect != null;
    }
}