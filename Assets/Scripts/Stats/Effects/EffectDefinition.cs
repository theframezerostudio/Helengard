using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "GameplayEffect",
    menuName = "Gameplay/Stats/Gameplay Effect"
)]
public sealed class EffectDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField]
    private string id;

    [SerializeField]
    private string displayName;

    [Header("Duration")]
    [SerializeField]
    private bool infiniteDuration;

    [SerializeField]
    private float duration = 5f;

    [Header("Stacking")]
    [SerializeField]
    private EffectStackingMode stackingMode = EffectStackingMode.RefreshDuration;

    [SerializeField]
    private int maxStacks = 1;

    [Header("Tags")]
    [SerializeField]
    private List<EffectCategory> category = new();

    [SerializeField]
    private List<EffectCategory> blockedByTags = new();

    [Header("Stat Modifiers")]
    [SerializeField]
    private List<StatModifierDefinition> modifiers = new();

    [Header("Modules")]
    [SerializeField]
    private List<EffectModuleDefinition> modules = new();

    public string Id => id;

    public string DisplayName => displayName;

    public bool InfiniteDuration => infiniteDuration;

    public float Duration => duration;

    public EffectStackingMode StackingMode => stackingMode;

    public int MaxStacks => maxStacks;

    public IReadOnlyList<EffectCategory> Categories => category;

    public IReadOnlyList<EffectCategory> BlockedByTags => blockedByTags;

    public IReadOnlyList<StatModifierDefinition> Modifiers => modifiers;

    public IReadOnlyList<EffectModuleDefinition> Modules => modules;
}