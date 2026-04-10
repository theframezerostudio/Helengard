using System.Collections.Generic;
using UnityEngine;

public class CameraChannelRegistry : MonoBehaviour
{
    private List<ICameraChannel> channels;

    private void Awake()
    {
        channels = new List<ICameraChannel>
        {
            new FovChannel(),
            new OffsetChannel(),
            new DampingChannel(),
            new DutchChannel(),
            new OrbitRadiusChannel(),
            // Add new channels here ONLY
        };
    }

    public IReadOnlyList<ICameraChannel> Channels => channels;
}