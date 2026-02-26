public class AttackScoreCalculator
{
    public float Light(AICombatContext ctx)
    {
        return (ctx.PlayerIsAttacking * 0.7f) +
            (ctx.AIUnderPressure * 0.5f) +
            (CalculateIntents(ctx).defensive * 0.6f) +
            ((1 - ctx.DistanceNormalized) * 0.8f) +
            (ctx.AIStaminaNormalized * 0.4f);
    }

    public float Heavy(AICombatContext ctx)
    {
        float riskPenalty = ctx.AIHealthNormalized < 0.3f ? 0.3f : 0;

        return (ctx.PlayerIsOpen * 1.0f) +
            (CalculateIntents(ctx).aggressive * 0.8f) +
            ((1 - ctx.AIUnderPressure) * 0.7f) +
            (ctx.DistanceNormalized * 0.4f) -
            riskPenalty;
    }

    public float LightHold(AICombatContext ctx)
    {
        return (ctx.PlayerIsBlocking * 0.9f) +
            (CalculateIntents(ctx).aggressive * 0.7f) +
            (ctx.PlayerIsOpen * 0.5f) +
            (ctx.AIStaminaNormalized * 0.4f);
    }

    public float HeavyHold(AICombatContext ctx)
    {
        return (ctx.PlayerIsOpen * 1.0f) +
            (CalculateIntents(ctx).aggressive * 0.9f) +
            (ctx.AIStaminaNormalized * 0.7f) -
            (ctx.AIUnderPressure * 0.8f);
    }

    public (float aggressive, float defensive, float opportunistic) CalculateIntents(AICombatContext ctx)
    {
        float aggressive = ctx.AIStaminaNormalized * (1 - ctx.AIUnderPressure);
        float defensive = ctx.AIUnderPressure * (1 - ctx.AIStaminaNormalized);
        float opportunistic = ctx.PlayerIsOpen;

        return (aggressive, defensive, opportunistic);
    }
}
