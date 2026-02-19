using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioChannel
{
    private readonly List<AudioBox> activeBoxes = new();
    private readonly Queue<AudioBox> pool = new();

    private readonly AudioMixerGroup mixerGroup;

    public AudioChannel(int maxBoxes, AudioMixerGroup mixerGroup, Transform transform)
    {
        this.mixerGroup = mixerGroup;

        for (int i = 0; i < maxBoxes; i++)
        {
            GameObject newBox = new($"Audio Box {mixerGroup.name}_{i + 1}");
            newBox.transform.parent = transform;

            AudioSource newSource = newBox.AddComponent<AudioSource>();
            AudioBox audioBox = new(newSource, i + 1);

            pool.Enqueue(audioBox);
        }
    }

    public AudioBox TryPlay(AudioRequest request)
    {
        Cleanup();

        if (pool.Count == 0)
        {
            if (!TryRelease(request.priority))
                return null;
        }

        AudioBox box = pool.Dequeue();
        activeBoxes.Add(box);
        box.Play(request);

        return box;
    }

    public void Stop(AudioBox box)
    {
        box.Stop();
        activeBoxes.Remove(box);
        pool.Enqueue(box);
    }

    private void Cleanup()
    {
        for (int i = activeBoxes.Count - 1; i >= 0; i--)
        {
            if (!activeBoxes[i].IsPlaying)
            {
                Stop(activeBoxes[i]);
            }
        }
    }


    private bool TryRelease(AudioPriority priority)
    {
        AudioBox lowest = null;

        for (int i = 0; i < activeBoxes.Count; i++)
        {
            if (lowest == null || lowest.Priority > activeBoxes[i].Priority)
            {
                lowest = activeBoxes[i];
            }
        }

        if (lowest == null || lowest.Priority >= priority)
            return false;

        lowest.Stop();
        activeBoxes.Remove(lowest);
        pool.Enqueue(lowest);

        return true;
    }
}
