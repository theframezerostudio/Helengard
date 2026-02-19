using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioPriority
{
    Critical = 100,
    High = 75,
    Medium = 50,
    Low = 25,
    Ambient = 10
}

[System.Serializable]
public struct AudioRequest
{
    public AudioClip audioClip;
    public AudioMixerProfile mixerProfile;
    public AudioPriority priority;

    public Transform followTarget;
    public Vector3 position;

    [Range(0f, 1f)] public float volume;
    [Range(0f, 3f)] public float pitch;
    [Range(0f, 1f)] public float spatialBlend;

    public bool isLooped;
}

public class AudioManager : Singleton<AudioManager>
{
    private readonly Dictionary<AudioMixerProfile, AudioChannel> channels = new();

    public AudioHandle Play(AudioRequest request)
    {
        if (!channels.TryGetValue(request.mixerProfile, out AudioChannel channel))
        {
            channel = CreateChannel(request.mixerProfile);
            channels[request.mixerProfile] = channel;
        }

        AudioBox box = channel.TryPlay(request);

        if (box == null)
            return default;

        return new AudioHandle(channel, box, box.GenerationId);
    }

    public AudioChannel CreateChannel(AudioMixerProfile mixerProfile)
    {
        GameObject channelContainer = new (mixerProfile.MixerGroup.name);
        channelContainer.transform.parent = transform;
        AudioChannel newChannel = new (mixerProfile.MaxLimit, mixerProfile.MixerGroup, channelContainer.transform);

        return newChannel;
    }
}