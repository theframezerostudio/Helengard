using System;
using TMPro.EditorUtilities;
using UnityEngine;

public class Flight_ReactionModule : ReactionModule
{
    [Header("Launch Settings")]
    [SerializeField] private float verticalForceMultiplier = 1f;
    [SerializeField] private float horizontalForceMultiplier = 1f;
    [SerializeField] private float minimumVerticalBoost = 3f;
    [SerializeField] private AnimationCurve horizontalAirCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    //[SerializeField] private float verticalLaunchStrength = 6f;
    //[SerializeField] private float horizontalLaunchStrength = 2f;
    [SerializeField] private AnimationCurve gravityCurve;
    [SerializeField] private float safetyTime = 1.5f;
    [SerializeField] private string launchAnim = "Hit_Launch";

    private ReactionContext ctx;
    private MovementMotionPolicy previousMovementPolicy;
    private RotationMotionPolicy previousRotationPolicy;

    private float timer;
    private float gravityCurveTime;
    private Vector3 horizontalVelocity;

    private AirStateType state;

    public override ReactionPriority Priority => ReactionPriority.Low;

    public override bool CanHandle(DamageEvent ev, ReactionContext ctx)
    {
        if (ev.Effect != HitEffectType.Heavy)
            return false;

        if (ev.SwingType != HitSwing.DownToUp)
            return false;

        if (!ctx.Self.Context.isGrounded)
            return false;

        return true;
    }

    public override void Enter(DamageEvent ev, ReactionContext ctx)
    {
        base.Enter(ev, ctx);
        this.ctx = ctx;

        timer = 0f;
        gravityCurveTime = 0f;
        state = AirStateType.Rising;

        ctx.Animator.CrossFade(launchAnim, 0.1f);

        ctx.Motion.GetMotionPolicy(out previousMovementPolicy, out previousRotationPolicy);
        ctx.Motion.OverrideMotionPolicy(MovementMotionPolicy.NoRootMotion, RotationMotionPolicy.YawOnly);

        SetupForces(ev, ctx);

        ctx.Self.Context.GravityScale = gravityCurve.Evaluate(0f);
    }

    private void SetupForces(DamageEvent ev, ReactionContext ctx)
    {
        Vector3 force = ev.HitForce;
        Vector3 horizontalForce = force;
        horizontalForce.y = 0f;

        horizontalForce.Normalize();

        float horizontalMagnitude = Mathf.Max(force.magnitude * horizontalForceMultiplier, horizontalForce.magnitude);
        horizontalVelocity = horizontalForce * horizontalMagnitude;

        float verticalForce = force.y * verticalForceMultiplier;
        verticalForce = Math.Max(verticalForce, minimumVerticalBoost);

        ctx.Self.verticalVelocity = verticalForce;

        ctx.Self.Context.GravityScale = gravityCurve.Evaluate(0f);
    }

    public override void Tick(float dt)
    {
        timer += dt;
        gravityCurveTime += dt;

        if (IsFinished)
            return;

        if (timer >= safetyTime)
        {
            IsFinished = true;
            return;
        }

        float normalized = timer / safetyTime;
        normalized = Mathf.Clamp01(normalized);
        float horizontalFactor = horizontalAirCurve.Evaluate(normalized);

        ctx.Motion.AddPositionDelta(dt * horizontalFactor * horizontalVelocity);

        float curveValue = gravityCurve.Evaluate(gravityCurveTime);
        ctx.Self.Context.GravityScale = curveValue;

        if (state == AirStateType.Rising && ctx.Self.verticalVelocity <= 0f)
        {
            state = AirStateType.Falling;
        }

        if (ctx.Self.Context.isGrounded && state == AirStateType.Falling)
        {
            state = AirStateType.Landing;
            IsFinished = true;
        }
    }

    public override void Exit(ReactionContext ctx)
    {
        base.Exit(ctx);

        ctx.Self.Context.GravityScale = 1f;
        ctx.Motion.OverrideMotionPolicy(previousMovementPolicy, previousRotationPolicy);
    }
}