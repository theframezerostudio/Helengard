using UnityEngine;

public sealed class SpellAimResolver
{
    public SpellAimData Resolve(
        Transform caster,
        CharacterContext characterContext,
        PlayerTargeting targeting,
        SpellAimSettings settings,
        CastingData data)
    {
        Vector3 origin = caster.position + caster.TransformDirection(settings.castOffset);

        Target target = ResolveTarget(targeting, settings);

        if (target != null && IsTargetInRange(origin, target, settings.targetRange))
        {
            Vector3 aimPoint = GetTargetAimPoint(target);
            Vector3 direction = aimPoint - origin;

            if (direction.sqrMagnitude <= 0.0001f)
                direction = caster.forward;

            direction.Normalize();

            return new SpellAimData(
                target,
                origin,
                aimPoint,
                direction,
                false,
                true
            );
        }

        if (settings.requireTarget && !settings.allowManualFallback)
        {
            return new SpellAimData(
                null,
                origin,
                origin + caster.forward * settings.manualRange,
                caster.forward,
                true,
                false
            );
        }

        Vector3 manualDirection = ResolveManualDirection(caster, data);
        Vector3 manualAimPoint = origin + manualDirection * settings.manualRange;

        return new SpellAimData(
            null,
            origin,
            manualAimPoint,
            manualDirection,
            true,
            true
        );
    }

    private Target ResolveTarget(PlayerTargeting targeting, SpellAimSettings settings)
    {
        if (targeting == null)
            return null;

        switch (settings.aimMode)
        {
            case SpellAimMode.Manual:
                return null;

            case SpellAimMode.SoftTarget:
                return targeting.SoftTarget;

            case SpellAimMode.LockedTarget:
                return targeting.LockedTarget;

            case SpellAimMode.ActiveTarget:
                return targeting.ActiveTarget;

            case SpellAimMode.AutoTarget:
                return targeting.ActiveTarget;
        }

        return null;
    }

    private bool IsTargetInRange(Vector3 origin, Target target, float range)
    {
        if (target == null)
            return false;

        Vector3 toTarget = target.transform.position - origin;
        toTarget.y = 0f;

        return toTarget.sqrMagnitude <= range * range;
    }

    private Vector3 GetTargetAimPoint(Target target)
    {
        return target.transform.position + Vector3.up * 1.2f;
    }

    private Vector3 ResolveManualDirection(Transform caster, CastingData data)
    {
        // Replace this later with camera aim.
        Vector3 direction = caster.forward;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.0001f)
            direction = caster.forward;

        direction.Normalize();

        return direction;
    }
}