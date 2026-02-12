using UnityEngine;

public enum HitEffectType { Light, Heavy, Grab }
public enum HitDirection { Front, Back, Left, Right }
public enum HitHeight { High, Mid, Low }
public enum HitSwing { LeftToRight, RightToLeft, UpToDown, DownToUp, Stab }

public sealed class DamageEvent
{
    //TODO: Proper Damage Implementation
    public readonly float Damage;
    public readonly HitEffectType Effect;

    [Header("Hit Details")]
    public readonly Vector3 HitPoint;      
    public readonly Vector3 HitNormal;
    public readonly Vector3 HitForce;

    [Header("Event Details")]
    public readonly Transform Attacker;
    public readonly Transform Defender;

    [Header("Hit Classification")]
    public readonly HitDirection Direction;
    public readonly HitHeight Height;       
    public readonly HitSwing SwingType;

    [Header("Additional Properties")]
    public readonly bool CanChain;
    public readonly float StunDuration;
    public readonly float HitStop;

    public DamageEvent(float damage, HitEffectType effect, Vector3 hitPoint, Vector3 hitNormal, Vector3 hitForce,
        Transform attacker, Transform defender, HitDirection direction, HitHeight height, HitSwing swing, bool canChain = false,
        float stunDuration = 0f, float hitStop = 0f)
    {
        Damage = damage;
        Effect = effect;

        HitPoint = hitPoint;
        HitNormal = hitNormal;
        HitForce = hitForce;

        Attacker = attacker;
        Defender = defender;

        Direction = direction;
        Height = height;
        SwingType = swing;

        CanChain = canChain;
        StunDuration = stunDuration;
        HitStop = hitStop;
    }
}
