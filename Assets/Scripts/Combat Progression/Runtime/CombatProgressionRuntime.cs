using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class CombatProgressionRuntime
{
    [SerializeField] private float currentScore;
    [SerializeField] private CombatRankGrade currentRank;
    [SerializeField] private CombatRankGrade previousRank;
    [SerializeField] private float currentMultiplier = 1f;
    [SerializeField] private bool isCombatActive;

    [SerializeField] private float lastMeaningfulActionTime;
    [SerializeField] private float lastScoreChangeTime;
    [SerializeField] private float lastRankChangeTime;

    [SerializeField] private int maxRecentResults = 16;

    private readonly List<CombatProgressionScoreResult> recentResults = new List<CombatProgressionScoreResult>();

    public float CurrentScore => currentScore;
    public CombatRankGrade CurrentRank => currentRank;
    public CombatRankGrade PreviousRank => previousRank;
    public float CurrentMultiplier => currentMultiplier;
    public bool IsCombatActive => isCombatActive;
    public float LastMeaningfulActionTime => lastMeaningfulActionTime;
    public float LastScoreChangeTime => lastScoreChangeTime;
    public float LastRankChangeTime => lastRankChangeTime;
    public int RecentResultCount => recentResults.Count;

    public void Initialize(CombatProgressionProfile profile)
    {
        if (profile == null)
        {
            currentScore = 0f;
            currentRank = CombatRankGrade.D;
            previousRank = CombatRankGrade.D;
            currentMultiplier = 1f;
            isCombatActive = false;
            lastMeaningfulActionTime = Time.time;
            lastScoreChangeTime = Time.time;
            lastRankChangeTime = Time.time;
            recentResults.Clear();
            return;
        }

        currentScore = profile.ClampScore(profile.StartingScore);
        currentRank = profile.GetRankForScore(currentScore);
        previousRank = currentRank;
        currentMultiplier = profile.MultiplierSettings != null ? profile.MultiplierSettings.StartingMultiplier : 1f;
        isCombatActive = false;
        lastMeaningfulActionTime = Time.time;
        lastScoreChangeTime = Time.time;
        lastRankChangeTime = Time.time;
        recentResults.Clear();
    }

    public void Reset(CombatProgressionProfile profile)
    {
        Initialize(profile);
    }

    public void SetCombatActive(bool active)
    {
        isCombatActive = active;
    }

    public void MarkMeaningfulAction(float time)
    {
        lastMeaningfulActionTime = time;
    }

    public void SetScore(float score, CombatProgressionProfile profile)
    {
        if (profile != null)
            currentScore = profile.ClampScore(score);
        else
            currentScore = Mathf.Max(0f, score);

        lastScoreChangeTime = Time.time;
    }

    public void AddScore(float amount, CombatProgressionProfile profile)
    {
        SetScore(currentScore + amount, profile);
    }

    public void SetMultiplier(float multiplier, CombatProgressionProfile profile)
    {
        float min = 1f;
        float max = 999f;

        if (profile != null && profile.MultiplierSettings != null)
        {
            min = profile.MultiplierSettings.StartingMultiplier;
            max = profile.MultiplierSettings.MaximumMultiplier;
        }

        currentMultiplier = Mathf.Clamp(multiplier, min, max);
    }

    public void ResetMultiplier(CombatProgressionProfile profile)
    {
        if (profile != null && profile.MultiplierSettings != null)
            currentMultiplier = profile.MultiplierSettings.StartingMultiplier;
        else
            currentMultiplier = 1f;
    }

    public void SetRank(CombatRankGrade newRank)
    {
        if (currentRank == newRank)
            return;

        previousRank = currentRank;
        currentRank = newRank;
        lastRankChangeTime = Time.time;
    }

    public void RecordResult(CombatProgressionScoreResult result)
    {
        recentResults.Add(result);

        while (recentResults.Count > maxRecentResults)
            recentResults.RemoveAt(0);
    }

    public bool TryGetRecentResult(int indexFromLatest, out CombatProgressionScoreResult result)
    {
        result = default;

        if (indexFromLatest < 0)
            return false;

        int index = recentResults.Count - 1 - indexFromLatest;

        if (index < 0 || index >= recentResults.Count)
            return false;

        result = recentResults[index];
        return true;
    }

    public bool TryGetLatestResult(out CombatProgressionScoreResult result)
    {
        return TryGetRecentResult(0, out result);
    }

    public void ClearResults()
    {
        recentResults.Clear();
    }

    public float GetTimeSinceMeaningfulAction()
    {
        return Time.time - lastMeaningfulActionTime;
    }

    public float GetTimeSinceScoreChanged()
    {
        return Time.time - lastScoreChangeTime;
    }

    public float GetTimeSinceRankChanged()
    {
        return Time.time - lastRankChangeTime;
    }

    public float GetRankProgress01(CombatProgressionProfile profile)
    {
        if (profile == null)
            return 0f;

        return profile.GetRankProgress01(currentScore, currentRank);
    }

    public CombatProgressionStateSnapshot CreateSnapshot(CombatProgressionProfile profile)
    {
        float progress = GetRankProgress01(profile);

        return new CombatProgressionStateSnapshot(
            currentScore,
            currentRank,
            previousRank,
            currentMultiplier,
            progress,
            isCombatActive,
            lastMeaningfulActionTime,
            lastScoreChangeTime,
            lastRankChangeTime);
    }
}