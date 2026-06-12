using System;

public interface ICombatProgressionReader
{
    event Action<CombatProgressionStateSnapshot> StateChanged;
    event Action<CombatProgressionRankStabilityResult> RankChanged;
    event Action<CombatProgressionApplicationResult> ProgressionApplied;
    event Action<CombatProgressionDecayResult> ProgressionDecayed;

    float CurrentScore { get; }
    CombatRankGrade CurrentRank { get; }
    CombatRankGrade PreviousRank { get; }
    float CurrentMultiplier { get; }
    float RankProgress01 { get; }
    bool IsCombatActive { get; }

    CombatProgressionStateSnapshot CreateSnapshot();
    bool IsAtRank(CombatRankGrade rank);
    bool IsAtLeastRank(CombatRankGrade rank);
    bool TryGetLatestScoreResult(out CombatProgressionScoreResult result);
    bool TryGetRecentScoreResult(int indexFromLatest, out CombatProgressionScoreResult result);
}