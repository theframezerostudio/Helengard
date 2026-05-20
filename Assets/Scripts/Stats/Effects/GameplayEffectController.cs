using System;
using System.Collections.Generic;

public sealed class GameplayEffectController
{
    private readonly List<GameplayEffect> activeEffects = new();
    private readonly StatContainer statContainer;

    public event Action<GameplayEffect> EffectApplied;
    public event Action<GameplayEffect> EffectRemoved;
    public event Action<GameplayEffect> EffectExpired;
    public event Action<GameplayEffect> EffectRefreshed;
    public event Action<GameplayEffect> EffectStacked;

    public GameplayEffectController(StatContainer statContainer)
    {
        this.statContainer = statContainer;
    }

    public IReadOnlyList<GameplayEffect> ActiveEffects => activeEffects;

    public void Tick(float deltaTime)
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            GameplayEffect effect = activeEffects[i];

            effect.Tick(deltaTime);

            if (!effect.IsExpired)
                continue;

            ExpireEffect(effect);
        }
    }

    public GameplayEffect ApplyEffect(EffectDefinition definition)
    {
        if (definition == null)
            return null;

        if (IsBlocked(definition))
            return null;

        GameplayEffect existing = FindEffect(definition);

        if (existing != null)
        {
            HandleStacking(existing);
            return existing;
        }

        GameplayEffect effect = new GameplayEffect(definition);

        effect.InitializeModules(this, statContainer);

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
            GameplayEffect effect = activeEffects[i];

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

    public void RemoveEffect(GameplayEffect effect)
    {
        if (effect == null)
            return;

        effect.OnRemoved();

        RemoveModifiers(effect);

        activeEffects.Remove(effect);

        EffectRemoved?.Invoke(effect);
    }

    private void ExpireEffect(GameplayEffect effect)
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

    private GameplayEffect FindEffect(EffectDefinition definition)
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            GameplayEffect effect = activeEffects[i];

            if (effect.Definition == definition)
            {
                return effect;
            }
        }

        return null;
    }

    private void HandleStacking(GameplayEffect effect)
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

    private void ApplyModifiers(GameplayEffect effect)
    {
        for (int i = 0; i < effect.Definition.Modifiers.Count; i++)
        {
            StatModifierDefinition definition =
                effect.Definition.Modifiers[i];

            RuntimeStat stat =
                statContainer.GetStat(definition.Stat);

            if (stat == null)
                continue;

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

    private void RemoveModifiers(GameplayEffect effect)
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