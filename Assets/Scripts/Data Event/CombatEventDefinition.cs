using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Data/Combat Event Definition")]
public sealed class CombatEventDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;

    public string Id => id;
    public string DisplayName => displayName;
}