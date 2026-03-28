using UnityEngine;

public class ReactionContext
{
    public readonly HitAnimationController Animator;
    public readonly ReactionMotionAdapter Motion;
    public readonly Character Self;
    public readonly Transform Transform;
    public readonly System.Action<DamageEvent> EnqueueReaction;
    //TODO: Feedback Feedback;

    public ReactionContext(Character self,
                           HitAnimationController animator,
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
