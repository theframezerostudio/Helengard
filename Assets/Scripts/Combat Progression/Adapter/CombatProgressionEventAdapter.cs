using UnityEngine;

[System.Serializable]
public sealed class CombatProgressionEventAdapter
{
    [SerializeField] private bool logUntunedEvents;

    public bool LogUntunedEvents => logUntunedEvents;

    public bool TryCreateSignal(
        CombatEventData eventData,
        CombatProgressionProfile profile,
        out CombatProgressionSignal signal)
    {
        signal = default;

        if (!eventData.HasEvent)
            return false;

        if (profile == null)
            return false;

        if (!profile.TryGetEventTuning(eventData.Event, out CombatProgressionEventTuning tuning))
        {
            if (logUntunedEvents)
                Debug.LogWarning("Combat progression ignored untuned event: " + eventData.Event.DisplayName);

            return false;
        }

        float rawScore = tuning.CalculateRawScore(eventData.Value);

        signal = new CombatProgressionSignal(eventData, tuning, rawScore);

        return signal.IsValid;
    }

    public bool IsEventTuned(CombatEventDefinition eventDefinition, CombatProgressionProfile profile)
    {
        if (eventDefinition == null || profile == null)
            return false;

        CombatProgressionEventTuning tuning;
        return profile.TryGetEventTuning(eventDefinition, out tuning);
    }
}