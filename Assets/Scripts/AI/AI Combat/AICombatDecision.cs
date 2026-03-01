using UnityEngine;
[System.Serializable]
public class AICombatDecision
{
    private readonly AttackScoreCalculator scoreCalculator;
    private CharacterContext characterContext;

    private readonly float chainBias = 1.05f;

    [SerializeField, ReadOnly] private float lightScore;
    [SerializeField, ReadOnly] private float heavyScore;
    [SerializeField, ReadOnly] private float lightHoldScore;
    [SerializeField, ReadOnly] private float heavyHoldScore;

    public AICombatDecision(AttackPersona persona)
    {
        scoreCalculator = new AttackScoreCalculator(persona);
    }

    public void Initialize(CharacterContext ctx)
    {
        characterContext = ctx;
    }

    public AttackInput DecideChain(AICombatData data, AICombatMemory mem, ComboNode currentNode, float animNormalizedTime)
    {
        if (currentNode == null)
            return AttackInput.None;

        if (!currentNode.comboWindow.IsValid(animNormalizedTime))
            return AttackInput.None;

        AttackInput bestChain = AttackInput.None;
        float bestScore = float.MinValue;

        foreach (var t in currentNode.transitions)
        {
            if (t == null || t.targetNode == null)
                continue;

            if (!MeetsRequirements(t.targetNode))
                continue;

            float baseScore = ScoreInput(data, mem, t.attackInput);
            float chainScore = baseScore * chainBias;

            if (chainScore > bestScore)
            {
                bestScore = chainScore;
                bestChain = t.attackInput;
            }
        }

        return bestChain;
    }

    public AttackInput DecideEntry(AICombatData data, AICombatMemory mem, ComboGraph graph)
    {
        if (graph == null || graph.entryNodes.Count == 0)
            return AttackInput.None;

        float bestScore = float.MinValue;
        AttackInput best = AttackInput.None;

        foreach (var n in graph.entryNodes)
        {
            float score = ScoreInput(data, mem, n.input);
            if (score > bestScore)
            {
                bestScore = score;
                best = n.input;
            }
        }

        return best;
    }

    private float ScoreInput(AICombatData data, AICombatMemory mem, AttackInput input)
    {
        //TODO:  Cache scores for debugging/inspection, remove redundant calls to scoreCalculator
        switch (input)
        {
            case AttackInput.Light:
                lightScore = scoreCalculator.Light(data, mem);
                break;
            case AttackInput.Heavy:
                heavyScore = scoreCalculator.Heavy(data, mem);
                break;
            case AttackInput.LightHold:
                lightHoldScore = scoreCalculator.LightHold(data, mem);
                break;
            case AttackInput.HeavyHold:
                heavyHoldScore = scoreCalculator.HeavyHold(data, mem);
                break;
        }

        return input switch
        {
            AttackInput.Light => scoreCalculator.Light(data, mem),
            AttackInput.Heavy => scoreCalculator.Heavy(data, mem),
            AttackInput.LightHold => scoreCalculator.LightHold(data, mem),
            AttackInput.HeavyHold => scoreCalculator.HeavyHold(data, mem),
            _ => 0f
        };
    }

    private bool MeetsRequirements(ComboNode node)
    {
        if (node == null) return false;

        if (node.requiresGround && !characterContext.isGrounded) return false;
        if (node.requiresAir && characterContext.isGrounded) return false;
        if (node.requiresSprint && !characterContext.isSprinting) return false;

        return true;
    }
}