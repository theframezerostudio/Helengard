[System.Serializable]
public class AIDecision
{
    public Condition condition;

    public string TrueState = "";
    public string FalseState = "";

    public void Initialize(Character owner, AICombatData combatData)
    {
        condition.Initialize(owner, combatData);
    }

    public string ValidState()
    {
        if (condition.Evaluate())
            return TrueState;

        return FalseState;
    }
}