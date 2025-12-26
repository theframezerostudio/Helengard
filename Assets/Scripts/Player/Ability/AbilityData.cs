using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Ability/AbilityData")]
public class AbilityData : ScriptableObject
{
    public AbilityType Type;
    public float coolDown;

    public bool requiresGround;
    public float stamina;
}
