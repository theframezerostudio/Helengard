using UnityEngine;

[System.Serializable]
public sealed class CombatProgressionEvaluator
{
    [SerializeField] private CombatProgressionRuleSet ruleSet;

    public CombatProgressionRuleSet RuleSet => ruleSet;

    public bool Evaluate(
        CombatProgressionSignal signal,
        CombatProgressionRuntime runtime,
        CombatProgressionProfile profile,
        CombatMemory memory,
        out CombatProgressionRuleEvaluation evaluation)
    {
        evaluation = new CombatProgressionRuleEvaluation(signal);

        if (!signal.IsValid)
            return false;

        if (runtime == null || profile == null || ruleSet == null)
            return false;

        CombatProgressionRuleContext context = new (
            signal,
            runtime,
            profile,
            memory);

        for (int i = 0; i < ruleSet.Count; i++)
        {
            if (!ruleSet.TryGetRule(i, out CombatProgressionRule rule))
                continue;

            rule.EvaluateRule(context, evaluation);
        }

        return evaluation.ResultCount > 0;
    }
}