using UnityEngine;

[System.Serializable]
public class CameraWeightEntry
{
    [field: SerializeField] public CameraModifierDefinition Modifier { get; private set; }

    [field: SerializeField, Range(0f, 1f)]
    public float Weight { get; private set; } = 1f;
}