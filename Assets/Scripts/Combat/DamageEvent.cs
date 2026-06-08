using UnityEngine;

public enum HitImpactType { Light, Heavy, Grab }
public enum HitDirection { Front, Back, Left, Right }
public enum HitHeight { High, Mid, Low }
public enum SwingType { LeftToRight, RightToLeft, UpToDown, DownToUp, Stab }

public sealed class DamageEvent
{
    public InteractionResult Result { get; }
    public HitData Hit { get; }

    public AttackProfile Profile => Hit.profile;

    public ReactionKey ExpectedReaction =>
        Profile != null ? Profile.expectedReaction : null;

    public IDamageable TargetDamageable => Hit.target;

    public Transform Attacker => Hit.attackerTransform;
    public Transform Target => Hit.targetTransform;

    public Vector3 HitPoint => Hit.hitPoint;
    public Vector3 HitNormal => Hit.hitNormal;

    /// <summary>
    /// Runtime-resolved force supplied by the attacker/hit resolver.
    /// Reaction modules should use this instead of AttackProfile.hitForce.
    /// </summary>
    public Vector3 HitForce => Hit.hitForce;

    public HitDirection Direction => Hit.direction;
    public HitHeight Height => Hit.height;

    public float PowerMultiplier => Hit.powerMultiplier;

    public HitImpactType Effect =>
        Profile != null ? Profile.hitImpact : HitImpactType.Light;

    public SwingType SwingType =>
        Profile != null ? Profile.swingType : SwingType.Stab;

    /// <summary>
    /// Base gameplay stun authored by the attack.
    /// The stagger profile resolves the final duration from this value.
    /// </summary>
    public float BaseStunDuration =>
        Profile != null ? Profile.stunDuration : 0f;

    /// <summary>
    /// Kept for compatibility with existing gameplay code.
    /// Prefer BaseStunDuration inside new reaction resolution code.
    /// </summary>
    public float StunDuration => BaseStunDuration;

    public float HitStop =>
        Profile != null ? Profile.hitStop : 0f;

    public bool CanChain =>
        Profile != null && Profile.canChain;

    public float StaggerValue =>
        Profile != null ? Profile.staggerValue : 0f;

    public DamageEvent(InteractionResult result, HitData hit)
    {
        Result = result;
        Hit = hit;
    }
}