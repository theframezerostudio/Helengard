using UnityEngine;

[System.Serializable]
public sealed class CombatProgressionDecayProcessor
{
    [SerializeField] private bool recordDecayScoreResult;
    [SerializeField] private float minimumRecordedDecay = 1f;

    public bool Tick(
        float deltaTime,
        CombatProgressionRuntime runtime,
        CombatProgressionProfile profile,
        CombatProgressionStableRankCalculator rankCalculator,
        out CombatProgressionDecayResult decayResult)
    {
        decayResult = default;

        if (runtime == null || profile == null)
            return false;

        CombatProgressionDecaySettings settings = profile.DecaySettings;

        if (settings == null || !settings.Enabled)
            return false;

        if (deltaTime <= 0f)
            return false;

        if (settings.PauseWhenOutOfCombat && !runtime.IsCombatActive)
            return false;

        float timeSinceMeaningfulAction = runtime.GetTimeSinceMeaningfulAction();

        if (timeSinceMeaningfulAction < settings.DelayBeforeDecay)
            return false;

        float scoreBefore = runtime.CurrentScore;

        if (scoreBefore <= settings.MinimumScoreAfterDecay)
            return false;

        float decayAmount = settings.ScoreDecayPerSecond * deltaTime;

        if (decayAmount <= 0f)
            return false;

        float scoreAfter = Mathf.Max(
            settings.MinimumScoreAfterDecay,
            scoreBefore - decayAmount);

        runtime.SetScore(scoreAfter, profile);

        if (recordDecayScoreResult && decayAmount >= minimumRecordedDecay)
        {
            CombatProgressionScoreResult scoreResult = new CombatProgressionScoreResult(
                "Rank Decay",
                -decayAmount,
                -(scoreBefore - scoreAfter),
                1f,
                true,
                false,
                default,
                Time.time);

            runtime.RecordResult(scoreResult);
        }

        if (rankCalculator == null)
            rankCalculator = new CombatProgressionStableRankCalculator();

        CombatProgressionRankStabilityResult rankResult =
            rankCalculator.UpdateRuntimeRank(runtime, profile);

        decayResult = new CombatProgressionDecayResult(
            true,
            scoreBefore,
            scoreAfter,
            scoreBefore - scoreAfter,
            rankResult);

        return true;
    }
}