using UnityEngine;

public class TestCombatProgression : MonoBehaviour
{
    public CombatEventHub eventHub;
    public CombatEventDefinition testEvent;
    public CharacterContext context;

    public int amount;

    [ContextMenu("Test Combat Progression")]
    public void Test()
    {
        eventHub.Raise(testEvent, context, value: amount, actionId: "Test ID");
    }
}
