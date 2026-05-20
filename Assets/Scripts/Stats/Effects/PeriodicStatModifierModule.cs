public sealed class PeriodicStatModifierModule : TimedEffectModule
{
    private readonly StatDefinition targetStat;

    private readonly float value;

    private readonly PeriodicStatOperation operation;

    public PeriodicStatModifierModule(float interval,
                                      StatDefinition targetStat,
                                      float value,
                                      PeriodicStatOperation operation) : base(interval)
    {
        this.targetStat = targetStat;
        this.value = value;
        this.operation = operation;
    }

    protected override void Execute()
    {
        RuntimeStat stat = Stats.GetStat(targetStat);

        if (stat == null)
            return;

        switch (operation)
        {
            case PeriodicStatOperation.Add:
                stat.BaseValue += value;
                break;

            case PeriodicStatOperation.Subtract:
                stat.BaseValue -= value;
                break;

            case PeriodicStatOperation.Multiply:
                stat.BaseValue *= value;
                break;

            case PeriodicStatOperation.Divide:
                stat.BaseValue /= value;
                break;

            case PeriodicStatOperation.Set:
                stat.BaseValue = value;
                break;
        }
    }
}