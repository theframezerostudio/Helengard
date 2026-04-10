public class DutchChannel : CameraChannelBase<float>
{
    public override CameraChannel Id => CameraChannel.Dutch;

    protected override float GetBase(CameraModeDefinition mode)
        => mode.BaseDutch;

    protected override float GetModifierValue(CameraModifierDefinition modifier)
        => modifier.DutchDelta;

    protected override float Combine(float baseValue, float delta, float weight)
        => baseValue + delta * weight;

    protected override void Apply(ref CameraResolvedState state, float value)
        => state.Dutch = value;
}