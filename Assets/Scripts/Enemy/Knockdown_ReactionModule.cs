using UnityEngine;

public class Knockdown_ReactionModule : ReactionModule
{
    [Header("Knockdown Settings")]
    [SerializeField] private float knockbackStrength = 2.5f;
    [SerializeField] private float fallDuration = 1.2f;
    [SerializeField] private string knockdownAnim = "Hit_Knockdown";
    [SerializeField] private AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private ReactionContext ctx;
    private float timer;

    private MovementMotionPolicy prevMovePolicy;
    private RotationMotionPolicy prevRotPolicy;

    private Vector3 knockbackVelocity;
    private Vector3 forcePerSecond;
    private bool isDone = false;

    private DamageEvent currentEvent;
    private Vector3 initialVelocity;

    public override ReactionPriority Priority => ReactionPriority.Medium;

    public override bool CanHandle(DamageEvent ev, ReactionContext ctx)
    {
        if (ctx.Self.Context.isGrounded == false)
            return false;

        if (ev.Effect != HitEffectType.Heavy)
            return false;

        if (ev.SwingType != HitSwing.LeftToRight && ev.SwingType != HitSwing.RightToLeft)
            return false;

        return true;
    }

    public override void Enter(DamageEvent ev, ReactionContext ctx)
    {
        base.Enter(ev, ctx);

        this.ctx = ctx;
        timer = 0f;
        isDone = false;

        ctx.Animator.CrossFade(knockdownAnim, 0.1f);

        ctx.Motion.GetMotionPolicy(out prevMovePolicy, out prevRotPolicy);

        ctx.Motion.OverrideMotionPolicy(MovementMotionPolicy.NoRootMotion, RotationMotionPolicy.YawOnly);

        Vector3 horizontalForce = ev.HitForce;
        horizontalForce.y = 0;

        if (horizontalForce.sqrMagnitude < 0.01f)
        {
            horizontalForce = ctx.Self.transform.position - ev.Attacker.position;
            horizontalForce.y = 0;
        }

        horizontalForce.Normalize();

        float magnitude = Mathf.Max(ev.HitForce.magnitude, knockbackStrength);
        initialVelocity = horizontalForce * magnitude;

        //knockbackVelocity = direction * knockbackStrength;

        Quaternion faceAwayRotation = Quaternion.LookRotation(horizontalForce);
        ctx.Motion.AddRotation(faceAwayRotation);
    }

    public override void Tick(float dt)
    {
        if (isDone) return;

        timer += dt;

        float normalizedTime = timer / fallDuration;
        normalizedTime = Mathf.Clamp01(normalizedTime);

        float strength = knockbackCurve.Evaluate(normalizedTime);
        Vector3 deltaVelocity = initialVelocity * strength;

        if (timer < 0.2f)
        {
            ctx.Motion.AddPositionDelta(deltaVelocity * dt);
        }

        if (timer >= fallDuration)
        {
            isDone = true;
            IsFinished = true;
        }
    }

    public override void Exit(ReactionContext ctx)
    {
        base.Exit(ctx);

        ctx.Motion.OverrideMotionPolicy(prevMovePolicy, prevRotPolicy);

        // Recovery 
    }
}
