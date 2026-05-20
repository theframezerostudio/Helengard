public abstract class TimedEffectModule : EffectModule
{
    private readonly float interval;

    private float timer;

    protected TimedEffectModule(float interval)
    {
        this.interval = interval;
    }

    protected abstract void Execute();

    protected override void OnInitialized()
    {
        timer = interval;
    }

    public override void Tick(float deltaTime)
    {
        timer -= deltaTime;

        if (timer > 0f)
            return;

        Execute();

        timer = interval;
    }
}