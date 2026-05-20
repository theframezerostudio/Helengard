using System;
using UnityEngine;

[Serializable]
public sealed class StatModifierDefinition
{
    [SerializeField]
    private StatDefinition stat;

    [SerializeField]
    private StatModifierType type;

    [SerializeField]
    private float value;

    public StatDefinition Stat => stat;

    public StatModifierType Type => type;

    public float Value => value;
}