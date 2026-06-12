using UnityEngine;

[System.Serializable]
public sealed class CombatProgressionScoreApplier
{
    [SerializeField] private bool applyBaseScoreBlocking = true;
    [SerializeField] private bool logAppliedResults;

    public bool Apply(
        CombatProgressionRuleEvaluation evaluation,
        CombatProgressionRuntime runtime,
        CombatProgressionProfile profile,
        CombatProgressionStableRankCalculator rankCalculator,
        out CombatProgressionApplicationResult applicationResult)
    {
        applicationResult = default;

        if (evaluation == null || runtime == null || profile == null)
            return false;

        if (rankCalculator == null)
            rankCalculator = new CombatProgressionStableRankCalculator();

        float scoreBefore = runtime.CurrentScore;
        CombatRankGrade rankBefore = runtime.CurrentRank;
        float multiplierBefore = runtime.CurrentMultiplier;

        bool blockBaseScore = applyBaseScoreBlocking && evaluation.ShouldBlockBaseScore();

        ApplyScoreResults(evaluation, runtime, profile, blockBaseScore);
        ApplyMeaningfulAction(evaluation, runtime);
        ApplyMultiplierResults(evaluation, runtime, profile);

        rankCalculator.UpdateRuntimeRank(runtime, profile);

        applicationResult = new CombatProgressionApplicationResult(
            true,
            scoreBefore,
            runtime.CurrentScore,
            rankBefore,
            runtime.CurrentRank,
            multiplierBefore,
            runtime.CurrentMultiplier);

        return applicationResult.ScoreChanged ||
               applicationResult.RankChanged ||
               applicationResult.MultiplierChanged;
    }

    private void ApplyScoreResults(
        CombatProgressionRuleEvaluation evaluation,
        CombatProgressionRuntime runtime,
        CombatProgressionProfile profile,
        bool blockBaseScore)
    {
        for (int i = 0; i < evaluation.ResultCount; i++)
        {
            CombatProgressionRuleResult result;

            if (!evaluation.TryGetResult(i, out result))
                continue;

            if (!result.AffectsScore)
                continue;

            if (blockBaseScore && result.IsBaseScore)
                continue;

            float multiplier = result.ApplyScoreMultiplier ? runtime.CurrentMultiplier : 1f;
            float finalAmount = result.ScoreAmount * multiplier;

            runtime.AddScore(finalAmount, profile);

            CombatProgressionScoreResult scoreResult = new CombatProgressionScoreResult(
                result.Reason,
                result.ScoreAmount,
                finalAmount,
                multiplier,
                result.IsPenalty,
                result.IsMeaningfulAction,
                result.SourceEvent,
                Time.time);

            runtime.RecordResult(scoreResult);

            if (logAppliedResults)
                Debug.Log("Combat Progression Applied: " + result.Reason + " " + finalAmount);
        }
    }

    private void ApplyMeaningfulAction(
        CombatProgressionRuleEvaluation evaluation,
        CombatProgressionRuntime runtime)
    {
        if (!evaluation.HasMeaningfulAction())
            return;

        runtime.MarkMeaningfulAction(Time.time);
    }

    private void ApplyMultiplierResults(
        CombatProgressionRuleEvaluation evaluation,
        CombatProgressionRuntime runtime,
        CombatProgressionProfile profile)
    {
        for (int i = 0; i < evaluation.ResultCount; i++)
        {
            CombatProgressionRuleResult result;

            if (!evaluation.TryGetResult(i, out result))
                continue;

            if (!result.AffectsMultiplier)
                continue;

            if (result.ResetMultiplier)
            {
                runtime.ResetMultiplier(profile);
                continue;
            }

            runtime.SetMultiplier(runtime.CurrentMultiplier + result.MultiplierDelta, profile);
        }
    }
}