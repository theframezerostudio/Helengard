using UnityEngine;

[CreateAssetMenu(fileName = "JumpProfile", menuName = "Ability Profiles/JumpProfile")]
public class JumpProfile : ScriptableObject
{
    public float jumpForce = 10f;
    public float forwardForce = 5f;
    public float maxFallSpeed = -25f;
    public AnimationCurve gravityCurve;

    [Range(0f, 1f)]
    public float airControlMultiplier = 0.5f;

    public AnimationClip jumpAnim;
    public AnimationClip fallAnim;
    public AnimationClip landAnim;
}
