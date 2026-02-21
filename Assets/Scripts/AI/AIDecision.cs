[System.Serializable]
public class AIDecision
{
    public Condition condition;

    public string TrueState = "";
    public string FalseState = "";

    public string ValidState()
    {
        if (condition.Evaluate())
            return TrueState;

        return FalseState;
    }
}
