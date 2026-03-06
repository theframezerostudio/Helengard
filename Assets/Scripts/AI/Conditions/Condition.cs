using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    [field: SerializeField] public string Label {  get; protected set; }

    protected Character Owner { get; private set; }
    protected AICombatData CombatData { get; private set; }

    public virtual void Initialize(Character owner, AICombatData combatData)
    {
        Owner = owner;
        CombatData = combatData;
    }

    public abstract bool Evaluate();
}
