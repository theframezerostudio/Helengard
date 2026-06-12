using System.Collections.Generic;

public sealed class CombatProgressionRuleEvaluation
{
    private readonly List<CombatProgressionRuleResult> results = new List<CombatProgressionRuleResult>();

    public CombatProgressionSignal Signal { get; }
    public int ResultCount => results.Count;

    public CombatProgressionRuleEvaluation(CombatProgressionSignal signal)
    {
        Signal = signal;
    }

    public void AddResult(CombatProgressionRuleResult result)
    {
        if (!result.IsValid)
            return;

        results.Add(result);
    }

    public bool TryGetResult(int index, out CombatProgressionRuleResult result)
    {
        result = default;

        if (index < 0 || index >= results.Count)
            return false;

        result = results[index];
        return true;
    }

    public bool HasMeaningfulAction()
    {
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].IsMeaningfulAction)
                return true;
        }

        return false;
    }

    public bool ShouldBlockBaseScore()
    {
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].BlockBaseScore)
                return true;
        }

        return false;
    }

    public bool HasMultiplierReset()
    {
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].ResetMultiplier)
                return true;
        }

        return false;
    }
}