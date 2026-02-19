using UnityEngine;

public class AudioBox
{
    public int ID {  get; private set; }
    private AudioSource source;
    private Transform parentTransform;
    public int GenerationId { get; private set; }
    public AudioPriority Priority { get; private set; } = 0;
    public bool IsPlaying => source.isPlaying;

    public AudioBox(AudioSource source, int iD)
    {
        this.source = source;
        parentTransform = source.transform;
        ID = iD;
    }

    public void Play(AudioRequest request)
    {
        GenerationId++;

        Priority = request.priority;

        source.outputAudioMixerGroup = request.mixerProfile.MixerGroup;
        source.clip = request.audioClip;

        source.volume = request.volume;
        source.pitch = request.pitch;

        source.loop = request.isLooped;
        
        source.spatialBlend = request.spatialBlend;

        if (source.spatialBlend > 0)
        {
            if (request.followTarget)
            {
                source.transform.parent = request.followTarget;
                source.transform.localPosition = request.position;
            }
            else
            {
                source.transform.position = request.position;
            }
        }

        source.Play();
    }

    public void Stop() 
    {
        source.Stop();
    }

    public void Reset()
    {
        source.transform.parent = parentTransform;
    }
}
