using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Progression/Rules/Repetition Penalty Rule")]
public sealed class CombatProgressionRepetitionPenaltyRule : CombatProgressionRule
{
    [SerializeField] private string penaltyReason = "Repeated Action";

    protected override bool CanEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        if (context.Memory == null)
            return false;

        if (context.Profile.RepetitionSettings == null)
            return false;

        if (!context.Profile.RepetitionSettings.Enabled)
            return false;

        if (!context.Signal.CountsForRepetition)
            return false;

        if (!context.Signal.HasAction)
            return false;

        if (context.Profile.RepetitionSettings.OnlyPunishPositiveScore && context.Signal.RawScore <= 0f)
            return false;

        return true;
    }

    protected override void OnEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        CombatProgressionRepetitionSettings settings = context.Profile.RepetitionSettings;

        int recentUseCount = context.Memory.CountRecentAction(
            context.Signal.ActionId,
            settings.CheckWindow);

        if (recentUseCount <= settings.AllowedUses)
            return;

        string reason = penaltyReason;

        if (!string.IsNullOrEmpty(context.Signal.ActionId))
            reason = penaltyReason + ": " + context.Signal.ActionId;

        evaluation.AddResult(
            CombatProgressionRuleResult.CreateScore(
                reason,
                context.Signal,
                settings.RepeatedPenalty,
                false,
                true,
                false,
                false));

        if (settings.BlockBaseScoreWhenRepeated)
        {
            evaluation.AddResult(
                CombatProgressionRuleResult.CreateBlockBaseScore(
                    "Blocked Base Score: Repetition",
                    context.Signal));
        }
    }
}