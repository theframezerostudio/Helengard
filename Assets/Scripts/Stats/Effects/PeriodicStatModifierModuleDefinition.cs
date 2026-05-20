using UnityEngine;

[CreateAssetMenu(
    fileName = "PeriodicStatModifier",
    menuName =
        "Gameplay/Stats/Modules/Periodic Stat Modifier"
)]
public sealed class PeriodicStatModifierModuleDefinition : TimedEffectModuleDefinition
{
    [SerializeField]
    private StatDefinition targetStat;

    [SerializeField]
    private float value;

    [SerializeField]
    private PeriodicStatOperation operation = PeriodicStatOperation.Add;

    public override EffectModule CreateModule()
    {
        return new PeriodicStatModifierModule(
            Interval,
            targetStat,
            value,
            operation
            );
    }
}