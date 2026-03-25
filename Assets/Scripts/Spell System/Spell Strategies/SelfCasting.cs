using System;
using UnityEngine;


[Serializable]
public class SelfCasting : CastingStrategy
{   
    // Instance objects
    private GameObject spellInstance;

    public override void Start()
    {
        if (properties.spellVFX != null)
        {
            spellInstance = GameObject.Instantiate(properties.spellVFX);
            GameObject.Destroy(spellInstance, properties.spellDuration);
        }
    }

    public override void Performing(CastingData data)
    {
    }

    public override void Stop()
    {
    }
}
