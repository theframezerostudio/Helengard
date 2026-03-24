using UnityEngine;

public class AirJuggle_ReactionModule : ReactionModule
{
    [Header("Juggle Settings")]
    [SerializeField] private float baseVerticalPop = 3f;
    [SerializeField] private float baseHorizontalPush = 1f;
    [SerializeField] private float juggleDecayRate = 0.75f;
    [SerializeField] private float maxHeightOffset = 6f;

    [SerializeField] private AnimationCurve gravityCurve;
    [SerializeField] private float safetyTime = 2.5f;

    [SerializeField] private string airHitAnim = "Hit_Air";

    private ReactionContext ctx;

    private float timer;
    private float gravityCurveTime;
    private float startHeight;

    private MovementMotionPolicy prevMovePolicy;
    private RotationMotionPolicy prevRotPolicy;

    [SerializeField] private float juggleMultiplier = 1f;
    private Vector3 horizontalVelocity;

    public override ReactionPriority Priority => ReactionPriority.Medium;

    public override bool CanHandle(DamageEvent ev, ReactionContext ctx)
    {
        if (ctx.Self.Context.isGrounded)
            return false;

        if (!ev.CanChain)
            return false;

        return true;
    }

    public override void Enter(DamageEvent ev, ReactionContext ctx)
    {
        base.Enter(ev, ctx);

        this.ctx = ctx;

        timer = 0f;
        gravityCurveTime = 0f;
        startHeight = ctx.Self.transform.position.y;

        ctx.Animator.CrossFade(airHitAnim, 0.05f);

        ctx.Motion.GetMotionPolicy(out prevMovePolicy, out prevRotPolicy);
        ctx.Motion.OverrideMotionPolicy(MovementMotionPolicy.NoRootMotion, RotationMotionPolicy.YawOnly);

        Vector3 attackerToEnemy = ctx.Self.transform.position - ev.Attacker.position;
        attackerToEnemy.y = 0;
        attackerToEnemy.Normalize();

        horizontalVelocity = attackerToEnemy * baseHorizontalPush;

        float vertical = baseVerticalPop * juggleMultiplier;

        ctx.Self.verticalVelocity = vertical;

        ctx.Self.Context.GravityScale = gravityCurve.Evaluate(0);

        juggleMultiplier *= juggleDecayRate;
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

        float currentHeight = ctx.Self.transform.position.y;
        if (currentHeight - startHeight > maxHeightOffset)
        {
            ctx.Self.verticalVelocity = Mathf.Min(ctx.Self.verticalVelocity, 0f);
        }

        ctx.Motion.AddPositionDelta(horizontalVelocity * dt);

        float curveValue = gravityCurve.Evaluate(gravityCurveTime);
        ctx.Self.Context.GravityScale = curveValue;

        if (ctx.Self.Context.isGrounded && ctx.Self.verticalVelocity <= 0)
        {
            IsFinished = true;
        }
    }

    public override void Exit(ReactionContext ctx)
    {
        base.Exit(ctx);

        ctx.Motion.OverrideMotionPolicy(prevMovePolicy, prevRotPolicy);

        ctx.Self.Context.GravityScale = 1f;
    }
}
