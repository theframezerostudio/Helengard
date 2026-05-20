public abstract class EffectModule
{
    protected GameplayEffect Effect { get; private set; }

    protected GameplayEffectController Controller { get; private set; }

    protected StatContainer Stats { get; private set; }

    public void Initialize(GameplayEffect effect, GameplayEffectController controller, StatContainer stats)
    {
        Effect = effect;
        Controller = controller;
        Stats = stats;

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