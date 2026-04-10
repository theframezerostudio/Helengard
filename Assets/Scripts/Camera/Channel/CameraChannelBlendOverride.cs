using UnityEngine;

[System.Serializable]
public class CameraChannelBlendOverride
{
    [field: SerializeField] public CameraChannel Channel { get; private set; }

    [field: Min(0f)]
    [field: SerializeField] public float BlendInTimeMultiplier { get; private set; } = 1f;

    [field: Min(0f)]
    [field: SerializeField] public float BlendOutTimeMultiplier { get; private set; } = 1f;

    [field: SerializeField]
    public AnimationCurve BlendInCurve { get; private set; } =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [field: SerializeField]
    public AnimationCurve BlendOutCurve { get; private set; } =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
}