public sealed class RuntimeAilment
{
    private float recoveryTimer;
    private readonly AilmentDefinition definition;

    public AilmentDefinition Definition => definition;

    public float CurrentBuildup { get; private set; }

    public AilmentState State { get; private set; }

    public float NormalizedBuildup => CurrentBuildup / Definition.Threshold;

    public RuntimeAilment(AilmentDefinition definition)
    {
        this.definition = definition;
    }

    public void AddBuildup(float value)
    {
        CurrentBuildup += value;

        recoveryTimer = Definition.RecoveryDelay;

        if (CurrentBuildup > 0f)
        {
            State = AilmentState.Building;
        }

        if (CurrentBuildup >= Definition.Threshold)
        {
            Trigger();
        }
    }

    public void Tick(float deltaTime)
    {
        if (State == AilmentState.Triggered)
        {
            return;
        }

        if (recoveryTimer > 0f)
        {
            recoveryTimer -= deltaTime;
            return;
        }

        if (CurrentBuildup <= 0f)
        {
            CurrentBuildup = 0f;
            State = AilmentState.Inactive;
            return;
        }

        CurrentBuildup -= Definition.DecayPerSecond * deltaTime;

        if (CurrentBuildup < 0f)
        {
            CurrentBuildup = 0f;
        }
    }

    private void Trigger()
    {
        State = AilmentState.Triggered;
    }

    public void Reset()
    {
        CurrentBuildup = 0f;
        State = AilmentState.Inactive;
    }
}