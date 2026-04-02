using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class CameraShake_Feedback : Feedback
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [Header("Shake Settings")]
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 2f;
    [SerializeField] private float duration = 0.2f;

    private CinemachineBasicMultiChannelPerlin noise;

    private float timer;
    private bool isPlaying;
    private Coroutine shakeCoroutine;

    public override void Initialize()
    {
        if (cinemachineCamera != null)
        {
            noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 0f;
        }
    }

    public override void Play()
    {
        if (noise == null) return;

        if (shakeCoroutine != null)
            Stop();

        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        timer = duration;
        isPlaying = true;

        shakeCoroutine = CoroutineManager.Run(CameraShakeRoutine());
    }

    public override void Pause()
    {
        isPlaying = false;
    }

    public override void Resume()
    {
        if (timer > 0)
            isPlaying = true;

        shakeCoroutine = CoroutineManager.Run(CameraShakeRoutine());
    }

    public override void Stop()
    {
        if (noise == null) return;

        CoroutineManager.Stop(shakeCoroutine);
        shakeCoroutine = null;

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
        isPlaying = false;
        timer = 0f;
    }

    private IEnumerator CameraShakeRoutine()
    {
        while (isPlaying && timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        shakeCoroutine = null;
        Stop();
    }
}