public class FovChannel : CameraChannelBase<float>
{
    public override CameraChannel Id => CameraChannel.Fov;

    protected override float GetBase(CameraModeDefinition mode)
        => mode.BaseFov;

    protected override float GetModifierValue(CameraModifierDefinition mod)
        => mod.FovDelta;

    protected override float Combine(float baseValue, float delta, float weight)
        => baseValue + delta * weight;

    protected override void Apply(ref CameraResolvedState state, float value)
        => state.Fov = value;
}