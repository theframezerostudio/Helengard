using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterStat",
    menuName =
        "Gameplay/Stats/Character Stat"
)]
public sealed class CharacterStatDefinition : ScriptableObject
{
    [Header("Stat")]
    [SerializeField] private StatDefinition stat;

    [Header("Initialization")]
    [SerializeField] private float baseValue = 100f;

    [Header("Limits")]
    [SerializeField] private bool useMinValue;

    [SerializeField] private float minValue;

    [SerializeField] private bool useMaxValue;

    [SerializeField] private float maxValue = 100f;

    [Header("Behavior")]
    [SerializeField] private bool clampValue = true;

    public StatDefinition Stat => stat;

    public float BaseValue => baseValue;

    public bool UseMinValue => useMinValue;

    public float MinValue => minValue;

    public bool UseMaxValue => useMaxValue;

    public float MaxValue => maxValue;

    public bool ClampValue => clampValue;
}