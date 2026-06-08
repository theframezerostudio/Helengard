using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class HitAnimationController : MonoBehaviour
{
    [SerializeField] private AnimatorController animatorController;
    [SerializeField] private string animatorLayer = "Hit Layer";

    private Animator animator;
    private int hitLayerIndex;

    private static readonly int HitFrontBackHash =
        Animator.StringToHash("HitDirection");

    private static readonly int HitHeightHash =
        Animator.StringToHash("HitHeight");

    private static readonly int HitSwingHash =
        Animator.StringToHash("HitSwing");

    private void Awake()
    {
        animator = animatorController.GetAnimator();
        hitLayerIndex = animator.GetLayerIndex(animatorLayer);

        animator.SetLayerWeight(hitLayerIndex, 0f);
    }

    public void SetIntent(float intent)
    {
        animatorController.SetIntent(animatorLayer, intent);
    }

    /// <summary>
    /// Plays a contextual stagger animation.
    /// Gameplay stun duration is resolved by the reaction module, not here.
    /// </summary>
    public void ApplyHit(
        DamageEvent hit,
        string animationState,
        float transitionTime = 0.05f)
    {
        animatorController.SetIntent(animatorLayer, 1f);

        animator.SetFloat(HitFrontBackHash, DirectionToParameter(hit.Direction));
        animator.SetFloat(HitHeightHash, HeightToParameter(hit.Height));
        animator.SetFloat(HitSwingHash, SwingToParameter(hit.SwingType));

        animatorController.PlayAnim(
            animationState,
            transitionTime,
            hitLayerIndex,
            1f);
    }

    public float PlayAnim(string animationState, float transitionTime = 0.1f)
    {
        animatorController.SetIntent(animatorLayer, 1f);

        return animatorController.PlayAnim(
            animationState,
            transitionTime,
            hitLayerIndex);
    }

    public void EndHitAnim(bool forceEnd)
    {
        animatorController.SetIntent(animatorLayer, 0f);

        if (forceEnd)
            ClearHit();
    }

    public void ClearHit()
    {
        animator.SetLayerWeight(hitLayerIndex, 0f);
    }

    private static float DirectionToParameter(HitDirection direction)
    {
        return direction == HitDirection.Back ? 1f : 0f;
    }

    private static float HeightToParameter(HitHeight height)
    {
        return height switch
        {
            HitHeight.Low => 0f,
            HitHeight.Mid => 0.5f,
            HitHeight.High => 1f,
            _ => 0.5f
        };
    }

    private static float SwingToParameter(SwingType swing)
    {
        return swing switch
        {
            SwingType.LeftToRight => 0f,
            SwingType.RightToLeft => 1f,
            SwingType.DownToUp => 2f,
            SwingType.UpToDown => 3f,
            SwingType.Stab => 4f,
            _ => 0f
        };
    }
}