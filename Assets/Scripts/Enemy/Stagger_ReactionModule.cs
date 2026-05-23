using System.Threading.Tasks;
using UnityEngine;

public class Stagger_ReactionModule : ReactionModule
{
    [SerializeField] private string staggerAnim = "Hit_Blend";
    [Tooltip("Fallback if Reaction Duration could not be found")]
    [SerializeField] private float defaultHitstun = 0.2f;
    [Tooltip("Amplify incoming Hit Force")]
    [SerializeField] private float forceMultiplier = 1f;

    [Header("Impulse Info")]
    [SerializeField] private float impulseDuration = 0.05f;
    private float impulseTimer = 0f;
    private Vector3 impulseVelocity;
    private Vector3 forcePerSecond;

    [Header("Reaction Timings")]
    private ReactionMotionAdapter motor;
    private float timer = 0f;
    private float totalDuration = 0f;
    private float normalizedTime = 0f;
    private int hitVersion = 0;

    public override ReactionPriority Priority => ReactionPriority.Low;
    public override bool AllowChaining => true;

    public override bool CanHandle(DamageEvent ev, ReactionContext ctx)
    {
        if (ev.Effect == HitImpactType.Light) return true;
        return false;
    }

    public override void Enter(DamageEvent ev, ReactionContext ctx)
    {
        base.Enter(ev, ctx);

        motor = ctx.Motion;

        timer = 0f;
        totalDuration = 0f;
        normalizedTime = 0f;

        impulseVelocity = ev.HitForce * forceMultiplier;
        impulseTimer = impulseDuration;

        hitVersion++;
        int version = hitVersion;

        //ctx.Animator.PlayAnim(staggerAnim, 0.1f);

        _ = SuspendCharacterAsync(ev, ctx, version);
    }

    private async Task SuspendCharacterAsync(DamageEvent ev, ReactionContext ctx, int version)
    {
        float duration = await ctx.Animator.ApplyHit(ev, staggerAnim);

        // Ignore old async calls
        if (version != hitVersion) return;

        if (duration <= 0f)
            duration = defaultHitstun;

        totalDuration = duration;
        ctx.Self.Suspend(totalDuration);
    }

    public override void Tick(float deltaTime)
    {
        timer += deltaTime;

        if (impulseTimer > 0f)
        {
            float dt = deltaTime;

            float t = impulseTimer / impulseDuration; 
            float ease = t * t; // Ease-out curve

            Vector3 currentVelocity = impulseVelocity * ease;

            motor.AddPositionDelta(currentVelocity * dt);

            impulseTimer -= dt;
        }

        // Stop Execution temporarily until Total Duration is
        // not identified by SuspendCharacterAsync
        if (totalDuration == 0) return;

        normalizedTime = timer / totalDuration;

        if (normalizedTime > cancelTime)
            CanBreak = true;

        if (timer >= totalDuration)
            IsFinished = true;
    }

    public override void Exit(ReactionContext ctx)
    {
        base.Exit(ctx);

        ctx.Animator.EndHitAnim(forceExit);
        //ctx.StateMachine.SetHitReactionEnd();
    }
}