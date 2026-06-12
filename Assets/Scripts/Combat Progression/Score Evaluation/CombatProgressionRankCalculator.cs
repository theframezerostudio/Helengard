using UnityEngine;

[System.Serializable]
public sealed class CombatProgressionRankCalculator
{
    public CombatRankGrade CalculateRank(float score, CombatProgressionProfile profile)
    {
        if (profile == null)
            return CombatRankGrade.D;

        return profile.GetRankForScore(score);
    }

    public bool TryUpdateRuntimeRank(
        CombatProgressionRuntime runtime,
        CombatProgressionProfile profile,
        out CombatRankGrade previousRank,
        out CombatRankGrade newRank)
    {
        previousRank = CombatRankGrade.D;
        newRank = CombatRankGrade.D;

        if (runtime == null || profile == null)
            return false;

        previousRank = runtime.CurrentRank;
        newRank = CalculateRank(runtime.CurrentScore, profile);

        runtime.SetRank(newRank);

        return previousRank != newRank;
    }
}