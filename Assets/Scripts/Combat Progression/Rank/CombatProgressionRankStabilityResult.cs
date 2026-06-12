public readonly struct CombatProgressionRankStabilityResult
{
    public readonly CombatRankGrade PreviousRank;
    public readonly CombatRankGrade NewRank;
    public readonly CombatProgressionRankChangeType ChangeType;

    public bool Changed => ChangeType != CombatProgressionRankChangeType.None;
    public bool Promoted => ChangeType == CombatProgressionRankChangeType.Promoted;
    public bool Demoted => ChangeType == CombatProgressionRankChangeType.Demoted;

    public CombatProgressionRankStabilityResult(
        CombatRankGrade previousRank,
        CombatRankGrade newRank,
        CombatProgressionRankChangeType changeType)
    {
        PreviousRank = previousRank;
        NewRank = newRank;
        ChangeType = changeType;
    }
}