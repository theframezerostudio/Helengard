using UnityEngine;

[CreateAssetMenu(fileName = "New Interaction Channel", menuName = "Attributes/Interaction Channel")]
public sealed class InteractionChannelDefinition : ScriptableObject
{
    public string id;
    public string displayName;

    [Header("Resistance")]
    public StatDefinition resistanceStat;
    public float resistanceMultiplier = 1f;

    [Header("Clamping")]
    public bool clampFinalMultiplier = true;
    public float minFinalMultiplier = 0f;
    public float maxFinalMultiplier = 10f;

    public float ResolveResistanceMultiplier(CharacterAttributes target)
    {
        if (target == null || resistanceStat == null)
            return 1f;

        float resistance = target.Stats.GetValue(resistanceStat);
        float multiplier = 1f - resistance * resistanceMultiplier;

        if (clampFinalMultiplier)
            multiplier = Mathf.Clamp(multiplier, minFinalMultiplier, maxFinalMultiplier);

        return multiplier;
    }
}