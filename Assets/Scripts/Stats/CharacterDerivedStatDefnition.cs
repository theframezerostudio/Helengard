using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterDerivedStat",
    menuName = "Gameplay/Stats/Character Derived Stat"
)]
public sealed class CharacterDerivedStatDefinition : ScriptableObject
{
    [Header("Stat")]
    [SerializeField] private DerivedStatDefinition stat;

    [Header("Initialization")]
    [SerializeField] private float flatBonus;

    [Header("Limits")]
    [SerializeField] private bool useMinValue;
    [SerializeField] private float minValue;

    [SerializeField] private bool useMaxValue;
    [SerializeField] private float maxValue = 100f;

    [Header("Behavior")]
    [SerializeField] private bool clampValue = true;

    public DerivedStatDefinition Stat => stat;

    public float FlatBonus => flatBonus;

    public bool UseMinValue => useMinValue;
    public float MinValue => minValue;

    public bool UseMaxValue => useMaxValue;
    public float MaxValue => maxValue;

    public bool ClampValue => clampValue;
}