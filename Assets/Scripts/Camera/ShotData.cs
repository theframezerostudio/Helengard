using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class ShotData
{
    public string id;
    public CinemachineCamera camera;

    [Header("Blend Settings")]
    public CinemachineBlendDefinition.Styles blendStyle = CinemachineBlendDefinition.Styles.EaseInOut;
    public float blendTime = 0.5f;

    private ShotModifier[] modifiers;

    public void Initialize()
    {

    }

    public void Activate()
    {
        if (camera) camera.Priority = 100;

        if (modifiers == null) return;

        foreach (ShotModifier modifier in modifiers)
        {
            if (modifier == null) continue;
            modifier.Enable();
        }
    }

    public void Deactivate()
    {
        if (camera) camera.Priority = 1;

        if (modifiers == null) return;

        foreach (ShotModifier modifier in modifiers)
        {
            if (modifier == null) continue;
            modifier.Disable();
        }
    }
}