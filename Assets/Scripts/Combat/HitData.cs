using UnityEngine;

public readonly struct HitData
{
    public readonly IDamageable target;
    public readonly AttackProfile profile;

    public readonly Transform targetTransform;
    public readonly Vector3 hitPoint;
    public readonly Vector3 hitNormal;
    public readonly Vector3 hitForce;

    public readonly HitDirection direction;
    public readonly HitHeight height;

    public readonly float powerMultiplier;

    public HitData(IDamageable target,
                   AttackProfile profile,
                   Transform targetTransform,
                   Vector3 hitPoint,
                   Vector3 hitNormal,
                   Vector3 hitForce,
                   HitDirection direction,
                   HitHeight height,
                   float powerMultiplier)
    {
        this.target = target;
        this.profile = profile;
        this.targetTransform = targetTransform;
        this.hitPoint = hitPoint;
        this.hitNormal = hitNormal;
        this.hitForce = hitForce;
        this.direction = direction;
        this.height = height;
        this.powerMultiplier = powerMultiplier;
    }
}