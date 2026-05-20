using System;

public enum StatModifierType
{
    Flat = 100,
    AdditivePercent = 200,
    MultiplicativePercent = 300,
}

[Serializable]
public class StatModifier
{
    public float Value { get; }
    public StatModifierType Type { get; }
    public int Order { get; }
    public object Source { get; }

    public StatModifier(float value, StatModifierType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    public StatModifier(float value, StatModifierType type) : this(value, type, (int)type, null) { }

    public StatModifier(float value, StatModifierType type, int order) : this(value, type, order, null) { }

    public StatModifier(float value, StatModifierType type, object source) : this(value, type, (int)type, source) { }
}

