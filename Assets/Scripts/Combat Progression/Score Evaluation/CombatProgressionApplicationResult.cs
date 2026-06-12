public readonly struct CombatProgressionApplicationResult
{
    public readonly bool Applied;

    public readonly float ScoreBefore;
    public readonly float ScoreAfter;

    public readonly CombatRankGrade RankBefore;
    public readonly CombatRankGrade RankAfter;

    public readonly float MultiplierBefore;
    public readonly float MultiplierAfter;

    public bool ScoreChanged => !UnityEngine.Mathf.Approximately(ScoreBefore, ScoreAfter);
    public bool RankChanged => RankBefore != RankAfter;
    public bool MultiplierChanged => !UnityEngine.Mathf.Approximately(MultiplierBefore, MultiplierAfter);

    public CombatProgressionApplicationResult(
        bool applied,
        float scoreBefore,
        float scoreAfter,
        CombatRankGrade rankBefore,
        CombatRankGrade rankAfter,
        float multiplierBefore,
        float multiplierAfter)
    {
        Applied = applied;
        ScoreBefore = scoreBefore;
        ScoreAfter = scoreAfter;
        RankBefore = rankBefore;
        RankAfter = rankAfter;
        MultiplierBefore = multiplierBefore;
        MultiplierAfter = multiplierAfter;
    }
}