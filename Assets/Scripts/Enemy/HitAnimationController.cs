using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Animator))]
public class HitAnimationController : MonoBehaviour
{
    [SerializeField] private AnimatorController animatorController;
    [SerializeField] private string animatorLayer = "Hit Layer";
    private Animator animator;
    private int hitLayerIndex = 1;

    private static readonly int HitFrontBackHash = Animator.StringToHash("HitDirection");
    private static readonly int HitHeightHash = Animator.StringToHash("HitHeight");
    private static readonly int HitSwingHash = Animator.StringToHash("HitSwing");

    private Coroutine endRoutine;
    private float duration;

    private void Awake()
    {
        animator = animatorController.GetAnimator();
        hitLayerIndex = animator.GetLayerIndex(animatorLayer);

        animator.applyRootMotion = false;
        animator.SetLayerWeight(hitLayerIndex, 0f);
    }

    public void SetIntent(float intent)
    {
        animatorController.SetIntent(animatorLayer, intent);
    }

    public async Task<float> ApplyHit(DamageEvent hit, string animState)
    {
        duration = hit.StunDuration;

        if (endRoutine != null)
        {
            StopCoroutine(endRoutine);
            endRoutine = null;
        }

        animatorController.SetIntent(animatorLayer, 1f);

        animator.SetFloat(HitFrontBackHash, hit.Direction == HitDirection.Back ? 1f : 0f);
        animator.SetFloat(HitHeightHash, HeightToParam(hit.Height));
        animator.SetFloat(HitSwingHash, SwingToParam(hit.SwingType));
        await Task.Delay(10);

        animatorController.PlayAnim(animState, 0f, hitLayerIndex, 1f);

        var clips = animator.GetCurrentAnimatorClipInfo(hitLayerIndex);
        duration = clips.Length > 0 ? clips[0].clip.length : hit.StunDuration;
        return duration;
    }

    public float PlayAnim(string anim, float transitionTime = 0.1f)
    {
        animatorController.SetIntent(animatorLayer, 1f);
        return animatorController.PlayAnim(anim, transitionTime, hitLayerIndex);
    }

    public void EndHitAnim(bool forceEnd)
    {
        animatorController.SetIntent(animatorLayer, 0f);

        if (forceEnd)
        {
            ClearHit();
        }
    }

    public void ClearHit()
    {
        duration = 0f;

        animator.SetLayerWeight(hitLayerIndex, 0f);
    }

    private float HeightToParam(HitHeight height)
    {
        return height switch
        {
            HitHeight.Low => 0f,
            HitHeight.Mid => 0.5f,
            HitHeight.High => 1f,
            _ => 0.5f
        };
    }

    private float SwingToParam(SwingType swing)
    {
        return swing switch
        {
            SwingType.LeftToRight => 0f,
            SwingType.RightToLeft => 1f,
            SwingType.DownToUp => 2f,
            SwingType.UpToDown => 3f,
            _ => 0f
        };
    }
}
