using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    [field: SerializeField] public string Label {  get; protected set; }
    public abstract bool Evaluate(AICombatData combatContext);
}
