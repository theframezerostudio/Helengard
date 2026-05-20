using System.Collections.Generic;

public sealed class StatContainer : IStatSource
{
    private readonly Dictionary<StatDefinition, RuntimeStat> stats = new();

    public StatContainer(CharacterStatProfile profile)
    {
        InitializeBaseStats(profile);
        InitializeDerivedStats(profile);
    }

    public bool HasStat(StatDefinition definition)
    {
        return stats.ContainsKey(definition);
    }

    public RuntimeStat GetStat(StatDefinition definition)
    {
        stats.TryGetValue(definition, out RuntimeStat stat);

        return stat;
    }

    public float GetValue(StatDefinition definition, float fallback = 0f)
    {
        RuntimeStat stat = GetStat(definition);

        return stat?.Value ?? fallback;
    }

    private void InitializeBaseStats(CharacterStatProfile profile)
    {
        for (int i = 0; i < profile.BaseStats.Count; i++)
        {
            CharacterStatDefinition definition = profile.BaseStats[i];

            RuntimeStat stat = new RuntimeStat(definition, this);

            stats.Add(definition.Stat, stat);
        }
    }

    private void InitializeDerivedStats(CharacterStatProfile profile)
    {
        for (int i = 0; i < profile.DerivedStats.Count; i++)
        {
            CharacterDerivedStatDefinition definition = profile.DerivedStats[i];

            RuntimeStat stat = new RuntimeStat(definition, this);

            stats.Add(definition.Stat, stat);
        }
    }
}