public interface IStatSource
{
    bool HasStat(StatDefinition definition);

    RuntimeStat GetStat(StatDefinition definition);

    float GetValue(StatDefinition definition, float fallback = 0f);
}
