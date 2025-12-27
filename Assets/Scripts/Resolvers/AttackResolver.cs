using System;
using UnityEngine;

public class AttackResolver : MonoBehaviour
{
    public ComboGraph comboGraph;

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

    private bool IsValid(CharacterContext ctx, ComboNode targetNode)
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
