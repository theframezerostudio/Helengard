using System.Collections.Generic;
using UnityEngine;

public abstract class CombatSubAction : MonoBehaviour
{
    public string label;

    [Range(0, 10)] public float baseScore = 1f;

    public List<Condition> conditions;

    public virtual float Evaluate(AICombatContext combatContext)
    {
        float score = baseScore;

        foreach (var c in conditions)
        {
            if (!c.Evaluate(combatContext))
                score *= 0.25f;
        }
        return score;
    }

    public abstract void Enter(Character owner, AICombatContext context);
    public abstract void Tick();
    public abstract void Exit();
}