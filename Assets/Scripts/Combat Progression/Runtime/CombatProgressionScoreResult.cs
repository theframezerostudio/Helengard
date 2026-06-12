using UnityEngine;

public readonly struct CombatProgressionScoreResult
{
    public readonly string Reason;
    public readonly float BaseAmount;
    public readonly float FinalAmount;
    public readonly float Multiplier;
    public readonly bool IsPenalty;
    public readonly bool IsMeaningful;
    public readonly CombatEventData SourceEvent;
    public readonly float Time;

    public CombatProgressionScoreResult(
        string reason,
        float baseAmount,
        float finalAmount,
        float multiplier,
        bool isPenalty,
        bool isMeaningful,
        CombatEventData sourceEvent,
        float time)
    {
        Reason = reason;
        BaseAmount = baseAmount;
        FinalAmount = finalAmount;
        Multiplier = multiplier;
        IsPenalty = isPenalty;
        IsMeaningful = isMeaningful;
        SourceEvent = sourceEvent;
        Time = time;
    }
}