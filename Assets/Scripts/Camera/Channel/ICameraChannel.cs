public interface ICameraChannel
{
    CameraChannel Id { get; }

    void Resolve(CameraModeDefinition mode, CameraModifierRegistry registry, ref CameraResolvedState state);
}