using UnityEngine;
using System;

public class ConjurationProperties : ScriptableObject
{   

    [Header("Spell Properties")]
    [SerializeField] protected string SpellName;     // Name of the casted spell
    [SerializeField] protected float spellDuration; // Duration of spell 

    [Header("Conjuration Strategy")]
    [SerializeReference, SubclassSelector] ConjuringStrategy conjuringStrategy;


    [Header("Spell References")]
    [SerializeField] protected GameObject castVFX;     // Casting VFX
    [SerializeField] protected GameObject spellVFX;   // Spell VFX
    [SerializeField] protected AudioClip castSFX;    // Casting SFX
    [SerializeField] protected AudioClip spellSFX;  // Spell SFX

}
