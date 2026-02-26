public class AICombatDecision
{
    private readonly AttackScoreCalculator scoreCalculator;

    public AICombatDecision()
    {
        scoreCalculator = new ();
    }

    public AttackInput Decide(AICombatContext ctx)
    {
        float lightScore = scoreCalculator.Light(ctx);
        float heavyScore = scoreCalculator.Heavy(ctx);
        float lightHoldScore = scoreCalculator.LightHold(ctx);
        float heavyHoldScore = scoreCalculator.HeavyHold(ctx);

        float best = 0;
        AttackInput chosen = AttackInput.None;

        void TrySelect(float score, AttackInput type)
        {
            if (score > best)
            {
                best = score;
                chosen = type;
            }
        }

        TrySelect(lightScore, AttackInput.Light);
        TrySelect(heavyScore, AttackInput.Heavy);
        TrySelect(lightHoldScore, AttackInput.LightHold);
        TrySelect(heavyHoldScore, AttackInput.HeavyHold);

        return chosen;
    }
}