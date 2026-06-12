using System;
using UnityEngine;

[Serializable]
public sealed class CombatRankThreshold
{
    [SerializeField] private CombatRankGrade rank;
    [SerializeField] private string displayName;
    [SerializeField] private float enterScore;
    [SerializeField] private float demoteBelowScore;

    public CombatRankGrade Rank => rank;
    public string DisplayName => displayName;
    public float EnterScore => enterScore;
    public float DemoteBelowScore => demoteBelowScore;

    public void Validate()
    {
        if (enterScore < 0f)
            enterScore = 0f;

        if (demoteBelowScore < 0f)
            demoteBelowScore = 0f;

        if (demoteBelowScore > enterScore)
            demoteBelowScore = enterScore;

        if (string.IsNullOrEmpty(displayName))
            displayName = rank.ToString();
    }
}