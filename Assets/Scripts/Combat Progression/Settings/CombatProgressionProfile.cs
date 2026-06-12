using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Progression/Combat Progression Profile")]
public sealed class CombatProgressionProfile : ScriptableObject
{
    [Header("Score")]
    [SerializeField] private float startingScore;
    [SerializeField] private float minimumScore;
    [SerializeField] private float maximumScore = 1000f;
    [SerializeField] private CombatRankGrade startingRank = CombatRankGrade.D;

    [Header("Ranks")]
    [SerializeField] private CombatRankThreshold[] rankThresholds;

    [Header("Event Tuning")]
    [SerializeField] private CombatProgressionEventTuning[] eventTunings;

    [Header("Global Settings")]
    [SerializeField] private CombatProgressionRepetitionSettings repetitionSettings = new CombatProgressionRepetitionSettings();
    [SerializeField] private CombatProgressionMultiplierSettings multiplierSettings = new CombatProgressionMultiplierSettings();
    [SerializeField] private CombatProgressionDecaySettings decaySettings = new CombatProgressionDecaySettings();

    public float StartingScore => startingScore;
    public float MinimumScore => minimumScore;
    public float MaximumScore => maximumScore;
    public CombatRankGrade StartingRank => startingRank;
    public CombatProgressionRepetitionSettings RepetitionSettings => repetitionSettings;
    public CombatProgressionMultiplierSettings MultiplierSettings => multiplierSettings;
    public CombatProgressionDecaySettings DecaySettings => decaySettings;

    public float ClampScore(float score)
    {
        return Mathf.Clamp(score, minimumScore, maximumScore);
    }

    public CombatRankGrade GetRankForScore(float score)
    {
        CombatRankGrade result = startingRank;
        float bestScore = float.MinValue;

        if (rankThresholds == null)
            return result;

        for (int i = 0; i < rankThresholds.Length; i++)
        {
            CombatRankThreshold threshold = rankThresholds[i];

            if (threshold == null)
                continue;

            if (score >= threshold.EnterScore && threshold.EnterScore >= bestScore)
            {
                bestScore = threshold.EnterScore;
                result = threshold.Rank;
            }
        }

        return result;
    }

    public bool TryGetRankThreshold(CombatRankGrade rank, out CombatRankThreshold result)
    {
        result = null;

        if (rankThresholds == null)
            return false;

        for (int i = 0; i < rankThresholds.Length; i++)
        {
            CombatRankThreshold threshold = rankThresholds[i];

            if (threshold == null)
                continue;

            if (threshold.Rank == rank)
            {
                result = threshold;
                return true;
            }
        }

        return false;
    }

    public bool TryGetEventTuning(CombatEventDefinition eventDefinition, out CombatProgressionEventTuning result)
    {
        result = null;

        if (eventDefinition == null || eventTunings == null)
            return false;

        for (int i = 0; i < eventTunings.Length; i++)
        {
            CombatProgressionEventTuning tuning = eventTunings[i];

            if (tuning == null)
                continue;

            if (tuning.Matches(eventDefinition))
            {
                result = tuning;
                return true;
            }
        }

        return false;
    }

    public float GetNextRankScore(CombatRankGrade currentRank)
    {
        float currentEnterScore = 0f;

        CombatRankThreshold currentThreshold;
        if (TryGetRankThreshold(currentRank, out currentThreshold))
            currentEnterScore = currentThreshold.EnterScore;

        float nextScore = maximumScore;

        if (rankThresholds == null)
            return nextScore;

        for (int i = 0; i < rankThresholds.Length; i++)
        {
            CombatRankThreshold threshold = rankThresholds[i];

            if (threshold == null)
                continue;

            if (threshold.EnterScore > currentEnterScore && threshold.EnterScore < nextScore)
                nextScore = threshold.EnterScore;
        }

        return nextScore;
    }

    public float GetRankProgress01(float score, CombatRankGrade currentRank)
    {
        CombatRankThreshold currentThreshold;

        if (!TryGetRankThreshold(currentRank, out currentThreshold))
            return 0f;

        float currentScore = currentThreshold.EnterScore;
        float nextScore = GetNextRankScore(currentRank);

        if (nextScore <= currentScore)
            return 1f;

        return Mathf.Clamp01((score - currentScore) / (nextScore - currentScore));
    }

    private void OnValidate()
    {
        if (minimumScore < 0f)
            minimumScore = 0f;

        if (maximumScore < minimumScore)
            maximumScore = minimumScore;

        startingScore = Mathf.Clamp(startingScore, minimumScore, maximumScore);

        if (rankThresholds != null)
        {
            for (int i = 0; i < rankThresholds.Length; i++)
            {
                if (rankThresholds[i] != null)
                    rankThresholds[i].Validate();
            }
        }

        if (eventTunings != null)
        {
            for (int i = 0; i < eventTunings.Length; i++)
            {
                if (eventTunings[i] != null)
                    eventTunings[i].Validate();
            }
        }

        if (repetitionSettings != null)
            repetitionSettings.Validate();

        if (multiplierSettings != null)
            multiplierSettings.Validate();

        if (decaySettings != null)
            decaySettings.Validate();
    }
}