using UnityEngine;

[System.Serializable]
public sealed class AilmentResistance
{
    [SerializeField]
    private AilmentDefinition ailment;

    [SerializeField]
    [Range(0f, 1f)]
    private float resistance;

    public AilmentDefinition Ailment => ailment;

    public float Resistance => resistance;
}