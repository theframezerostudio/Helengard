using UnityEngine;

public class DemoCameraController : MonoBehaviour
{
    public CameraModeController modeController;
    public CameraModifierRegistry ModifierRegistry;

    public CameraModeDefinition firstPersonMode;
    public CameraModifierDefinition sprintModifier;

    private ActiveCameraModifier sprintCameraModifier;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ModifierRegistry.AddModifier(sprintModifier, this);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ModifierRegistry.RemoveModifier(sprintModifier, this);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            modeController.SetMode(firstPersonMode);
        }
    }
}
