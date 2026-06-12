using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Progression/Rules/Multiplier Rule")]
public sealed class CombatProgressionMultiplierRule : CombatProgressionRule
{
    protected override bool CanEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        if (context.Profile.MultiplierSettings == null)
            return false;

        return context.Profile.MultiplierSettings.Enabled;
    }

    protected override void OnEvaluate(
        CombatProgressionRuleContext context,
        CombatProgressionRuleEvaluation evaluation)
    {
        CombatProgressionMultiplierSettings settings = context.Profile.MultiplierSettings;
        CombatProgressionSignal signal = context.Signal;

        if (signal.ResetsMultiplier && settings.ResetOnMultiplierResetEvent)
        {
            evaluation.AddResult(
                CombatProgressionRuleResult.CreateMultiplierReset(
                    "Multiplier Reset",
                    signal));

            return;
        }

        if (signal.IsPenalty)
        {
            if (settings.LossOnPenalty <= 0f)
                return;

            evaluation.AddResult(
                CombatProgressionRuleResult.CreateMultiplierDelta(
                    "Multiplier Penalty",
                    signal,
                    -settings.LossOnPenalty));

            return;
        }

        if (!signal.MeaningfulAction)
            return;

        if (!signal.CanIncreaseMultiplier)
            return;

        if (settings.GainPerMeaningfulAction <= 0f)
            return;

        evaluation.AddResult(
            CombatProgressionRuleResult.CreateMultiplierDelta(
                "Multiplier Gain",
                signal,
                settings.GainPerMeaningfulAction));
    }
}