[System.Serializable]
public class AIDecision
{
    public Condition condition;

    public string TrueState = "";
    public string FalseState = "";

    public string ValidState(AICombatContext combatContext)
    {
        if (condition.Evaluate(combatContext))
            return TrueState;

        return FalseState;
    }
}