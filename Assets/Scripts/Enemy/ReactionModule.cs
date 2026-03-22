using System;
using UnityEngine;

public abstract class ReactionModule : MonoBehaviour, IReactionModule
{
    [SerializeField] protected ActionData recoveryData;
    public abstract ReactionPriority Priority { get; }
    public bool IsFinished { get; protected set; } = false;
    public virtual bool AllowChaining => false;

    public Action<ActionData> onExit;
    public abstract bool CanHandle(DamageEvent ev, ReactionContext ctx);

    public virtual void Enter(DamageEvent ev, ReactionContext ctx)
    {
        IsFinished = false;

        InitialRotation(ev, ctx);
    }

    private void InitialRotation(DamageEvent ev, ReactionContext ctx)
    {
        Vector3 attackDirection = ev.Attacker.position - transform.position;
        attackDirection.y = 0;
        Quaternion lookDirection;

        // Character will always face at the attacker (y - axis) no matter the attacker Position
        lookDirection = Quaternion.LookRotation(attackDirection);

        // Show face to attacker
        //if (ev.Direction == HitDirection.Front)
        //    lookDirection = Quaternion.LookRotation(attackDirection);
        //else // Show Back to attacker
        //    lookDirection = Quaternion.LookRotation(-attackDirection);

        Quaternion delta = lookDirection * Quaternion.Inverse(transform.rotation);

        //Quaternion deltaRotation = Quaternion.Slerp(Quaternion.identity, delta, #Add alpha for smoother rotation#);

        //transform.rotation = lookDirection;

        ctx.Motion.AddRotation(delta);
    }

    public virtual void Tick(float deltaTime) { }

    public virtual void Exit(ReactionContext ctx)
    {
        IsFinished = true;

        onExit?.Invoke(recoveryData);
    }
}
