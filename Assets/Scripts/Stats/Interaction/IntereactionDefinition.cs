using UnityEngine;

[CreateAssetMenu(fileName = "New Interaction", menuName = "Attributes/Interaction")]
public sealed class InteractionDefinition : ScriptableObject
{
    public string id;
    public string displayName;

    [Header("Costs / Resource Changes")]
    public ResourceChangeDefinition[] resourceChanges;

    [Header("Effects")]
    public EffectApplyDefinition[] effects;

    [Header("Ailments")]
    public AilmentApplyDefinition[] ailments;
}