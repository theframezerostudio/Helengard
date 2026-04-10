using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Mode Definition")]
public class CameraModeDefinition : CameraElementDefinition
{
    [Header("Base Camera Values")]
    [field: SerializeField] public float BaseFov { get; private set; } = 60f;
    [field: SerializeField] public Vector3 BaseOffset { get; private set; }
    [field: SerializeField] public float BaseDutch { get; private set; }
    [field: SerializeField] public float BaseOrbitRadius { get; private set; } = 3f;
    [field: SerializeField] public float BaseDamping { get; private set; } = 0.5f;

    [Header("Modifier Weights")]
    [SerializeField] private List<CameraWeightEntry> modifierWeights = new();

    public IReadOnlyList<CameraWeightEntry> ModifierWeights => modifierWeights;

    public float GetWeightForModifier(CameraModifierDefinition modifier)
    {
        if (modifier == null)
            return 0f;

        foreach (var entry in modifierWeights)
        {
            if (entry != null && entry.Modifier == modifier)
                return entry.Weight;
        }

        return 1f;
    }
}