using UnityEngine;

[CreateAssetMenu(
    fileName = "PeriodicResourceModifier",
    menuName = "Gameplay/Stats/Modules/Periodic Resource Modifier"
)]
public sealed class PeriodicResourceModifierModuleDefinition : TimedEffectModuleDefinition
{
    [SerializeField] private ResourceDefinition targetResource;
    [SerializeField] private float value;
    [SerializeField] private PeriodicResourceOperation operation = PeriodicResourceOperation.Consume;

    public override EffectModule CreateModule()
    {
        return new PeriodicResourceModifierModule(
            Interval,
            targetResource,
            value,
            operation);
    }
}