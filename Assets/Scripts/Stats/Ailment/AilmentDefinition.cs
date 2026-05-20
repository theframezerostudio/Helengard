using UnityEngine;

[CreateAssetMenu(
    fileName = "Ailment",
    menuName = "Gameplay/Ailments/Ailment"
)]
public sealed class AilmentDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField]
    private string id;

    [SerializeField]
    private string displayName;

    [Header("Buildup")]
    [SerializeField]
    private float threshold = 100f;

    [SerializeField]
    private float decayPerSecond = 5f;

    [SerializeField]
    private float recoveryDelay = 3f;

    [Header("Trigger")]
    [SerializeField]
    private EffectDefinition triggerEffect;

    [Header("Categories")]
    [SerializeField]
    private EffectCategory category;

    public string Id => id;

    public string DisplayName => displayName;

    public float Threshold => threshold;

    public float DecayPerSecond => decayPerSecond;

    public float RecoveryDelay => recoveryDelay;

    public EffectDefinition TriggerEffect => triggerEffect;

    public EffectCategory Category => category;
}