using UnityEngine;

public enum ResourceConditionValueType
{
    CurrentValue,
    NormalizedValue
}

public enum ResourceConditionComparison
{
    GreaterThanOrEqual,
    LessThanOrEqual
}

[CreateAssetMenu(fileName = "New Resource Condition", menuName = "Attributes/Interaction Conditions/Resource")]
public sealed class ResourceConditionDefinition : InteractionConditionDefinition
{
    public InteractionTarget target = InteractionTarget.Target;

    public ResourceDefinition resource;

    public ResourceConditionValueType valueType;
    public ResourceConditionComparison comparison;

    public float requiredValue;

    public override bool IsMet(InteractionContext context)
    {
        if (context == null || resource == null)
            return false;

        CharacterAttributes attributes = context.Get(target);

        if (attributes == null)
            return false;

        Resource runtimeResource = attributes.Resources.GetResource(resource);

        if (runtimeResource == null)
            return false;

        float value = valueType == ResourceConditionValueType.CurrentValue
            ? runtimeResource.CurrentValue
            : runtimeResource.NormalizedValue;

        return comparison switch
        {
            ResourceConditionComparison.GreaterThanOrEqual => value >= requiredValue,
            ResourceConditionComparison.LessThanOrEqual => value <= requiredValue,
            _ => false,
        };
    }
}