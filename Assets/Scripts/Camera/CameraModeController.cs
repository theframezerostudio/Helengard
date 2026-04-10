using UnityEngine;

public class CameraModeController : MonoBehaviour
{
    [SerializeField] private CameraRig rig;

    [SerializeField] private CameraModeDefinition startingMode;

    private CameraModeDefinition currentMode;

    private void Start()
    {
        SetMode(startingMode);
    }

    public void SetMode(CameraModeDefinition newMode)
    {
        if (newMode == null || rig == null)
            return;

        currentMode = newMode;

        foreach (var cam in rig.GetAllCameras())
        {
            cam.Priority = 0;
        }

        var activeCam = rig.GetCamera(newMode);

        if (activeCam != null)
        {
            activeCam.Priority = 100;
        }
    }

    public CameraModeDefinition CurrentMode => currentMode;
}