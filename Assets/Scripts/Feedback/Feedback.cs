using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class Feedback
{
    [SerializeField] protected string Label;

    public virtual IEnumerator PauseWait => null;

    public abstract void Initialize();
    public abstract void Play();
    public abstract void Pause();
    public abstract void Resume();
    public abstract void Stop();
}
