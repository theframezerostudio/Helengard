using System.Collections.Generic;
using UnityEngine;

public sealed class CombatMemory : MonoBehaviour
{
    [SerializeField] private int maxEvents = 128;

    private readonly List<CombatEventData> events = new ();

    public void Record(CombatEventData eventData)
    {
        events.Add(eventData);

        if (events.Count > maxEvents)
            events.RemoveAt(0);
    }

    public int CountRecent(CombatEventDefinition eventDefinition, float seconds)
    {
        if (eventDefinition == null)
            return 0;

        float minTime = Time.time - seconds;
        int count = 0;

        for (int i = events.Count - 1; i >= 0; i--)
        {
            CombatEventData eventData = events[i];

            if (eventData.Time < minTime)
                break;

            if (eventData.Event == eventDefinition)
                count++;
        }

        return count;
    }

    public bool WasRecent(CombatEventDefinition eventDefinition, float seconds)
    {
        return CountRecent(eventDefinition, seconds) > 0;
    }

    public int CountRecentAction(string actionId, float seconds)
    {
        if (string.IsNullOrEmpty(actionId))
            return 0;

        float minTime = Time.time - seconds;
        int count = 0;

        for (int i = events.Count - 1; i >= 0; i--)
        {
            CombatEventData eventData = events[i];

            if (eventData.Time < minTime)
                break;

            if (eventData.ActionId == actionId)
                count++;
        }

        return count;
    }

    public bool RepeatedActionRecently(string actionId, float seconds, int requiredCount)
    {
        return CountRecentAction(actionId, seconds) >= requiredCount;
    }
}