using System;
using TMPro;
using UnityEngine;

[Serializable]
public class ProjectileCasting : CastingStrategy
{
    // Instance objects
    private GameObject spellInstance;
    private Vector3 targetPosition;

    private float projectileSpeed;
    public override void Activate(SpellCastContext context)
    {
        base.Activate(context);

        targetPosition = context.Aim.Origin;

        if (properties is ProjectileCastingProperties projectileProperties)
        {
            projectileSpeed = projectileProperties.projectileSpeed;
        }

        if (properties.spellVFX != null)
        {
            float duration = spellAnimator.PlayAnim(ExecuteAnimState, 0.2f);
            StartRecovery(duration, 0.4f);

            spellInstance = GameObject.Instantiate(properties.spellVFX, targetPosition,
            Quaternion.identity);

            if (spellInstance.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = context.Aim.AimPoint * projectileSpeed;
            }
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
