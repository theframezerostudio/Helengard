using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    Jump,
    Dash,
}

public class AbilitySystem
{
    public CharacterContext Owner;
    private readonly Dictionary<AbilityType, AbilityInstance> abilities = new();

    public AbilitySystem(CharacterContext owner, AbilityData[] startingAbilities)
    {
        Owner = owner;
        if (startingAbilities == null) return;

        foreach (var ability in startingAbilities)
        {
            AddAbility(ability);
        }
    }

    public void AddAbility(AbilityData data)
    {
        if (!abilities.ContainsKey(data.Type))
        {
            abilities[data.Type] = new AbilityInstance(data);
        }
    }

    public bool TryUse(AbilityType type)
    {
        if (!abilities.TryGetValue(type, out var ability))
            return false;

        if (!CanUse(type))
            return false;

        ability.lastUsedTime = Time.time;

        return true;
    }

    public bool CanUse(AbilityType type)
    {
        AbilityInstance ability = abilities[type];
        if (Time.time < ability.lastUsedTime + ability.data.coolDown)
            return false;

        if (ability.data.requiresGround && !Owner.isGrounded)
            return false;

        return true;
    }

    public void UseAbility(AbilityType type)
    {
        abilities[type].lastUsedTime = Time.time;
    }
}