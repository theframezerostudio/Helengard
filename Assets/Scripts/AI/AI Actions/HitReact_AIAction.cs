using System;
using UnityEngine;

public class HitReact_AIAction : AIAction
{
    [SerializeField] private Target target;
    [SerializeField] private ReactionController reactionController;
    
    private bool isLocked = false;

    public override void Enter(Character Owner, StateContext stateContext)
    {
        context = stateContext;

        DamageEvent lastHit = target.GetRecentHit();
        OnHit(lastHit);

        target.onHit += OnHit;
    }

    public override void Exit()
    {
        target.onHit -= OnHit;
    }

    public override void Tick()
    {
        if (isLocked && !reactionController.IsReacting)
        {
            isLocked = false;
            context.State.Unlock();
        }
    }

    private void OnHit(DamageEvent ev)
    {
        if (isLocked)
            context.State.Unlock();

        context.State.Lock();
        isLocked = true;

        reactionController.HandleHit(ev);
    }
}
