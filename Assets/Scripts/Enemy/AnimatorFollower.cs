using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorFollower : MonoBehaviour
{
    [SerializeField] private int hitLayerIndex = 1;

    private Animator animator;

    private static readonly int HitFrontBackHash = Animator.StringToHash("HitDirection");
    private static readonly int HitHeightHash = Animator.StringToHash("HitHeight");
    private static readonly int HitSwingHash = Animator.StringToHash("HitSwing");

    private Coroutine endRoutine;
    private float timer;
    private float duration;
    private bool active;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        animator.SetLayerWeight(hitLayerIndex, 0f);
    }

    public void EndHitAnim()
    {
        Debug.Log("Ending hit animation");
        if (endRoutine != null)
        {
            StopCoroutine(endRoutine);
            endRoutine = null;
        }

        endRoutine = StartCoroutine(EndHitAnimRoutine());
    }

    private IEnumerator EndHitAnimRoutine()
    {
        while (active)
        {
            ResetHitLayer();
            yield return null;
        }

        endRoutine = null;
    }

    private void ResetHitLayer()
    {
        timer += Time.deltaTime;

        float t = duration > 0f ? Mathf.Clamp01(timer / duration) : 1f;

        float weight = 1f - EaseOutCubic(t);
        animator.SetLayerWeight(hitLayerIndex, weight);

        if (timer >= duration)
        {
            ClearHit();
        }
    }

    public void PlayAnim()
    {

    }

    public async Task<float> ApplyHit(DamageEvent hit)
    {
        active = true;
        timer = 0f;
        duration = hit.StunDuration;

        if (endRoutine != null)
        {
            StopCoroutine(endRoutine);
            endRoutine = null;
        }

        animator.SetLayerWeight(hitLayerIndex, 1f);

        animator.SetFloat(HitFrontBackHash, hit.Direction == HitDirection.Back ? 1f : 0f);
        animator.SetFloat(HitHeightHash, HeightToParam(hit.Height));
        animator.SetFloat(HitSwingHash, SwingToParam(hit.SwingType));
        await Task.Delay(10);

        animator.Play(animator.GetCurrentAnimatorStateInfo(hitLayerIndex).fullPathHash, hitLayerIndex, 0f);

        var clips = animator.GetCurrentAnimatorClipInfo(hitLayerIndex);
        duration = clips.Length > 0 ? clips[0].clip.length : hit.StunDuration;
        return duration;
    }

    public void ClearHit()
    {
        active = false;
        timer = 0f;
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

    private float SwingToParam(HitSwing swing)
    {
        return swing switch
        {
            HitSwing.LeftToRight => 0f,
            HitSwing.RightToLeft => 1f,
            HitSwing.DownToUp => 2f,
            HitSwing.UpToDown => 3f,
            _ => 0f
        };
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
