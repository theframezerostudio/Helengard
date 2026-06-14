using System;
using UnityEngine;

public abstract class ReactionModule : MonoBehaviour, IReactionModule
{
    [Tooltip("Optional Recover Data For Followup Of Current Reaction")]
    [SerializeField] protected ActionData recoveryData = null;
    [Tooltip("Immediate Override Reaction Module On Cancel")]
    [SerializeField] protected bool forceExit = false;
    [Tooltip("If Module Can Be Reactivated Within Its Duration")]
    [SerializeField] private bool allowChaining = false;

    [field: SerializeField, ReadOnly] public bool IsFinished { get; protected set; } = false;
    [field: SerializeField, ReadOnly] public bool CanBreak { get; protected set; } = false;
    
    public Action<ActionData> onExit;

    public bool AllowChaining => allowChaining;

    public abstract bool CanHandle(DamageEvent ev, ReactionContext ctx);

    public virtual void Enter(DamageEvent ev, ReactionContext ctx)
    {
        IsFinished = false;
        CanBreak = false;

        InitialRotation(ev, ctx);
    }

    private void InitialRotation(DamageEvent ev, ReactionContext ctx)
    {
        if (ev == null || ev.Attacker == null)
            return;

        Vector3 attackDirection =
            ev.Attacker.position - ev.Target.position;

        attackDirection.y = 0f;

        if (attackDirection.sqrMagnitude <= 0.0001f)
            return;

        Quaternion lookDirection =
            Quaternion.LookRotation(attackDirection.normalized, Vector3.up);

        Quaternion deltaRotation =
            lookDirection * Quaternion.Inverse(ev.Target.rotation);

        ctx.Motion.AddRotation(deltaRotation);
    }

    public virtual void Tick(float deltaTime) { }

    public virtual void Exit(ReactionContext ctx)
    {
        IsFinished = true;
        CanBreak = true;

        ctx.Animator.SetIntent(0);

        onExit?.Invoke(recoveryData);
    }

    public virtual void Chain(DamageEvent ev, ReactionContext ctx)
    {
        // Default = restart
        Exit(ctx);
        Enter(ev, ctx);
    }
}
