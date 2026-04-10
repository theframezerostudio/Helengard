using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField] private List<CameraModeBinding> bindings = new();

    private Dictionary<CameraModeDefinition, CinemachineCamera> map;

    private void Awake()
    {
        map = new Dictionary<CameraModeDefinition, CinemachineCamera>();

        foreach (var binding in bindings)
        {
            if (binding.Mode == null || binding.Camera == null)
                continue;

            map[binding.Mode] = binding.Camera;
        }
    }

    public CinemachineCamera GetCamera(CameraModeDefinition mode)
    {
        if (mode == null) return null;

        map.TryGetValue(mode, out var cam);
        return cam;
    }

    public IEnumerable<CinemachineCamera> GetAllCameras()
    {
        return map.Values;
    }
}