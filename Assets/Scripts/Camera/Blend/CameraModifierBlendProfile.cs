using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraModifierBlendProfile
{
    [field: Min(0f)]
    [field: SerializeField] public float BlendInTime { get; private set; } = 0.15f;

    [field: Min(0f)]
    [field: SerializeField] public float BlendOutTime { get; private set; } = 0.2f;

    [field: SerializeField]
    public AnimationCurve DefaultBlendInCurve { get; private set; } =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [field: SerializeField]
    public AnimationCurve DefaultBlendOutCurve { get; private set; } =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField] private List<CameraChannelBlendOverride> channelOverrides = new();

    public CameraChannelBlendOverride GetOverride(CameraChannel channel)
    {
        for (int i = 0; i < channelOverrides.Count; i++)
        {
            var over = channelOverrides[i];
            if (over != null && (over.Channel & channel) != 0)
                return over;
        }

        return null;
    }

    public float GetBlendInTime(CameraChannel channel)
    {
        var over = GetOverride(channel);
        return BlendInTime * (over != null ? over.BlendInTimeMultiplier : 1f);
    }

    public float GetBlendOutTime(CameraChannel channel)
    {
        var over = GetOverride(channel);
        return BlendOutTime * (over != null ? over.BlendOutTimeMultiplier : 1f);
    }

    public AnimationCurve GetBlendInCurve(CameraChannel channel)
    {
        var over = GetOverride(channel);
        return over != null ? over.BlendInCurve : DefaultBlendInCurve;
    }

    public AnimationCurve GetBlendOutCurve(CameraChannel channel)
    {
        var over = GetOverride(channel);
        return over != null ? over.BlendOutCurve : DefaultBlendOutCurve;
    }
}