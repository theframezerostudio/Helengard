public interface IReactionModule
{
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
