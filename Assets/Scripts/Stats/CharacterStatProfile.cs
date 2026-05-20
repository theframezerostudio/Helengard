using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterStatProfile",
    menuName = "Gameplay/Stats/Character Profile"
)]
public sealed class CharacterStatProfile : ScriptableObject
{
    [SerializeField] private List<CharacterStatDefinition> baseStats = new();

    [SerializeField] private List<CharacterDerivedStatDefinition> derivedStats = new();

    public IReadOnlyList<CharacterStatDefinition> BaseStats => baseStats;

    public IReadOnlyList<CharacterDerivedStatDefinition> DerivedStats => derivedStats;
}