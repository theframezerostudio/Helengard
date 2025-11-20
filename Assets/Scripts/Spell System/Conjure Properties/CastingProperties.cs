using UnityEngine;

public class CastingProperties : ScriptableObject
{   
    [field: Header("Conjuration Properties")]
    [field: SerializeField] public string SpellName { get; private set; } // Name of the casted spell
    [field: SerializeField] public float spellDuration { get; private set; } // Duration of spell 

    [field: Header("Conjuration Strategy")]
    [SerializeReference, SubclassSelector] private CastingStrategy conjuringStrategy;

    [field: Header("Base Properties")]
    [field: SerializeField] public GameObject castVFX { get; private set; }    // Casting VFX
    [field: SerializeField] public GameObject spellVFX { get; private set; }   // Spell VFX
    [field: SerializeField] public AudioClip castSFX { get; private set; }    // Casting SFX
    [field: SerializeField] public AudioClip spellSFX { get; private set; }  // Spell SFX
}