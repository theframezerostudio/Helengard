using UnityEngine;

public readonly struct CombatEventData
{
    public readonly CombatEventDefinition Event;
    public readonly CharacterContext Actor;
    public readonly CharacterContext Target;
    public readonly Object Source;
    public readonly float Time;
    public readonly float Value;
    public readonly int ComboIndex;
    public readonly string ActionId;

    public readonly bool HasEvent => Event != null;

    public CombatEventData(
        CombatEventDefinition eventDefinition,
        CharacterContext actor,
        CharacterContext target,
        Object source,
        float time,
        float value = 0f,
        int comboIndex = 0,
        string actionId = null)
    {
        Event = eventDefinition;
        Actor = actor;
        Target = target;
        Source = source;
        Time = time;
        Value = value;
        ComboIndex = comboIndex;
        ActionId = actionId;
    }
}