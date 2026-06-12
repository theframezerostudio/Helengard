using UnityEngine;

public readonly struct CombatProgressionRuleContext
{
    public readonly CombatProgressionSignal Signal;
    public readonly CombatProgressionRuntime Runtime;
    public readonly CombatProgressionProfile Profile;
    public readonly CombatMemory Memory;
    public readonly float Time;

    public bool IsValid => Signal.IsValid && Runtime != null && Profile != null;

    public CombatProgressionRuleContext(
        CombatProgressionSignal signal,
        CombatProgressionRuntime runtime,
        CombatProgressionProfile profile,
        CombatMemory memory)
    {
        Signal = signal;
        Runtime = runtime;
        Profile = profile;
        Memory = memory;
        Time = UnityEngine.Time.time;
    }
}