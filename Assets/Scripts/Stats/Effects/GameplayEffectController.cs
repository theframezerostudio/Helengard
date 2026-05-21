using System;
using System.Collections.Generic;
using System.Diagnostics;

public sealed class GameplayEffectController
{
    private readonly List<ActiveEffect> activeEffects = new();
    private readonly StatContainer statContainer;
    private readonly ResourceContainer resourceContainer;

    public event Action<ActiveEffect> EffectApplied;
    public event Action<ActiveEffect> EffectRemoved;
    public event Action<ActiveEffect> EffectExpired;
    public event Action<ActiveEffect> EffectRefreshed;
    public event Action<ActiveEffect> EffectStacked;

    public GameplayEffectController(StatContainer statContainer, ResourceContainer resourceContainer)
    {
        this.statContainer = statContainer;
        this.resourceContainer = resourceContainer;
    }

    public IReadOnlyList<ActiveEffect> ActiveEffects => activeEffects;

    public void Tick(float deltaTime)
    {
        if (activeEffects.Count == 0)
            return;

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            ActiveEffect effect = activeEffects[i];

            effect.Tick(deltaTime);

            if (!effect.IsExpired)
                continue;

            ExpireEffect(effect);
        }
    }

    public ActiveEffect ApplyEffect(EffectDefinition definition)
    {
        if (definition == null)
            return null;

        if (IsBlocked(definition))
            return null;

        ActiveEffect existing = FindEffect(definition);

        if (existing != null)
        {
            HandleStacking(existing);
            return existing;
        }

        ActiveEffect effect = new ActiveEffect(definition);

        effect.InitializeModules(this, statContainer, resourceContainer);

        ApplyModifiers(effect);

        effect.OnApplied();

        activeEffects.Add(effect);

        EffectApplied?.Invoke(effect);

        return effect;
    }

    public bool HasEffect(EffectDefinition definition)
    {
        return FindEffect(definition) != null;
    }

    public bool HasCategory(EffectCategory category)
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            ActiveEffect effect = activeEffects[i];

            IReadOnlyList<EffectCategory> categories =
                effect.Definition.Categories;

            for (int j = 0; j < categories.Count; j++)
            {
                if (categories[j] == category)
                    return true;
            }
        }

        return false;
    }

    public void RemoveEffect(ActiveEffect effect)
    {
        if (effect == null)
            return;

        effect.OnRemoved();

        RemoveModifiers(effect);

        activeEffects.Remove(effect);

        EffectRemoved?.Invoke(effect);
    }

    private void ExpireEffect(ActiveEffect effect)
    {
        RemoveEffect(effect);

        EffectExpired?.Invoke(effect);
    }

    private bool IsBlocked(EffectDefinition definition)
    {
        for (int i = 0; i < definition.BlockedByTags.Count; i++)
        {
            EffectCategory category =
                definition.BlockedByTags[i];

            if (HasCategory(category))
                return true;
        }

        return false;
    }

    private ActiveEffect FindEffect(EffectDefinition definition)
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            ActiveEffect effect = activeEffects[i];

            if (effect.Definition == definition)
            {
                return effect;
            }
        }

        return null;
    }

    private void HandleStacking(ActiveEffect effect)
    {
        EffectDefinition definition = effect.Definition;

        switch (definition.StackingMode)
        {
            case EffectStackingMode.None:
                return;

            case EffectStackingMode.RefreshDuration:

                effect.RefreshDuration();

                EffectRefreshed?.Invoke(effect);

                return;

            case EffectStackingMode.StackDuration:

                effect.AddDuration(definition.Duration);

                EffectStacked?.Invoke(effect);

                return;

            case EffectStackingMode.StackIntensity:

                if (!effect.CanAddStack())
                    return;

                effect.AddStack();

                ApplyModifiers(effect);

                EffectStacked?.Invoke(effect);

                return;

            case EffectStackingMode.Replace:

                RemoveEffect(effect);

                ApplyEffect(definition);

                return;
        }
    }

    private void ApplyModifiers(ActiveEffect effect)
    {
        for (int i = 0; i < effect.Definition.Modifiers.Count; i++)
        {
            StatModifierDefinition definition =
                effect.Definition.Modifiers[i];

            RuntimeStat stat =
                statContainer.GetStat(definition.Stat);

            if (stat == null)
            {
                Debug.WriteLine($"Stat '{definition.Stat.name}' not found for modifier in effect '{effect.Definition.name}'.");
                continue;
            }

            StatModifier modifier = new StatModifier(
                definition.Value,
                definition.Type,
                effect);

            stat.AddModifier(modifier);

            effect.AddAppliedModifier(
                new AppliedStatModifier(
                    stat,
                    modifier));
        }
    }

    private void RemoveModifiers(ActiveEffect effect)
    {
        for (int i = 0; i < effect.AppliedModifiers.Count; i++)
        {
            AppliedStatModifier applied =
                effect.AppliedModifiers[i];

            applied.Stat.RemoveModifier(
                applied.Modifier);
        }
    }
}