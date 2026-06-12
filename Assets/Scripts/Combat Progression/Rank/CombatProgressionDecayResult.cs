public readonly struct CombatProgressionDecayResult
{
    public readonly bool Decayed;
    public readonly float ScoreBefore;
    public readonly float ScoreAfter;
    public readonly float DecayAmount;
    public readonly CombatProgressionRankStabilityResult RankResult;

    public bool ScoreChanged => Decayed && !UnityEngine.Mathf.Approximately(ScoreBefore, ScoreAfter);
    public bool RankChanged => RankResult.Changed;

    public CombatProgressionDecayResult(
        bool decayed,
        float scoreBefore,
        float scoreAfter,
        float decayAmount,
        CombatProgressionRankStabilityResult rankResult)
    {
        Decayed = decayed;
        ScoreBefore = scoreBefore;
        ScoreAfter = scoreAfter;
        DecayAmount = decayAmount;
        RankResult = rankResult;
    }
}