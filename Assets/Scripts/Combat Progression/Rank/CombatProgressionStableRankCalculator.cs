using UnityEngine;

[System.Serializable]
public sealed class CombatProgressionStableRankCalculator
{
    public CombatRankGrade CalculateStableRank(
        float score,
        CombatRankGrade currentRank,
        CombatProgressionProfile profile)
    {
        if (profile == null)
            return CombatRankGrade.D;

        CombatRankGrade directRank = profile.GetRankForScore(score);

        if (directRank > currentRank)
            return directRank;

        if (directRank == currentRank)
            return currentRank;

        CombatRankThreshold currentThreshold;

        if (!profile.TryGetRankThreshold(currentRank, out currentThreshold))
            return directRank;

        if (score < currentThreshold.DemoteBelowScore)
            return directRank;

        return currentRank;
    }

    public CombatProgressionRankStabilityResult UpdateRuntimeRank(
        CombatProgressionRuntime runtime,
        CombatProgressionProfile profile)
    {
        if (runtime == null || profile == null)
        {
            return new CombatProgressionRankStabilityResult(
                CombatRankGrade.D,
                CombatRankGrade.D,
                CombatProgressionRankChangeType.None);
        }

        CombatRankGrade previousRank = runtime.CurrentRank;

        CombatRankGrade newRank = CalculateStableRank(
            runtime.CurrentScore,
            previousRank,
            profile);

        runtime.SetRank(newRank);

        CombatProgressionRankChangeType changeType = CombatProgressionRankChangeType.None;

        if (newRank > previousRank)
            changeType = CombatProgressionRankChangeType.Promoted;
        else if (newRank < previousRank)
            changeType = CombatProgressionRankChangeType.Demoted;

        return new CombatProgressionRankStabilityResult(
            previousRank,
            newRank,
            changeType);
    }
}