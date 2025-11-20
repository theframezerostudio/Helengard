using UnityEngine;
using System;

[System.Serializable]
public abstract class CastingStrategy
{
    protected Spell spell;
    protected CharacterCastingManager castingManager;

    public abstract void Started(Spell spell , CharacterCastingManager castingManager);
    public abstract void Performing();
    public abstract void Stopped();
    
}
