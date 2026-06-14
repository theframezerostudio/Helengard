using UnityEngine;

public sealed class Flight_ReactionModule : ReactionModule
{
    [Header("Launch Settings")]
    [SerializeField, Min(0f)] private float verticalForceMultiplier = 1f;
    [SerializeField, Min(0f)] private float horizontalForceMultiplier = 1f;
    [SerializeField, Min(0f)] private float minimumVerticalBoost = 3f;

    [SerializeField]
    private AnimationCurve horizontalAirCurve =
        AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Gravity Modifiers")]
    [SerializeField]
    private AnimationCurve riseCurve =
        AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [SerializeField, Min(1f)] private float riseMultiplier = 1f;

    [SerializeField]
    private AnimationCurve fallCurve = 
        AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [SerializeField, Min(1f)] private float fallMultiplier = 1f;

    [SerializeField, Min(0.1f)] private float safetyTime = 1.5f;

    [Header("Animation")]
    [SerializeField] private string launchAnim = "Hit_Launch";

    private ReactionContext context;

    private MovementMotionPolicy previousMovementPolicy;
    private RotationMotionPolicy previousRotationPolicy;

    private float timer;
    private float riseCurveTime;
    private float fallCurveTime;

    private Vector3 horizontalVelocity;
    private AirStateType state;

    public override bool CanHandle(DamageEvent hit, ReactionContext context)
    { 
        return context.Self.Context.isGrounded;
    }

    public override void Enter(DamageEvent hit, ReactionContext context)
    {
        base.Enter(hit, context);

        this.context = context;

        timer = 0f;
        riseCurveTime = 0f;

        CanBreak = false;
        IsFinished = false;

        state = AirStateType.Rising;

        context.Animator.PlayAnim(launchAnim, 0.1f);

        context.Motion.GetMotionPolicy(
            out previousMovementPolicy,
            out previousRotationPolicy);

        context.Motion.OverrideMotionPolicy(
            MovementMotionPolicy.NoRootMotion,
            RotationMotionPolicy.YawOnly);

        SetupForces(hit, context);

        context.Self.Context.GravityScale = riseCurve.Evaluate(0f);
    }

    public override void Tick(float deltaTime)
    {
        if (IsFinished)
            return;

        timer += deltaTime;

        if (state == AirStateType.Falling)
            fallCurveTime += deltaTime;
        else if (state == AirStateType.Rising)
            riseCurveTime += deltaTime;

        if (timer >= safetyTime)
        {
            IsFinished = true;
            return;
        }

        float normalizedTime = Mathf.Clamp01(timer / safetyTime);
        float horizontalFactor = horizontalAirCurve.Evaluate(normalizedTime);

        context.Motion.AddPositionDelta(
            deltaTime * horizontalFactor * horizontalVelocity);

        AnimationCurve gravityCurve = state == AirStateType.Rising ? riseCurve : fallCurve;
        float curveTime = state == AirStateType.Rising ? riseCurveTime : fallCurveTime;
        float multiplier = state == AirStateType.Rising ? riseMultiplier : fallMultiplier;

        context.Self.Context.GravityScale =
            gravityCurve.Evaluate(curveTime) * multiplier;

        if (state == AirStateType.Rising &&
            context.Self.verticalVelocity <= 0f)
        {
            state = AirStateType.Falling;
        }

        if (state == AirStateType.Falling &&
            context.Self.Context.isGrounded)
        {
            state = AirStateType.Landing;
            IsFinished = true;
        }
    }

    public override void Exit(ReactionContext context)
    {
        context.Self.Context.GravityScale = 1f;

        context.Motion.OverrideMotionPolicy(
            previousMovementPolicy,
            previousRotationPolicy);

        this.context = null;

        base.Exit(context);
    }

    private void SetupForces(DamageEvent hit, ReactionContext context)
    {
        Vector3 incomingForce = hit.HitForce;

        Vector3 horizontalDirection = new Vector3(
            incomingForce.x,
            0f,
            incomingForce.z);

        if (horizontalDirection.sqrMagnitude > 0.0001f)
            horizontalDirection.Normalize();

        horizontalVelocity =
            horizontalDirection * horizontalForceMultiplier;

        float verticalForce =
            incomingForce.y * verticalForceMultiplier;

        verticalForce =
            Mathf.Max(verticalForce, minimumVerticalBoost);

        context.Self.verticalVelocity = verticalForce;
    }
}