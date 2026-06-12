public readonly struct CombatProgressionRuleResult
{
    public readonly bool IsValid;

    public readonly string Reason;
    public readonly CombatProgressionSignal SourceSignal;
    public readonly CombatEventData SourceEvent;

    public readonly bool AffectsScore;
    public readonly float ScoreAmount;
    public readonly bool ApplyScoreMultiplier;
    public readonly bool IsPenalty;
    public readonly bool IsBaseScore;

    public readonly bool IsMeaningfulAction;

    public readonly bool AffectsMultiplier;
    public readonly float MultiplierDelta;
    public readonly bool ResetMultiplier;

    public readonly bool BlockBaseScore;

    private CombatProgressionRuleResult(
        string reason,
        CombatProgressionSignal sourceSignal,
        bool affectsScore,
        float scoreAmount,
        bool applyScoreMultiplier,
        bool isPenalty,
        bool isBaseScore,
        bool isMeaningfulAction,
        bool affectsMultiplier,
        float multiplierDelta,
        bool resetMultiplier,
        bool blockBaseScore)
    {
        IsValid = true;

        Reason = reason;
        SourceSignal = sourceSignal;
        SourceEvent = sourceSignal.SourceEvent;

        AffectsScore = affectsScore;
        ScoreAmount = scoreAmount;
        ApplyScoreMultiplier = applyScoreMultiplier;
        IsPenalty = isPenalty;
        IsBaseScore = isBaseScore;

        IsMeaningfulAction = isMeaningfulAction;

        AffectsMultiplier = affectsMultiplier;
        MultiplierDelta = multiplierDelta;
        ResetMultiplier = resetMultiplier;

        BlockBaseScore = blockBaseScore;
    }

    public static CombatProgressionRuleResult CreateScore(
        string reason,
        CombatProgressionSignal sourceSignal,
        float scoreAmount,
        bool applyScoreMultiplier,
        bool isPenalty,
        bool isBaseScore,
        bool isMeaningfulAction)
    {
        return new CombatProgressionRuleResult(
            reason,
            sourceSignal,
            true,
            scoreAmount,
            applyScoreMultiplier,
            isPenalty,
            isBaseScore,
            isMeaningfulAction,
            false,
            0f,
            false,
            false);
    }

    public static CombatProgressionRuleResult CreateMeaningfulAction(
        string reason,
        CombatProgressionSignal sourceSignal)
    {
        return new CombatProgressionRuleResult(
            reason,
            sourceSignal,
            false,
            0f,
            false,
            false,
            false,
            true,
            false,
            0f,
            false,
            false);
    }

    public static CombatProgressionRuleResult CreateMultiplierDelta(
        string reason,
        CombatProgressionSignal sourceSignal,
        float multiplierDelta)
    {
        return new CombatProgressionRuleResult(
            reason,
            sourceSignal,
            false,
            0f,
            false,
            false,
            false,
            false,
            true,
            multiplierDelta,
            false,
            false);
    }

    public static CombatProgressionRuleResult CreateMultiplierReset(
        string reason,
        CombatProgressionSignal sourceSignal)
    {
        return new CombatProgressionRuleResult(
            reason,
            sourceSignal,
            false,
            0f,
            false,
            false,
            false,
            false,
            true,
            0f,
            true,
            false);
    }

    public static CombatProgressionRuleResult CreateBlockBaseScore(
        string reason,
        CombatProgressionSignal sourceSignal)
    {
        return new CombatProgressionRuleResult(
            reason,
            sourceSignal,
            false,
            0f,
            false,
            false,
            false,
            false,
            false,
            0f,
            false,
            true);
    }
}