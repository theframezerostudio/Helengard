using UnityEngine;

public abstract class CombatProgressionRule : ScriptableObject
{
    [SerializeField] private bool enabled = true;

    public bool Enabled => enabled;

    public void EvaluateRule(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        if (!enabled)
            return;

        if (!context.IsValid)
            return;

        if (evaluation == null)
            return;

        if (!CanEvaluate(context, evaluation))
            return;

        OnEvaluate(context, evaluation);
    }

    protected virtual bool CanEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        return true;
    }

    protected abstract void OnEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation);
}