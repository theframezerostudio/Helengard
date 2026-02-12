using System;
using UnityEngine;

public class GroundSmash_ReactionModule : ReactionModule
{
    [Header("Slam Settings")]
    [SerializeField] private float horizontalForceMultiplier = 1;
    [SerializeField] private float verticalForceMultiplier = 1;
    [SerializeField] private float minDownwardForce = -12f;
    [SerializeField] private AnimationCurve horizontalDecay = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [SerializeField] private float horizontalForce = 1f;  // Pull toward attacker
    [SerializeField] private AnimationCurve gravityCurve; // Controls falling speed profile
    [SerializeField] private float maxSlamTime = 1.0f;

    [SerializeField] private string groundSmashAnim = "Hit_SmashDown";

    private ReactionContext ctx;
    private float timer;
    private float gravityCurveTime;

    private MovementMotionPolicy prevMovePolicy;
    private RotationMotionPolicy prevRotPolicy;

    private Vector3 horizontalVelocity;
    private bool hitGround = false;

    public override ReactionPriority Priority => ReactionPriority.High;

    public override bool CanHandle(DamageEvent ev, ReactionContext ctx)
    {
        if (ctx.Self.Context.isGrounded)
            return false;

        if (!ev.CanChain)
            return false;

        if (ev.SwingType != HitSwing.UpToDown)
            return false;

        if (ev.Effect != HitEffectType.Heavy)
            return false;

        return true;
    }

    public override void Enter(DamageEvent ev, ReactionContext ctx)
    {
        base.Enter(ev, ctx);
        this.ctx = ctx;
        timer = 0f;
        gravityCurveTime = 0f;
        hitGround = false;

        ctx.Animator.CrossFade(groundSmashAnim, 0.05f);

        ctx.Motion.GetMotionPolicy(out prevMovePolicy, out prevRotPolicy);
        ctx.Motion.OverrideMotionPolicy(
            MovementMotionPolicy.NoRootMotion,
            RotationMotionPolicy.YawOnly);

        SetupForces(ev, ctx);
    }

    private void SetupForces(DamageEvent ev, ReactionContext ctx)
    {
        Vector3 force = ev.HitForce;

        //Vector3 toAttacker = ev.Attacker.position - ctx.Transform.position;
        //toAttacker.y = 0;

        //if (toAttacker.sqrMagnitude < 0.01f)
        //    toAttacker = Vector3.zero;
        //else
        //    toAttacker.Normalize();

        //float horizontalMagnitude = Mathf.Max(force.magnitude * horizontalForceMultiplier, horizontalForce);

        //horizontalVelocity = toAttacker * horizontalMagnitude;

        horizontalVelocity = force * horizontalForceMultiplier;
        force.y = 0;

        float verticalVelocity = Mathf.Min(force.y * verticalForceMultiplier, minDownwardForce);
        ctx.Self.verticalVelocity = verticalVelocity;

        ctx.Self.Context.GravityScale = gravityCurve.Evaluate(0);
    }

    public override void Tick(float dt)
    {
        timer += dt;
        gravityCurveTime += dt;

        if (IsFinished)
            return;

        if (timer >= maxSlamTime)
        {
            IsFinished = true;
            return;
        }

        float normalized = timer / maxSlamTime;
        float horizontalFactor = horizontalDecay.Evaluate(normalized);

        ctx.Motion.AddPositionDelta(dt * horizontalFactor * horizontalVelocity);

        float curveValue = gravityCurve.Evaluate(gravityCurveTime);
        ctx.Self.Context.GravityScale = curveValue;

        if (ctx.Self.Context.isGrounded && ctx.Self.verticalVelocity <= 0)
        {
            hitGround = true;
            IsFinished = true;
        }
    }

    public override void Exit(ReactionContext ctx)
    {
        base.Exit(ctx);
        ctx.Motion.OverrideMotionPolicy(prevMovePolicy, prevRotPolicy);

        ctx.Self.Context.GravityScale = 1f;

        if (hitGround)
        {
            // Next reaction
        }
    }
}
