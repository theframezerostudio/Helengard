using UnityEngine;

public class CameraResolver : MonoBehaviour
{
    [SerializeField] private CameraModifierRegistry modifierRegistry;
    [SerializeField] private CameraChannelRegistry channelRegistry;

    public CameraResolvedState Resolve(CameraModeDefinition mode)
    {
        var state = CameraResolvedState.FromMode(mode);

        if (mode == null)
            return state;

        foreach (var channel in channelRegistry.Channels)
        {
            channel.Resolve(mode, modifierRegistry, ref state);
        }

        return state;
    }
}