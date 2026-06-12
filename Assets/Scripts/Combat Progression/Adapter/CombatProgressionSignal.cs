public readonly struct CombatProgressionSignal
{
    public readonly bool IsValid;

    public readonly CombatEventData SourceEvent;
    public readonly CombatProgressionEventTuning Tuning;

    public readonly CombatEventDefinition EventDefinition;
    public readonly CombatProgressionEventCategory Category;
    public readonly string FeedbackLabel;

    public readonly float RawScore;
    public readonly string ActionId;
    public readonly int ComboIndex;

    public readonly bool MeaningfulAction;
    public readonly bool CanIncreaseMultiplier;
    public readonly bool CanBreakDecay;
    public readonly bool CountsForRepetition;
    public readonly bool ResetsMultiplier;

    public bool HasAction => !string.IsNullOrEmpty(ActionId);
    public bool IsPenalty => Category == CombatProgressionEventCategory.Penalty || RawScore < 0f;

    public CombatProgressionSignal(
        CombatEventData sourceEvent,
        CombatProgressionEventTuning tuning,
        float rawScore)
    {
        IsValid = sourceEvent.HasEvent && tuning != null;

        SourceEvent = sourceEvent;
        Tuning = tuning;

        EventDefinition = sourceEvent.Event;
        Category = tuning != null ? tuning.Category : CombatProgressionEventCategory.None;
        FeedbackLabel = tuning != null ? tuning.FeedbackLabel : string.Empty;

        RawScore = rawScore;
        ActionId = sourceEvent.ActionId;
        ComboIndex = sourceEvent.ComboIndex;

        MeaningfulAction = tuning != null && tuning.MeaningfulAction;
        CanIncreaseMultiplier = tuning != null && tuning.CanIncreaseMultiplier;
        CanBreakDecay = tuning != null && tuning.CanBreakDecay;
        CountsForRepetition = tuning != null && tuning.CountsForRepetition;
        ResetsMultiplier = tuning != null && tuning.ResetsMultiplier;
    }
}