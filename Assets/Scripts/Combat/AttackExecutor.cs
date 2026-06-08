using UnityEngine;

public sealed class AttackExecutor
{
    private readonly CharacterAttributes owner;
    private readonly GameObject sourceObject;
    private readonly object causer;

    public AttackExecutor(CharacterAttributes owner, GameObject sourceObject, object causer)
    {
        this.owner = owner;
        this.sourceObject = sourceObject;
        this.causer = causer;
    }

    public bool TryCommitAttack(AttackProfile profile, float powerMultiplier = 1f)
    {
        if (profile == null || owner == null)
            return false;

        if (profile.activationInteraction == null)
            return true;

        InteractionContext context = new InteractionContext
        {
            Source = owner,
            Target = owner,
            SourceObject = sourceObject,
            Causer = causer,
            Position = owner.transform.position,
            Direction = owner.transform.forward,
            PowerMultiplier = powerMultiplier
        };

        InteractionResult result = InteractionRunner.Run(profile.activationInteraction, context);

        return result.Succeeded;
    }

    public bool TryResolveHit(HitData hit, out DamageEvent damageEvent)
    {
        damageEvent = null;

        if (owner == null)
            return false;

        if (hit.target == null || hit.target.Attributes == null)
        {
            Debug.LogWarning(hit.target == null ? "Hit target is null." : "Hit target's attributes are null.");
            return false;
        }

        if (hit.profile == null || hit.profile.impactInteraction == null)
        {
            Debug.LogWarning(hit.profile == null ? "Hit profile is null." : "Hit profile's impact interaction is null.");
            return false;
        }

        InteractionContext context = new InteractionContext
        {
            Source = owner,
            Target = hit.target.Attributes,
            SourceObject = sourceObject,
            Causer = causer,
            Position = hit.hitPoint,
            Direction = owner.transform.forward,
            PowerMultiplier = hit.powerMultiplier
        };

        InteractionResult result = InteractionRunner.Run(hit.profile.impactInteraction, context);

        if (!result.Succeeded)
        {
            Debug.LogWarning("Hit interaction failed. No damage event will be created.");
            return false;
        }

        damageEvent = CreateDamageEvent(hit, result);

        return true;
    }

    private DamageEvent CreateDamageEvent(HitData hit, InteractionResult result)
    {
        return new DamageEvent(result, hit);
    }
}