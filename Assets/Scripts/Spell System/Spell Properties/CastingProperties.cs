using UnityEngine;

public class CastingProperties : ScriptableObject
{   
    [field: Header("Conjuration Properties")]
    [field: SerializeField] public float spellDuration { get; private set; } // Duration of spell 
    [field: SerializeField] public AttackProfile AttackProfile { get; private set; }

    [field: Header("Blocked Abilities")]
    [field: SerializeField] public AbilityTag[] blockAbilities;

    [field: Header("Base Properties")]
    [field: SerializeField] public GameObject castVFX { get; private set; }    // Casting VFX
    [field: SerializeField] public GameObject spellVFX { get; private set; }   // Spell VFX
    [field: SerializeField] public AudioClip CastSFX { get; private set; }    // Casting SFX
    [field: SerializeField] public AudioClip SpellSFX { get; private set; }  // Spell SFX
}