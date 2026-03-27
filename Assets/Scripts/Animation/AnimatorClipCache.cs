using System.Collections.Generic;
using UnityEngine;

public class AnimatorClipCache
{
    [SerializeField] private Animator animator;

    private readonly Dictionary<string, Dictionary<string, float>> layerClipLengths = new();

    public AnimatorClipCache(Animator animator)
    {
        this.animator = animator;
        BuildCache();
    }

    private void BuildCache()
    {
        layerClipLengths.Clear();

        var controller = animator.runtimeAnimatorController;

        // Get all clips once
        AnimationClip[] allClips = controller.animationClips;

        // Initialize layer dictionaries
        for (int i = 0; i < animator.layerCount; i++)
        {
            string layerName = animator.GetLayerName(i);
            layerClipLengths[layerName] = new Dictionary<string, float>();
        }

        // Runtime limitation:
        // We cannot directly know which clip belongs to which layer,
        // so we populate ALL layers with all clips (safe fallback).
        // Later we rely on querying CURRENT layer state for accuracy.

        foreach (var clip in allClips)
        {
            foreach (var layer in layerClipLengths.Keys)
            {
                if (!layerClipLengths[layer].ContainsKey(clip.name))
                {
                    layerClipLengths[layer].Add(clip.name, clip.length);
                }
            }
        }
    }

    // Get cached clip length (by name + layer)
    public float GetDuration(string layerName, string clipName)
    {
        if (layerClipLengths.TryGetValue(layerName, out var clips))
        {
            if (clips.TryGetValue(clipName, out float length))
            {
                return length;
            }
        }

        Debug.LogWarning($"Clip not found: {clipName} in layer: {layerName}");
        return 0f;
    }

    // Get REAL duration (accounts for Animator speed)
    public float GetCurrentStateDuration(string layerName)
    {
        int layerIndex = animator.GetLayerIndex(layerName);
        if (layerIndex < 0) return 0f;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layerIndex);

        return info.length / info.speed;
    }

    // Get current clip length directly (more accurate than cache)
    public float GetCurrentClipLength(string layerName)
    {
        int layerIndex = animator.GetLayerIndex(layerName);
        if (layerIndex < 0) return 0f;

        var clips = animator.GetCurrentAnimatorClipInfo(layerIndex);

        if (clips.Length > 0)
        {
            return clips[0].clip.length;
        }

        return 0f;
    }
}