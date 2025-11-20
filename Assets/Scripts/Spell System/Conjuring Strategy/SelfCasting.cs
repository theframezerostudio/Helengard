using System;
using UnityEngine;


[Serializable]
public class SelfCasting : CastingStrategy
{
  
    public override void Started(Spell spell, CharacterCastingManager castingManager)
    {
        this.spell = spell;
        this.castingManager = castingManager;

        castingManager.SetCurrentStrategy(this);
        Debug.Log("Started");

        GameObject.Instantiate(spell.conjurationProperties.castVFX);
    }

    public override void Performing()
    {
        Debug.Log("Performing");
    }

    public override void Stopped()
    {
        Debug.Log("Released");
        castingManager.ClearCurrentStrategy();
    }
}
