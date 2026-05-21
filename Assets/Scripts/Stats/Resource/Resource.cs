using System;
using static ResourceDefinition;

public sealed class Resource
{
    private float currentValue;

    private readonly RuntimeStat maxStat;
    private readonly ResourceSyncMode syncMode;

    public ResourceDefinition Definition { get; }

    public event Action<Resource, float, float> ValueChanged;
    public event Action<Resource, float, float> MaxValueChanged;
    public event Action<Resource> Depleted;

    public Resource(ResourceDefinition definition, RuntimeStat maxStat)
    {
        Definition = definition;
        this.maxStat = maxStat;
        syncMode = definition.SyncMode;

        currentValue = GetInitialValue(definition);

        ClampToMax();

        maxStat.ValueChanged += OnMaxStatChanged;
    }

    public float CurrentValue => currentValue;
    public float MaxValue => maxStat.Value;

    public float NormalizedValue =>
        MaxValue <= 0f ? 0f : currentValue / MaxValue;

    public bool IsDepleted => currentValue <= 0f;

    public void SetCurrent(float value)
    {
        float oldValue = currentValue;

        currentValue = Clamp(value, 0f, MaxValue);

        if (NearlyEqual(oldValue, currentValue))
            return;

        ValueChanged?.Invoke(this, oldValue, currentValue);

        if (currentValue <= 0f)
            Depleted?.Invoke(this);
    }

    public void Restore(float amount)
    {
        if (amount <= 0f)
            return;

        SetCurrent(currentValue + amount);
    }

    public void Consume(float amount)
    {
        if (amount <= 0f)
            return;

        SetCurrent(currentValue - amount);
    }

    public void Fill()
    {
        SetCurrent(MaxValue);
    }

    public void Empty()
    {
        SetCurrent(0f);
    }

    private float GetInitialValue(ResourceDefinition definition)
    {
        switch (definition.StartMode)
        {
            case ResourceStartMode.Empty:
                return 0f;

            case ResourceStartMode.Full:
                return MaxValue;

            case ResourceStartMode.Custom:
                return definition.CustomStartValue;

            default:
                return MaxValue;
        }
    }

    private void OnMaxStatChanged(RuntimeStat stat, float oldMax, float newMax)
    {
        float oldCurrent = currentValue;

        switch (syncMode)
        {
            case ResourceSyncMode.HardClamp:
                currentValue = Clamp(currentValue, 0f, newMax);
                break;

            case ResourceSyncMode.PreservePercentage:
                PreservePercentage(oldMax, newMax);
                break;

            case ResourceSyncMode.PreserveDelta:
                PreserveDelta(oldMax, newMax);
                break;
        }

        ClampToMax();

        MaxValueChanged?.Invoke(this, oldMax, newMax);

        if (NearlyEqual(oldCurrent, currentValue))
            return;

        ValueChanged?.Invoke(this, oldCurrent, currentValue);

        if (currentValue <= 0f)
            Depleted?.Invoke(this);
    }

    private void PreservePercentage(float oldMax, float newMax)
    {
        if (oldMax <= 0f)
        {
            currentValue = newMax;
            return;
        }

        float normalized = currentValue / oldMax;

        currentValue = normalized * newMax;
    }

    private void PreserveDelta(float oldMax, float newMax)
    {
        float missing = oldMax - currentValue;

        currentValue = newMax - missing;
    }

    private void ClampToMax()
    {
        currentValue = Clamp(currentValue, 0f, MaxValue);
    }

    private static float Clamp(float value, float min, float max)
    {
        if (value < min)
            return min;

        if (value > max)
            return max;

        return value;
    }

    private static bool NearlyEqual(float a, float b)
    {
        return Math.Abs(a - b) < 0.0001f;
    }
}