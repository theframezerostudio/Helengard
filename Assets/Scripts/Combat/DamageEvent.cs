using UnityEngine;

public enum HitImpactType { Light, Heavy, Grab }
public enum HitDirection { Front, Back, Left, Right }
public enum HitHeight { High, Mid, Low }
public enum SwingType { LeftToRight, RightToLeft, UpToDown, DownToUp, Stab }

public sealed class DamageEvent
{
    public InteractionResult Result { get; }

    public HitImpactType Effect { get; }
    public SwingType SwingType { get; }

    public Vector3 HitPoint { get; }
    public Vector3 HitNormal { get; }
    public Vector3 HitForce { get; }

    public Transform Attacker { get; }
    public Transform Target { get; }

    public HitDirection Direction { get; }
    public HitHeight Height { get; }

    public bool CanChain { get; }
    public float StunDuration { get; }
    public float HitStop { get; }
    public float StaggerValue { get; }

    public DamageEvent(InteractionResult result,
                       HitImpactType hitImpact,
                       SwingType swingType,
                       Vector3 hitPoint,
                       Vector3 hitNormal,
                       Vector3 hitForce,
                       Transform attacker,
                       Transform target,
                       HitDirection direction,
                       HitHeight height,
                       bool canChain,
                       float stunDuration,
                       float hitStop,
                       float staggerValue)
    {
        Result = result;
        Effect = hitImpact;
        SwingType = swingType;
        HitPoint = hitPoint;
        HitNormal = hitNormal;
        HitForce = hitForce;
        Attacker = attacker;
        Target = target;
        Direction = direction;
        Height = height;
        CanChain = canChain;
        StunDuration = stunDuration;
        HitStop = hitStop;
        StaggerValue = staggerValue;
    }
}