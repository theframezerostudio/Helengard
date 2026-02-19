using System.Collections;
using UnityEngine;

[System.Serializable]
public class Pause_Feedback : Feedback
{
    public override IEnumerator Pause { get { return PauseCoroutine(); } }

    [SerializeField] private float duration = 1f;

    private Coroutine pauseCoroutine;

    public override void Initialize()
    {

    }

    public override void PauseFeedback()
    {

    }

    public override void Play()
    {
        pauseCoroutine = CoroutineManager.Run(PauseCoroutine());
    }

    public override void Resume()
    {

    }

    public override void Stop()
    {
        CoroutineManager.Stop(pauseCoroutine);
    }

    //protected virtual IEnumerator PauseWait()
    //{
    //    yield return new WaitForSeconds(duration);
    //}

    private IEnumerator PauseCoroutine()
    {
        yield return new WaitForSeconds(duration);
    }
}
