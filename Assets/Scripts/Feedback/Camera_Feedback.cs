using System.Collections;
using UnityEngine;

[System.Serializable]
public class Camera_Feedback : Feedback
{
    [SerializeField] private ShotGraphManager shotGraphManager;
    [SerializeField] private string shotId;
    [SerializeField] private float duration = 1f;
    [SerializeField] private bool revertOnStop = true;

    private Coroutine camCoroutine;

    public override void Initialize()
    {
    }

    public override void Play()
    {
        if (camCoroutine != null)
            CoroutineManager.Stop(camCoroutine);

        shotGraphManager.SetShot(shotId);
        camCoroutine = CoroutineManager.Run(StopRoutine());
    }

    public override void Pause()
    {
    }

    public override void Resume()
    {
    }

    public override void Stop()
    {
        if (revertOnStop)
            shotGraphManager.RevertShot();
    }

    private IEnumerator StopRoutine()
    {
        yield return new WaitForSeconds(duration);
        Stop();

        camCoroutine = null;
    }
}
