using UnityEngine;

public class ReactionContext
{
    public readonly Animator Animator;
    public readonly ReactionMotionAdapter Motion;
    public readonly Character Self;
    public readonly Transform Transform;
    public readonly System.Action<DamageEvent> EnqueueReaction;
    //TODO: Feedback Feedback;

    public ReactionContext(Character self,
                           Animator animator,
                           ReactionMotionAdapter motion,
                           //TODO: Feedback,
                           System.Action<DamageEvent> enqueueReaction = null)
    {
        Self = self;
        Animator = animator;
        Motion = motion;
        Transform = self.transform;
        EnqueueReaction = enqueueReaction;
    }
}
