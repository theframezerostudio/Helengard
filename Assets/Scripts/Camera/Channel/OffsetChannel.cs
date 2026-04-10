using UnityEngine;

public class OffsetChannel : CameraChannelBase<Vector3>
{
    public override CameraChannel Id => CameraChannel.Offset;

    protected override Vector3 GetBase(CameraModeDefinition mode)
        => mode.BaseOffset;

    protected override Vector3 GetModifierValue(CameraModifierDefinition mod)
        => mod.OffsetDelta;

    protected override Vector3 Combine(Vector3 baseValue, Vector3 delta, float weight)
        => baseValue + delta * weight;

    protected override void Apply(ref CameraResolvedState state, Vector3 value)
        => state.Offset = value;
}