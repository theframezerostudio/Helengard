using UnityEngine;
using System.Collections;

public class ShotSequencePlayer : MonoBehaviour
{
    public ShotGraphManager manager;

    [System.Serializable]
    public class SequenceShot
    {
        public string shotId;
        public float duration = 1.5f;
    }

    public SequenceShot[] sequence;
    public bool playOnStart = false;

    void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        foreach (var step in sequence)
        {
            manager.SetShot(step.shotId);
            yield return new WaitForSeconds(step.duration);
        }
    }
}