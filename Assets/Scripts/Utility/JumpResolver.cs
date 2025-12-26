using UnityEngine;

public class JumpResolver : MonoBehaviour
{
    public JumpProfile idleJump;
    public JumpProfile sprintJump;

    public JumpProfile Resolve(CharacterContext context)
    {
        if (context.isSprinting)
            return sprintJump;

        return idleJump;
    }
}
