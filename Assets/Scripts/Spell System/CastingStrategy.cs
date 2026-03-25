using UnityEngine;
using System;

[System.Serializable]
public  class CastingStrategy
{
    protected Spell spell;
    protected CharacterCastingManager castingManager;

    public virtual void Started(Spell spell , CharacterCastingManager castingManager)
    {

    }

    public virtual void Performing()
    {

    }

    public virtual void Stopped()
    {

    }
    
}
