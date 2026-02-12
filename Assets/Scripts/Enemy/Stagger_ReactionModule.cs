using System.Threading.Tasks;
using UnityEngine;

public class Stagger_ReactionModule : ReactionModule
{
    [SerializeField] private float defaultHitstun = 0.2f;

    [SerializeField] private AnimatorFollower animatorFollower;
    public override ReactionPriority Priority => ReactionPriority.Low;
    public override bool AllowChaining => true;

    private Vector3 forcePerSecond;

    private ReactionMotionAdapter motor;
    private float timer = 0f;
    private float totalDuration = 0f;

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
        Debug.Log("Suspend" + totalDuration);
        timer = 0f;

        if (ev.StunDuration > 0f)
        {
            // Handle stun after stagger
        }

        if (totalDuration <= 0f) totalDuration = defaultHitstun;

        forcePerSecond = ev.HitForce / totalDuration;

        ctx.Self.Suspend(totalDuration);
    }

    public override void Tick(float deltaTime)
    {
        timer += deltaTime;
        if (totalDuration == 0) return;

        Vector3 frameForce = forcePerSecond * deltaTime;
        motor.AddPositionDelta(frameForce);
        if (timer >= totalDuration)
        {
            IsFinished = true;
        }
    }

    public override void Exit(ReactionContext ctx)
    {
        base.Exit(ctx);

        animatorFollower.EndHitAnim();
        //ctx.StateMachine.SetHitReactionEnd();
    }
}