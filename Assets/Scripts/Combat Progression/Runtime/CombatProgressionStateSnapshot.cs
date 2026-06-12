public readonly struct CombatProgressionStateSnapshot
{
    public readonly float Score;
    public readonly CombatRankGrade CurrentRank;
    public readonly CombatRankGrade PreviousRank;
    public readonly float Multiplier;
    public readonly float RankProgress01;
    public readonly bool IsCombatActive;
    public readonly float LastMeaningfulActionTime;
    public readonly float LastScoreChangeTime;
    public readonly float LastRankChangeTime;

    public CombatProgressionStateSnapshot(
        float score,
        CombatRankGrade currentRank,
        CombatRankGrade previousRank,
        float multiplier,
        float rankProgress01,
        bool isCombatActive,
        float lastMeaningfulActionTime,
        float lastScoreChangeTime,
        float lastRankChangeTime)
    {
        Score = score;
        CurrentRank = currentRank;
        PreviousRank = previousRank;
        Multiplier = multiplier;
        RankProgress01 = rankProgress01;
        IsCombatActive = isCombatActive;
        LastMeaningfulActionTime = lastMeaningfulActionTime;
        LastScoreChangeTime = lastScoreChangeTime;
        LastRankChangeTime = lastRankChangeTime;
    }
}