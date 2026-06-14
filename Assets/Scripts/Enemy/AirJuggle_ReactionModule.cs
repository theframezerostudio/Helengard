using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

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

    [Header("Impulse Settings")]
    [SerializeField] private float impulseMultiplier = 1f;
    [SerializeField, Min(0f)] private float impulseDuration = 0.05f;

    [SerializeField]
    private AnimationCurve impulseFalloff =
        AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private ReactionMotionAdapter motor;
    private Vector3 impulseVelocity;

    public override bool CanHandle(DamageEvent ev, ReactionContext ctx)
    {
        if (ctx.Self.Context.isGrounded)
            return false;

        if (!ev.CanChain)
            return false;
        Debug.Log("Air juggle reaction can handle the event.");
        return true;
    }

    public override void Enter(DamageEvent ev, ReactionContext ctx)
    {
        base.Enter(ev, ctx);

        this.ctx = ctx;

        motor = ctx.Motion;
        timer = 0f;
        gravityCurveTime = 0f;

        startHeight = ctx.Self.transform.position.y;
        impulseVelocity = impulseMultiplier * ev.HitForce;

        ctx.Animator.PlayAnim(airHitAnim, 0.05f);

        motor.GetMotionPolicy(out prevMovePolicy, out prevRotPolicy);
        motor.OverrideMotionPolicy(MovementMotionPolicy.NoRootMotion, RotationMotionPolicy.YawOnly);
        
        Vector3 attackerToEnemy = ctx.Self.transform.position - ev.Attacker.position;
        attackerToEnemy.y = 0;
        attackerToEnemy.Normalize();

        horizontalVelocity = attackerToEnemy * baseHorizontalPush;

        float vertical = baseVerticalPop * juggleMultiplier;

        ctx.Self.verticalVelocity = vertical;

        ctx.Self.Context.GravityScale = gravityCurve.Evaluate(0);

        if (juggleDecayRate > 0)
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

        ApplyImpulse(timer);

        float currentHeight = ctx.Self.transform.position.y;
        if (currentHeight - startHeight > maxHeightOffset)
        {
            ctx.Self.verticalVelocity = Mathf.Min(ctx.Self.verticalVelocity, 0f);
        }

        motor.AddPositionDelta(horizontalVelocity * dt);

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

        motor.OverrideMotionPolicy(prevMovePolicy, prevRotPolicy);

        ctx.Self.Context.GravityScale = 1f;
    }

    private void ApplyImpulse(float deltaTime)
    {
        if (motor == null || impulseDuration <= 0f || timer >= impulseDuration)
        {
            return;
        }

        float normalizedTime = Mathf.Clamp01(timer / impulseDuration);

        float falloff = impulseFalloff.Evaluate(normalizedTime);

        motor.AddPositionDelta(deltaTime * falloff * impulseVelocity);
    }
}
