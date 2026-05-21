public sealed class PeriodicResourceModifierModule : TimedEffectModule
{
    private readonly ResourceDefinition targetResource;
    private readonly float value;
    private readonly PeriodicResourceOperation operation;

    public PeriodicResourceModifierModule(
        float interval,
        ResourceDefinition targetResource,
        float value,
        PeriodicResourceOperation operation) : base(interval)
    {
        this.targetResource = targetResource;
        this.value = value;
        this.operation = operation;
    }

    protected override void Execute()
    {
        Resource resource = Resources.Get(targetResource);

        if (resource == null)
            return;

        switch (operation)
        {
            case PeriodicResourceOperation.Restore:
                resource.Restore(value);
                break;

            case PeriodicResourceOperation.Consume:
                resource.Consume(value);
                break;

            case PeriodicResourceOperation.Set:
                resource.SetCurrent(value);
                break;

            case PeriodicResourceOperation.Fill:
                resource.Fill();
                break;

            case PeriodicResourceOperation.Empty:
                resource.Empty();
                break;
        }
    }
}