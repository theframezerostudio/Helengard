using System;
using System.Diagnostics;

public static class InteractionRunner
{
    public static InteractionResult Run(InteractionDefinition definition, InteractionContext context)
    {
        InteractionResult result = new();

        if (definition == null || context == null)
        {
            result.Blocked = true;
            return result;
        }

        if (!MeetsConditions(definition, context, result))
            return result;
        
        ApplyResourceChanges(definition, context, result);

        if (result.Blocked)
            return result;

        ApplyEffects(definition, context, result);
        ApplyAilments(definition, context, result);

        return result;
    }

    private static bool MeetsConditions(InteractionDefinition definition, InteractionContext context, InteractionResult result)
    {
        if (definition.conditions == null)
            return true;

        for (int i = 0; i < definition.conditions.Length; i++)
        {
            InteractionConditionDefinition condition = definition.conditions[i];

            if (condition == null)
                continue;

            if (condition.IsMet(context))
                continue;

            result.Blocked = true;
            result.FailedCondition = condition;
            return false;
        }

        return true;
    }

    private static void ApplyResourceChanges(InteractionDefinition definition, InteractionContext context, InteractionResult result)
    {
        if (definition.resourceChanges == null)
            return;

        for (int i = 0; i < definition.resourceChanges.Length; i++)
        {
            ResourceChangeDefinition change = definition.resourceChanges[i];

            if (change == null)
                continue;

            change.Apply(context, result);

            if (result.Blocked)
                return;
        }
    }

    private static void ApplyEffects(InteractionDefinition definition, InteractionContext context, InteractionResult result)
    {
        if (definition.effects == null)
            return;

        for (int i = 0; i < definition.effects.Length; i++)
        {
            EffectApplyDefinition effect = definition.effects[i];

            if (effect == null)
                continue;

            effect.Apply(context, result);
        }
    }

    private static void ApplyAilments(InteractionDefinition definition, InteractionContext context, InteractionResult result)
    {
        if (definition.ailments == null)
            return;

        for (int i = 0; i < definition.ailments.Length; i++)
        {
            AilmentApplyDefinition ailment = definition.ailments[i];

            if (ailment == null)
                continue;

            ailment.Apply(context, result);
        }
    }
}