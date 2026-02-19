using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class Feedback
{
    [SerializeField] protected string Label;

    public virtual IEnumerator Pause => null;

    public abstract void Initialize();
    public abstract void Play();
    public abstract void PauseFeedback();
    public abstract void Resume();
    public abstract void Stop();
}
