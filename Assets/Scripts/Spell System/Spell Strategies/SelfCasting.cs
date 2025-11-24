using System;
using UnityEngine;


[Serializable]
public class SelfCasting : CastingStrategy
{   
    // Instance objects
    private GameObject spellInstance;
    public override void Started(Spell spell, CharacterCastingManager castingManager)
    {
        this.spell = spell;
        this.castingManager = castingManager;

        castingManager.SetCurrentStrategy(this);
        Debug.Log("Started");

        if (spell.castingProperties.spellVFX != null)
        {
            spellInstance = GameObject.Instantiate(spell.castingProperties.spellVFX);
            GameObject.Destroy(spellInstance, spell.castingProperties.spellDuration);
        }
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
