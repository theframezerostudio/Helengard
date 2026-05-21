using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterStatProfile",
    menuName = "Gameplay/Stats/Character Profile"
)]
public sealed class CharacterStatProfile : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] private List<CharacterStatDefinition> baseStats = new();
    [SerializeField] private List<CharacterDerivedStatDefinition> derivedStats = new();

    [Header("Resources")]
    [SerializeField] private List<ResourceDefinition> resources = new();

    [Header("Ailments")]
    [SerializeField] private List<AilmentResistance> ailmentResistances = new();

    public IReadOnlyList<CharacterStatDefinition> BaseStats => baseStats;
    public IReadOnlyList<CharacterDerivedStatDefinition> DerivedStats => derivedStats;
    public IReadOnlyList<ResourceDefinition> Resources => resources;
    public IReadOnlyList<AilmentResistance> AilmentResistances => ailmentResistances;
}