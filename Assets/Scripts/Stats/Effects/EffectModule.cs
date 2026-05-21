public abstract class EffectModule
{
    protected ActiveEffect Effect { get; private set; }
    protected GameplayEffectController Controller { get; private set; }
    protected StatContainer Stats { get; private set; }
    protected ResourceContainer Resources { get; private set; }

    public void Initialize(ActiveEffect effect,
                           GameplayEffectController controller,
                           StatContainer stats,
                           ResourceContainer resources)
    {
        Effect = effect;
        Controller = controller;
        Stats = stats;
        Resources = resources;

        OnInitialized();
    }

    protected virtual void OnInitialized()
    {
    }

    public virtual void OnApplied()
    {
    }

    public virtual void Tick(float deltaTime)
    {
    }

    public virtual void OnRemoved()
    {
    }
}