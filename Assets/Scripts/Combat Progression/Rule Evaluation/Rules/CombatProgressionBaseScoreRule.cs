using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Progression/Rules/Base Score Rule")]
public sealed class CombatProgressionBaseScoreRule : CombatProgressionRule
{
    [SerializeField] private bool ignoreZeroScore = true;

    protected override bool CanEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        if (!ignoreZeroScore)
            return true;

        return !Mathf.Approximately(context.Signal.RawScore, 0f);
    }

    protected override void OnEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        CombatProgressionSignal signal = context.Signal;

        bool isPenalty = signal.IsPenalty || signal.RawScore < 0f;
        bool applyMultiplier = true;

        if (isPenalty)
        {
            applyMultiplier = context.Profile.MultiplierSettings != null &&
                              context.Profile.MultiplierSettings.ApplyMultiplierToPenalties;
        }

        string reason = signal.FeedbackLabel;

        if (string.IsNullOrEmpty(reason))
            reason = signal.EventDefinition != null ? signal.EventDefinition.DisplayName : "Score";

        evaluation.AddResult(
            CombatProgressionRuleResult.CreateScore(
                reason,
                signal,
                signal.RawScore,
                applyMultiplier,
                isPenalty,
                true,
                signal.MeaningfulAction));
    }
}