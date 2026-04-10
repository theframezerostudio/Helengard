using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class CameraModeBinding
{
    [field: SerializeField] public CinemachineCamera Camera { get; private set; }
    [field: SerializeField] public CameraModeDefinition Mode { get; private set; }
}