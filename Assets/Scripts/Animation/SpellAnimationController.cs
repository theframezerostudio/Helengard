using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using static System.TimeZoneInfo;

public class SpellAnimationController : MonoBehaviour
{
    [SerializeField] private AnimatorController animatorController;
    [SerializeField] private string layer = "Magic Layer";

    private Coroutine intentRoutine;
    private Animator animator;
    private int layerIndex = -1;

    private void Awake()
    {
        animator = animatorController.GetAnimator();
        layerIndex = animator.GetLayerIndex(layer);
    }

    public void SetIntent(float value, float duration = 0f)
    {
        if (intentRoutine != null)
        {
            StopCoroutine(intentRoutine);
            intentRoutine = null;
        }

        if (duration == 0f)
        {
            animatorController.SetIntent(layer, value);
            return;
        }

        intentRoutine = StartCoroutine(SetIntentRoutine(value, duration));
    }

    public void StopIntentRoutine()
    {
        if (intentRoutine != null)
        {
            StopCoroutine(intentRoutine);
            intentRoutine = null;
        }
    }

    private IEnumerator SetIntentRoutine(float value, float duration)
    {
        yield return new WaitForSeconds(duration);

        animatorController.SetIntent(layer, value);

        intentRoutine = null;
    }

    public float PlayAnim(string anim, float transitionTime = 0.1f)
    {
        animatorController.SetIntent(layer, 1f);
        return animatorController.PlayAnim(anim, transitionTime, layerIndex);
    }
}
