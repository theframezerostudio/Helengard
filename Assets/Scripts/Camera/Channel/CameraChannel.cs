using System;

[Flags]
public enum CameraChannel
{
    None = 0,
    Offset = 1 << 0,
    Fov = 1 << 1,
    Dutch = 1 << 2,
    OrbitRadius = 1 << 3,
    Damping = 1 << 4,
    Noise = 1 << 5
}