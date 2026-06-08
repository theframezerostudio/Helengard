using System;
using UnityEngine;


[Serializable]
public class SelfCasting : CastingStrategy
{   
    // Instance objects
    private GameObject spellInstance;

    public override void Activate(SpellCastContext context)
    {
        base.Activate(context);

        if (properties.spellVFX != null)
        {
            float duration = spellAnimator.PlayAnim(ExecuteAnimState, 0.2f);
            StartRecovery(duration, 0.4f);

            spellInstance = GameObject.Instantiate(properties.spellVFX);
            GameObject.Destroy(spellInstance, properties.spellDuration);
        }
    }

    public override void Performing(SpellCastContext context)
    {
        base.Performing(context);
    }

    public override void Deactivate()
    {
        base.Deactivate();

    }
}
