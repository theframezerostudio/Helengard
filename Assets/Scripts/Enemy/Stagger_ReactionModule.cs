using UnityEngine;

public sealed class Stagger_ReactionModule : ReactionModule
{
    [Header("Resolution")]
    [SerializeField] private StaggerReactionProfile staggerProfile;

    [Header("Fallbacks")]
    [Tooltip("Used only when no StaggerReactionProfile is assigned.")]
    [SerializeField, Min(0f)] private float fallbackDuration = 0.2f;

    [SerializeField] private string fallbackAnimationState = "Hit_Blend";

    [Tooltip("Used only when no StaggerReactionProfile is assigned.")]
    [SerializeField, Range(0f, 1f)] private float fallbackCancelNormalizedTime = 0.7f;

    [Header("Validity")]
    [SerializeField] private bool requiresGrounded = true;

    [Header("Impulse")]
    [Tooltip("Global multiplier owned by this target's stagger module.")]
    [SerializeField, Min(0f)] private float forceMultiplier = 1f;

    [SerializeField, Min(0f)] private float impulseDuration = 0.05f;

    [SerializeField]
    private AnimationCurve impulseFalloff =
        AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private ReactionMotionAdapter motor;

    private float elapsedTime;
    private float totalDuration;
    private float cancelNormalizedTime;

    private float impulseElapsedTime;
    private Vector3 impulseVelocity;

    /// <summary>
    /// ReactionController already selected this module through ReactionKey.
    /// This method validates only the target's current runtime state.
    /// </summary>
    public override bool CanHandle(DamageEvent hit, ReactionContext context)
    {
        if (requiresGrounded && !context.Self.Context.isGrounded)
            return false;

        return true;
    }

    public override void Enter(DamageEvent hit, ReactionContext context)
    {
        base.Enter(hit, context);

        motor = context.Motion;

        elapsedTime = 0f;
        impulseElapsedTime = 0f;

        CanBreak = false;
        IsFinished = false;

        ResolvedStaggerReaction resolved = ResolveReaction(hit);

        totalDuration = resolved.Duration;
        cancelNormalizedTime = resolved.CancelNormalizedTime;

        impulseVelocity =
            hit.HitForce *
            forceMultiplier *
            resolved.ImpulseMultiplier;

 
        context.Self.Suspend(totalDuration);

        context.Animator.ApplyHit(
            hit,
            resolved.AnimationState,
            transitionTime: 0.05f);
    }

    public override void Tick(float deltaTime)
    {
        if (IsFinished)
            return;

        elapsedTime += deltaTime;

        ApplyImpulse(deltaTime);

        float normalizedTime = totalDuration <= 0f
            ? 1f
            : Mathf.Clamp01(elapsedTime / totalDuration);

        if (normalizedTime >= cancelNormalizedTime)
            CanBreak = true;

        if (elapsedTime >= totalDuration)
            IsFinished = true;
    }

    public override void Exit(ReactionContext context)
    {
        context.Animator.EndHitAnim(forceExit);

        motor = null;

        CanBreak = false;
        IsFinished = true;

        base.Exit(context);
    }

    private ResolvedStaggerReaction ResolveReaction(DamageEvent hit)
    {
        if (staggerProfile != null)
            return staggerProfile.Resolve(hit);

        float duration = hit.BaseStunDuration > 0f
            ? hit.BaseStunDuration
            : fallbackDuration;

        return new ResolvedStaggerReaction(
            duration,
            impulseMultiplier: 1f,
            fallbackCancelNormalizedTime,
            fallbackAnimationState);
    }

    private void ApplyImpulse(float deltaTime)
    {
        if (motor == null ||
            impulseDuration <= 0f ||
            impulseElapsedTime >= impulseDuration)
        {
            return;
        }

        float normalizedTime =
            Mathf.Clamp01(impulseElapsedTime / impulseDuration);

        float falloff = impulseFalloff.Evaluate(normalizedTime);

        motor.AddPositionDelta(
            impulseVelocity * falloff * deltaTime);

        impulseElapsedTime += deltaTime;
    }
}