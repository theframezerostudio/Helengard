using System;
using UnityEngine;

public abstract class ReactionModule : MonoBehaviour, IReactionModule
{
    [Tooltip("Optional Recover Data For Followup Of Current Reaction")]
    [SerializeField] protected ActionData recoveryData;
    [Tooltip("Normailed Time For Cancelling Reaction")]
    [Range(0f, 1f), SerializeField] protected float cancelTime = 1f;
    [Tooltip("Immediate Override Reaction Module On Cancel")]
    [SerializeField] protected bool forceExit = false;

    [field: SerializeField, ReadOnly] public bool IsFinished { get; protected set; } = false;
    [field: SerializeField, ReadOnly] public bool CanBreak { get; protected set; } = false;
    
    public Action<ActionData> onExit;

    public abstract ReactionPriority Priority { get; }
    public virtual bool AllowChaining => false;

    public abstract bool CanHandle(DamageEvent ev, ReactionContext ctx);

    public virtual void Enter(DamageEvent ev, ReactionContext ctx)
    {
        IsFinished = false;
        CanBreak = false;

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
        CanBreak = true;
        onExit?.Invoke(recoveryData);
    }
}
