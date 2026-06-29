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
    private AttackExecutor attackExecutor;

    public override void Activate(SpellCastContext context)
    {
        base.Activate(context);

        attackExecutor = new AttackExecutor(context.CharacterContext.attributes, context.Owner, this);

        targetPosition = context.Aim.Origin;

        if (properties is ProjectileCastingProperties projectileProperties)
        {
            projectileSpeed = projectileProperties.projectileSpeed;
        }

        if (properties.spellVFX != null)
        {
            float duration = spellAnimator.PlayAnim(ExecuteAnimState, 0.2f);
            StartRecovery(duration, 0.4f);

            spellInstance = GameObject.Instantiate(properties.spellVFX, targetPosition, Quaternion.identity);
            spellInstance.transform.forward = context.Aim.AimPoint;

            if (spellInstance.TryGetComponent(out Hitbox hitbox))
            {
                hitbox.Initialize(context.CharacterContext.attributes);
                hitbox.InitiateHit(properties.AttackProfile);

                hitbox.OnHit += HandleHit;
            }

            if (spellInstance.TryGetComponent(out Rigidbody rb))
            {
                rb.linearVelocity = spellInstance.transform.forward * projectileSpeed;
            }

            GameObject.Destroy(spellInstance, properties.spellDuration);
        }
    }

    private void HandleHit(HitData data)
    {
        if (data.target != null)
        {
            if (!attackExecutor.TryResolveHit(data, out DamageEvent damageEvent))
                return;

            if (damageEvent != null)
                data.target.TakeDamage(damageEvent);
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
