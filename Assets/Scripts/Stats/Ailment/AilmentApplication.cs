public readonly struct AilmentApplication
{
    public readonly AilmentDefinition Ailment;

    public readonly float Buildup;

    public readonly object Source;

    public AilmentApplication(AilmentDefinition ailment, float buildup, object source)
    {
        Ailment = ailment;
        Buildup = buildup;
        Source = source;
    }
}