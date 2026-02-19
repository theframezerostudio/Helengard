using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Audio_Feedback : Feedback
{
    [SerializeField] private bool resetOnPlay = true;
    [SerializeField] private bool skipIfPlaying = true;

    [SerializeField] private AudioRequest audioRequest;

    private AudioHandle audioHandle = default;

    public override void Initialize()
    {
        
    }

    public override void PauseFeedback()
    {

    }

    public override void Play()
    {
        if (skipIfPlaying && audioHandle.IsPlaying())
        {
            return;
        }

        if (resetOnPlay)
        {
            audioHandle.Stop();
        }

        audioHandle = AudioManager.Instance.Play(audioRequest);
    }

    public override void Resume()
    {
    }

    public override void Stop()
    {

    }
}
