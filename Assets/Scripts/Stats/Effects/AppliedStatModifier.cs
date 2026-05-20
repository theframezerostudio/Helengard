public sealed class AppliedStatModifier
{
    public RuntimeStat Stat { get; }

    public StatModifier Modifier { get; }

    public AppliedStatModifier(RuntimeStat stat, StatModifier modifier)
    {
        Stat = stat;
        Modifier = modifier;
    }
}