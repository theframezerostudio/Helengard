using UnityEngine;

public class OrbitRadiusChannel : CameraChannelBase<float>
{
    public override CameraChannel Id => CameraChannel.OrbitRadius;

    protected override float GetBase(CameraModeDefinition mode)
        => mode.BaseOrbitRadius;

    protected override float GetModifierValue(CameraModifierDefinition modifier)
        => modifier.OrbitRadiusDelta;

    protected override float Combine(float baseValue, float delta, float weight)
        => baseValue + delta * weight;

    protected override void Apply(ref CameraResolvedState state, float value)
        => state.OrbitRadius = value;
}