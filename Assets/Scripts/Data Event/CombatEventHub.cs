using System;
using UnityEngine;

public sealed class CombatEventHub : MonoBehaviour
{
    public event Action<CombatEventData> EventRaised;

    [SerializeField] private CharacterContext owner;
    [SerializeField] private CombatMemory memory;

    public CharacterContext Owner => owner;
    public CombatMemory Memory => memory;

    public void Raise(
        CombatEventDefinition eventDefinition,
        CharacterContext target = null,
        UnityEngine.Object source = null,
        float value = 0f,
        int comboIndex = 0,
        string actionId = null)
    {
        if (eventDefinition == null)
            return;

        CombatEventData eventData = new CombatEventData(
            eventDefinition,
            owner,
            target,
            source,
            Time.time,
            value,
            comboIndex,
            actionId);

        if (memory != null)
            memory.Record(eventData);

        EventRaised?.Invoke(eventData);
    }
}