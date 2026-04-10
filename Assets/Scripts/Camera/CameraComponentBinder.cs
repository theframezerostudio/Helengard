using Unity.Cinemachine;
using UnityEngine;

public class CameraComponentBinder : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Optional Cinemachine Components")]
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private CinemachineCameraOffset cameraOffset;
    [SerializeField] private CinemachineRotationComposer rotationComposer;
    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] private CinemachineFollowZoom followZoom;

    public CinemachineCamera Camera => cinemachineCamera;

    private void Reset()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        cameraOffset = GetComponent<CinemachineCameraOffset>();
        rotationComposer = GetComponent<CinemachineRotationComposer>();
        noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        followZoom = GetComponent<CinemachineFollowZoom>();
    }

    public void Apply(in CameraResolvedState state)
    {
        if (cinemachineCamera != null)
        {
            var lens = cinemachineCamera.Lens;
            lens.FieldOfView = state.Fov;
            lens.Dutch = state.Dutch;
            cinemachineCamera.Lens = lens;
        }

        if (orbitalFollow != null)
        {
            orbitalFollow.Radius = state.OrbitRadius;
            orbitalFollow.TargetOffset = state.Offset;
        }

        if (cameraOffset != null)
        {
            cameraOffset.Offset = state.Offset;
            cameraOffset.PreserveComposition = true;
        }

        if (rotationComposer != null)
        {
            rotationComposer.Damping = new Vector2(state.Damping, state.Damping);
        }

        if (noise != null)
        {
            noise.AmplitudeGain = state.NoiseMultiplier;
            noise.FrequencyGain = 1f;
        }

        if (followZoom != null)
        {
            followZoom.Damping = state.Damping;
        }
    }
}