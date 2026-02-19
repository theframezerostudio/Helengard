using System.Collections;
using UnityEngine;

[System.Serializable]
public class HitStop_Feedback : Feedback
{
    [SerializeField] private float duration;

    private Coroutine hitStopCoroutine;

    public override void Initialize()
    {
    }

    public override void PauseFeedback()
    {

    }

    public override void Play()
    {
        if (hitStopCoroutine != null)
        {
            Stop();
        }

        hitStopCoroutine = CoroutineManager.Run(FreezeTime());
    }

    public override void Resume()
    {
    }

    public override void Stop()
    {
        if (hitStopCoroutine != null)
        {
            CoroutineManager.Stop(hitStopCoroutine);
        }

        hitStopCoroutine = null;
        Time.timeScale = 1f;
    }

    private IEnumerator FreezeTime()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;

        hitStopCoroutine = null;
    }
}
