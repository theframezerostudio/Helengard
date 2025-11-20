using UnityEngine;

public class AOECasting : CastingStrategy
{
    public override void Started(Spell spell, CharacterCastingManager castingManager)
    {
        this.castingManager = castingManager;
        this.spell = spell;

        castingManager.SetCurrentStrategy(this);
        
    }

    public override void Performing()
    {
       
    }

    public override void Stopped()
    {
        castingManager.ClearCurrentStrategy();
    }
}
