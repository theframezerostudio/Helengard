using System;
using UnityEngine;

[Serializable]
public sealed class CombatProgressionDecaySettings
{
    [SerializeField] private bool enabled = true;
    [SerializeField] private float delayBeforeDecay = 3f;
    [SerializeField] private float scoreDecayPerSecond = 8f;
    [SerializeField] private bool pauseWhenOutOfCombat = true;
    [SerializeField] private float minimumScoreAfterDecay;

    public bool Enabled => enabled;
    public float DelayBeforeDecay => delayBeforeDecay;
    public float ScoreDecayPerSecond => scoreDecayPerSecond;
    public bool PauseWhenOutOfCombat => pauseWhenOutOfCombat;
    public float MinimumScoreAfterDecay => minimumScoreAfterDecay;

    public void Validate()
    {
        if (delayBeforeDecay < 0f)
            delayBeforeDecay = 0f;

        if (scoreDecayPerSecond < 0f)
            scoreDecayPerSecond = 0f;

        if (minimumScoreAfterDecay < 0f)
            minimumScoreAfterDecay = 0f;
    }
}