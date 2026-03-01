public class AttackResolver
{
    private readonly ComboGraph comboGraph;

    public AttackResolver() { }

    public AttackResolver (ComboGraph comboGraph)
    {
        this.comboGraph = comboGraph;
    }

    public ComboNode GetEntryNode(CharacterContext ctx, AttackInput attackInput)
    {
        return comboGraph.GetEntryNode(ctx, attackInput);
    }

    public ComboNode Resolve(CharacterContext ctx, AttackInput attackInput, ComboNode currNode)
    {
        if (currNode == null)
            return comboGraph.GetEntryNode(ctx, attackInput);

        foreach (var transition in currNode.transitions)
        {
            if (transition.attackInput == attackInput)
            {
                if (IsValid(ctx, transition.targetNode))
                    return transition.targetNode;
            }
        }

        return null;
    }

    public bool IsValid(CharacterContext ctx, ComboNode targetNode)
    {
        if (targetNode.requiresGround && !ctx.isGrounded)
            return false;

        if (targetNode.requiresAir && ctx.isGrounded)
            return false;

        if (targetNode.requiresSprint && !ctx.isSprinting)
            return false;

        if (targetNode.requiresDash && !ctx.isDashing)
            return false;

        return true;
    }
}
