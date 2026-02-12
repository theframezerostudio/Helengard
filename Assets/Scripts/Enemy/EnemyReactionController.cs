using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyReactionController : MonoBehaviour
{
    [SerializeField] private List<ReactionModule> modules = new();
    [SerializeField] private ReactionModule active;

    private ReactionContext ctx;
    private Enemy enemy;

    private Queue<DamageEvent> queue = new();

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        Animator animator = enemy.Animator;
        ReactionMotionAdapter motion = new (enemy);

        ctx = new ReactionContext(
            enemy,
            animator,
            motion,
            //stateMachine,
            EnqueueHit
        );
    }

    private void Update()
    {
        if (active != null)
        {
            active.Tick(Time.deltaTime);

            if (active.IsFinished)
            {
                active.Exit(ctx);
                active = null;
            }
        }

        if (active == null && queue.Count > 0)
        {
            var ev = queue.Dequeue();
            //TryStart(ev);
        }
    }

    public void OnDamageReceived(DamageEvent ev)
    {
        TryStart(ev);
    }

    private void TryStart(DamageEvent ev)
    {
        List<ReactionModule> candidates = modules.Where(m => m.CanHandle(ev, ctx)).ToList();
        if (candidates.Count == 0) return;

        ReactionModule chosen = candidates.OrderByDescending(c => c.Priority).First();

        if (active == null)
        {
            StartModule(chosen, ev);
            return;
        }

        if (chosen.Priority >= active.Priority)
        {
            active.onExit -= HandelModuleExit;
            active.Exit(ctx);
            
            StartModule(chosen, ev);
            return;
        }

        chosen = null;
        EnqueueHit(ev);
    }

    private void StartModule(ReactionModule module, DamageEvent ev)
    {
        active = module;
        active.onExit += HandelModuleExit;
        module.Enter(ev, ctx);
    }

    private void HandelModuleExit(ActionData recoveryData)
    {
        active.onExit -= HandelModuleExit;

        if (queue.Count != 0) return;

        if (recoveryData == null)
            enemy.Unsuspend();
        else
            enemy.Recover(recoveryData);
    }

    private void EnqueueHit(DamageEvent ev)
    {
        queue.Enqueue(ev);
    }
}