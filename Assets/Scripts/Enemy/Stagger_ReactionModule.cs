using System.Threading.Tasks;
using UnityEngine;

public class Stagger_ReactionModule : ReactionModule
{
    [Tooltip("Animator Follower of the Character")]
    [SerializeField] private AnimatorFollower animatorFollower;
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

    public override ReactionPriority Priority => ReactionPriority.Low;
    public override bool AllowChaining => true;

    public override bool CanHandle(DamageEvent ev, ReactionContext ctx)
    {
        if (ev.Effect == HitEffectType.Light) return true;
        return false;
    }

    public override void Enter(DamageEvent ev, ReactionContext ctx)
    {
        base.Enter(ev, ctx);

        // TODO: Feedback

        motor = ctx.Motion;

        totalDuration = 0;
        _ = SuspendCharacterAsync(ev, ctx);

        // ctx.Self -> Send to suspended state
        //ctx.StateMachine.SetHitReactionStart();
    }

    private async Task SuspendCharacterAsync(DamageEvent ev, ReactionContext ctx)
    {
        totalDuration = await animatorFollower.ApplyHit(ev);
        timer = 0f;

        if (ev.StunDuration > 0f)
        {
            // Handle stun after stagger
        }

        if (totalDuration <= 0f) totalDuration = defaultHitstun;

        impulseVelocity = ev.HitForce * forceMultiplier;
        impulseTimer = impulseDuration;

        ctx.Self.Suspend(totalDuration);
    }

    public override void Tick(float deltaTime)
    {
        timer += deltaTime;

        if (impulseTimer > 0f)
        {
            float dt = Mathf.Min(deltaTime, impulseTimer);
            motor.AddPositionDelta(impulseVelocity * dt);
            impulseTimer -= dt;
        }

        normalizedTime = timer / totalDuration;

        if (normalizedTime > cancelTime)
            CanBreak = true;

        // Stop Execution temporarily until Total Duration is
        // not identified by SuspendCharacterAsync
        if (totalDuration == 0) return;

        if (timer >= totalDuration)
            IsFinished = true;
    }

    public override void Exit(ReactionContext ctx)
    {
        base.Exit(ctx);

        animatorFollower.EndHitAnim(forceExit);
        //ctx.StateMachine.SetHitReactionEnd();
    }
}