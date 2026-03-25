using UnityEngine;
using System;

[Serializable]
public class Spell
{
    [SerializeReference, SubclassSelector] public CastingStrategy castingStrategy;
    public CastingProperties castingProperties;

    public void Initialize()
    {
        castingStrategy.Initialize(castingProperties);
    }

    public void Start()
    {
        castingStrategy.Start();
    }
}
