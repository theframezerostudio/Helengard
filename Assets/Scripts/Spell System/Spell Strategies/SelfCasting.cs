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

        if (spell.castingProperties.spellVFX != null)
        {
            spellInstance = GameObject.Instantiate(spell.castingProperties.spellVFX);
            GameObject.Destroy(spellInstance, spell.castingProperties.spellDuration);
        }
    }

    public override void Performing()
    {
    }

    public override void Stopped()
    {
        castingManager.ClearCurrentStrategy();
    }
}
