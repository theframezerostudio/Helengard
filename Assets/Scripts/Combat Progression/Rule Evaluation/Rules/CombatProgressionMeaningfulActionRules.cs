using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Progression/Rules/Meaningful Action Rule")]
public sealed class CombatProgressionMeaningfulActionRule : CombatProgressionRule
{
    protected override bool CanEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        return context.Signal.MeaningfulAction;
    }

    protected override void OnEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        string reason = context.Signal.FeedbackLabel;

        if (string.IsNullOrEmpty(reason))
            reason = "Meaningful Action";

        evaluation.AddResult(
            CombatProgressionRuleResult.CreateMeaningfulAction(
                reason,
                context.Signal
                )
            );
    }
}