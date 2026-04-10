using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Modifier Definition")]
public class CameraModifierDefinition : CameraElementDefinition
{
    [Header("Priority")]
    [field: SerializeField] public int Priority { get; private set; } = 0;

    [Header("Affected Channels")]
    [field: SerializeField] public CameraChannel Channels { get; private set; } = CameraChannel.None;

    [Header("Delta Values")]
    [field: SerializeField] public float FovDelta { get; private set; }
    [field: SerializeField] public Vector3 OffsetDelta { get; private set; }
    [field: SerializeField] public float DutchDelta { get; private set; }
    [field: SerializeField] public float OrbitRadiusDelta { get; private set; }
    [field: SerializeField] public float DampingMultiplier { get; private set; } = 1f;
    [field: SerializeField] public float NoiseMultiplier { get; private set; } = 1f;

    [Header("Blend Rules")]
    [field: SerializeField] public CameraModifierBlendProfile BlendProfile { get; private set; } = new CameraModifierBlendProfile();

    public bool Affects(CameraChannel channel)
    {
        return Channels.HasFlag(channel);
    }
}