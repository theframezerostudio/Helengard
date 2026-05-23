using UnityEngine;

public abstract class InteractionConditionDefinition: ScriptableObject
{
    public abstract bool IsMet(InteractionContext context);
}