public enum ReactionPriority
{
    None = 0,
    Low = 100,      // small staggers, light hit
    Medium = 200,   // stun
    High = 300,     // knockdown
    Critical = 400, // grab, special forced states
}

public interface IReactionModule
{
    ReactionPriority Priority { get; }

    // Quick check before trying to start. Light-weight.
    bool CanHandle(DamageEvent ev, ReactionContext ctx);

    // Called when module is chosen to start.
    void Enter(DamageEvent ev, ReactionContext ctx);

    // Called every frame while active (controller calls this).
    void Tick(float deltaTime);

    // Called when reaction ends or is interrupted.
    void Exit(ReactionContext ctx);

    // Returns true if this module is finished and can be removed.
    bool IsFinished { get; }

    // If this reaction allows chaining for follow-ups
    bool AllowChaining { get; }
}
