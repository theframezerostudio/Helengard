using System;
using UnityEngine;

[Serializable]
public sealed class CombatProgressionEventTuning
{
    [SerializeField] private CombatEventDefinition eventDefinition;
    [SerializeField] private CombatProgressionEventCategory category;
    [SerializeField] private string feedbackLabel;

    [Header("Score")]
    [SerializeField] private float baseScore;
    [SerializeField] private float valueMultiplier;
    [SerializeField] private bool clampScore;
    [SerializeField] private float minScore;
    [SerializeField] private float maxScore = 9999f;

    [Header("Behaviour")]
    [SerializeField] private bool meaningfulAction = true;
    [SerializeField] private bool canIncreaseMultiplier = true;
    [SerializeField] private bool canBreakDecay = true;
    [SerializeField] private bool countsForRepetition = true;
    [SerializeField] private bool resetsMultiplier;

    public CombatEventDefinition EventDefinition => eventDefinition;
    public CombatProgressionEventCategory Category => category;
    public string FeedbackLabel => feedbackLabel;
    public float BaseScore => baseScore;
    public float ValueMultiplier => valueMultiplier;
    public bool ClampScore => clampScore;
    public float MinScore => minScore;
    public float MaxScore => maxScore;
    public bool MeaningfulAction => meaningfulAction;
    public bool CanIncreaseMultiplier => canIncreaseMultiplier;
    public bool CanBreakDecay => canBreakDecay;
    public bool CountsForRepetition => countsForRepetition;
    public bool ResetsMultiplier => resetsMultiplier;

    public float CalculateRawScore(float eventValue)
    {
        float score = baseScore + eventValue * valueMultiplier;

        if (clampScore)
            score = Mathf.Clamp(score, minScore, maxScore);

        return score;
    }

    public bool Matches(CombatEventDefinition eventDefinition)
    {
        return this.eventDefinition != null && this.eventDefinition == eventDefinition;
    }

    public void Validate()
    {
        if (maxScore < minScore)
            maxScore = minScore;

        if (string.IsNullOrEmpty(feedbackLabel))
        {
            if (eventDefinition != null)
                feedbackLabel = eventDefinition.DisplayName;
            else
                feedbackLabel = category.ToString();
        }
    }
}