using System;
using UnityEngine;


[Serializable]
public class SelfCasting : CastingStrategy
{   
    // Instance objects
    private GameObject spellInstance;

    public override void Start()
    {
        base.Start();

        if (properties.spellVFX != null)
        {
            float duration = spellAnimator.PlayAnim(ExecuteAnimState, 0.2f);
            StartRecovery(duration, 0.4f);

            spellInstance = GameObject.Instantiate(properties.spellVFX);
            GameObject.Destroy(spellInstance, properties.spellDuration);
        }
    }

    public override void Performing(CastingData data)
    {
        base.Performing(data);
    }

    public override void Stop()
    {
        base.Stop();

    }
}
