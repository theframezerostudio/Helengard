using UnityEngine;

[CreateAssetMenu(fileName = "JumpProfile", menuName = "Ability Profiles/JumpProfile")]
public class JumpProfile : ScriptableObject
{
    public float jumpForce = 10f;
    public float forwardForce = 5f;
    public float maxFallSpeed = -25f;
    public AnimationCurve gravityCurve;

    [Tooltip("Multiplier for horizontal movement while in the air. 1 means full control, 0 means no control.")]
    public float airSpeedMultiplier = 0.5f;

    public AnimationClip jumpAnim;
    public AnimationClip fallAnim;
    public AnimationClip landAnim;
}
