using System;
using System.Collections.Generic;

public sealed class RuntimeStat
{
    private readonly List<StatModifier> modifiers = new();
    private readonly IStatSource statSource;
    private readonly float derivedFlatBonus;
    private readonly bool clampValue;
    private readonly bool useMinValue;
    private readonly float minValue;

    private readonly bool useMaxValue;
    private readonly float maxValue;

    private bool dirty = true;

    private float cachedValue;
    private float baseValue;

    public StatDefinition Definition { get; }

    public event Action<RuntimeStat, float, float> ValueChanged;

    public RuntimeStat(CharacterStatDefinition definition, IStatSource statSource)
    {
        Definition = definition.Stat;

        this.statSource = statSource;

        baseValue = definition.BaseValue;

        clampValue = definition.ClampValue;

        useMinValue = definition.UseMinValue;
        minValue = definition.MinValue;

        useMaxValue = definition.UseMaxValue;
        maxValue = definition.MaxValue;

        SubscribeDependencies();
    }

    public RuntimeStat(CharacterDerivedStatDefinition definition, IStatSource statSource)
    {
        Definition = definition.Stat;

        this.statSource = statSource;

        derivedFlatBonus = definition.FlatBonus;

        clampValue = definition.ClampValue;

        useMinValue = definition.UseMinValue;
        minValue = definition.MinValue;

        useMaxValue = definition.UseMaxValue;
        maxValue = definition.MaxValue;

        SubscribeDependencies();
    }

    public bool IsDerived => Definition.IsDerived;

    public float BaseValue
    {
        get => baseValue;

        set
        {
            if (IsDerived)
                return;

            if (NearlyEqual(baseValue, value))
                return;

            float oldValue = Value;

            baseValue = value;

            MarkDirty();

            NotifyIfChanged(oldValue);
        }
    }

    public float Value
    {
        get
        {
            if (dirty)
                Recalculate();

            return cachedValue;
        }
    }

    public IReadOnlyList<StatModifier> Modifiers =>
        modifiers;

    public void AddModifier(
        StatModifier modifier)
    {
        if (modifier == null)
            return;

        float oldValue = Value;

        modifiers.Add(modifier);

        modifiers.Sort(CompareModifierOrder);

        MarkDirty();

        NotifyIfChanged(oldValue);
    }

    public bool RemoveModifier(
        StatModifier modifier)
    {
        if (modifier == null)
            return false;

        float oldValue = Value;

        bool removed = modifiers.Remove(modifier);

        if (!removed)
            return false;

        MarkDirty();

        NotifyIfChanged(oldValue);

        return true;
    }

    public int RemoveAllModifiersFromSource(
        object source)
    {
        if (source == null)
            return 0;

        float oldValue = Value;

        int removed = modifiers.RemoveAll(x => x.Source == source);

        if (removed <= 0)
            return 0;

        MarkDirty();

        NotifyIfChanged(oldValue);

        return removed;
    }

    private void SubscribeDependencies()
    {
        if (Definition is not DerivedStatDefinition derived)
            return;

        for (int i = 0; i < derived.Contributions.Count; i++)
        {
            StatDefinition dependency = derived.Contributions[i].stat;

            RuntimeStat dependencyStat = statSource.GetStat(dependency);

            if (dependencyStat == null)
                continue;

            dependencyStat.ValueChanged += OnDependencyChanged;
        }
    }

    private void OnDependencyChanged(RuntimeStat stat, float oldValue, float newValue)
    {
        float previousValue = Value;

        MarkDirty();

        NotifyIfChanged(previousValue);
    }

    private void MarkDirty()
    {
        dirty = true;
    }

    private void Recalculate()
    {
        dirty = false;

        float finalValue = EvaluateBaseValue();

        float additivePercentSum = 0f;

        for (int i = 0; i < modifiers.Count; i++)
        {
            StatModifier modifier = modifiers[i];

            switch (modifier.Type)
            {
                case StatModifierType.Flat:

                    finalValue += modifier.Value;

                    break;

                case StatModifierType.AdditivePercent:

                    additivePercentSum += modifier.Value;

                    bool lastAdditive =
                        i + 1 >= modifiers.Count ||
                        modifiers[i + 1].Type != StatModifierType.AdditivePercent;

                    if (lastAdditive)
                    {
                        finalValue *= 1f + additivePercentSum;

                        additivePercentSum = 0f;
                    }

                    break;

                case StatModifierType.MultiplicativePercent:

                    finalValue *= 1f + modifier.Value;

                    break;
            }
        }

        finalValue = ApplyLimits(finalValue);

        cachedValue = finalValue;
    }

    private float EvaluateBaseValue()
    {
        if (Definition is not DerivedStatDefinition derived)
            return baseValue;

        return derived.Evaluate(statSource) + derivedFlatBonus;
    }

    private float ApplyLimits(
        float value)
    {
        if (!clampValue)
            return value;

        if (useMinValue)
        {
            value = MathF.Max(minValue, value);
        }

        if (useMaxValue)
        {
            value = MathF.Min(maxValue, value);
        }

        return value;
    }

    private void NotifyIfChanged(float oldValue)
    {
        float newValue = Value;

        if (NearlyEqual(oldValue, newValue))
            return;

        ValueChanged?.Invoke(this, oldValue, newValue);
    }

    private static int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        return a.Order.CompareTo(b.Order);
    }

    private static bool NearlyEqual(float a, float b)
    {
        return MathF.Abs(a - b) < 0.0001f;
    }
}