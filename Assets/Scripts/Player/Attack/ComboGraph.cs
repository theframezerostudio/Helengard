using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboGraph", menuName = "Combo/ComboGraph")]
public class ComboGraph : ScriptableObject
{
    public List<ComboNode> entryNodes = new();

    public ComboNode GetEntryNode(CharacterContext ctx, AttackInput attackInput)
    {
        foreach (var node in entryNodes)
        {
            if (node.input == attackInput)
            {
                if (IsValid(ctx, node))
                    return node;
            }
        }

        return null;
    }

    private bool IsValid(CharacterContext ctx, ComboNode node)
    {
        if (node.requiresGround && !ctx.isGrounded)
            return false;
        if (node.requiresAir && ctx.isGrounded)
            return false;
        if (node.requiresSprint && !ctx.isSprinting)
            return false;
        if (node.requiresDash && !ctx.isDashing)
            return false;
        return true;
    }
}
