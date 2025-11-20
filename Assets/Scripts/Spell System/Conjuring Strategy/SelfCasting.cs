using System;

[Serializable]
public class SelfCasting : CastingStrategy
{
    public Spell conjuration;

    public override void Started(Spell spell, CharacterCastingManager castingManager)
    {
        this.spell = spell;
        this.castingManager = castingManager;

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
