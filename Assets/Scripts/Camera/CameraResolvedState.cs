using UnityEngine;

public struct CameraResolvedState
{
    public float Fov;
    public Vector3 Offset;
    public float Dutch;
    public float OrbitRadius;
    public float Damping;
    public float NoiseMultiplier;

    public static CameraResolvedState FromMode(CameraModeDefinition mode)
    {
        return new CameraResolvedState
        {
            Fov = mode != null ? mode.BaseFov : 60f,
            Offset = mode != null ? mode.BaseOffset : Vector3.zero,
            Dutch = mode != null ? mode.BaseDutch : 0f,
            OrbitRadius = mode != null ? mode.BaseOrbitRadius : 3f,
            Damping = mode != null ? mode.BaseDamping : 0.5f,
            NoiseMultiplier = 1f
        };
    }
}