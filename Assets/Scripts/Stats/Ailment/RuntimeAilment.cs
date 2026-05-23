using System;
using UnityEngine;

public sealed class RuntimeAilment
{
    public AilmentDefinition Definition { get; }

    public float CurrentBuildup { get; private set; }
    public AilmentState State { get; private set; }

    public float NormalizedBuildup
    {
        get
        {
            if (Definition == null || Definition.Threshold <= 0f)
                return 0f;

            return Mathf.Clamp01(CurrentBuildup / Definition.Threshold);
        }
    }

    private float recoveryDelayTimer;

    public event Action<RuntimeAilment> Triggered;
    public event Action<RuntimeAilment> BuildupCleared;

    public RuntimeAilment(AilmentDefinition definition)
    {
        Definition = definition;
        State = AilmentState.Inactive;
    }

    public void AddBuildup(float amount)
    {
        if (Definition == null || amount <= 0f)
            return;

        recoveryDelayTimer = Definition.RecoveryDelay;
        CurrentBuildup = Mathf.Min(CurrentBuildup + amount, Definition.Threshold);

        if (CurrentBuildup >= Definition.Threshold)
        {
            State = AilmentState.Triggered;
            Triggered?.Invoke(this);
            return;
        }

        State = AilmentState.Building;
    }

    public void Tick(float deltaTime)
    {
        if (State != AilmentState.Building)
            return;

        if (recoveryDelayTimer > 0f)
        {
            recoveryDelayTimer -= deltaTime;
            return;
        }

        CurrentBuildup = Mathf.Max(0f, CurrentBuildup - Definition.DecayPerSecond * deltaTime);

        if (CurrentBuildup > 0f)
            return;

        State = AilmentState.Inactive;
        BuildupCleared?.Invoke(this);
    }

    public void Reset()
    {
        CurrentBuildup = 0f;
        recoveryDelayTimer = 0f;
        State = AilmentState.Inactive;
    }
}