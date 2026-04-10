using UnityEngine;

public class DampingChannel : CameraChannelBase<float>
{
    public override CameraChannel Id => CameraChannel.Damping;

    protected override float GetBase(CameraModeDefinition mode)
        => mode.BaseDamping;

    protected override float GetModifierValue(CameraModifierDefinition mod)
        => mod.DampingMultiplier;

    protected override float Combine(float baseValue, float multiplier, float weight)
        => baseValue * Mathf.Lerp(1f, multiplier, weight);

    protected override void Apply(ref CameraResolvedState state, float value)
        => state.Damping = value;
}