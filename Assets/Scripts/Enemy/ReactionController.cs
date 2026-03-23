using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class ReactionController : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private List<ReactionModule> modules = new();

    [SerializeField, ReadOnly] private ReactionModule active;
    private ReactionContext ctx;
    private readonly Queue<DamageEvent> queue = new();

    private Coroutine recoveryRoutine = null;
    [SerializeField, ReadOnly] private bool isFinished = false;

    /// <summary>
    /// Is Reacting is set to false when the
    /// whole reaction sequence of a particular module is completed
    /// </summary>
    public bool IsReacting => !isFinished;

    /// <summary>
    /// Is Cancellable is to true when the 
    /// active module can be broken mid animation
    /// </summary>
    public bool IsCancellable => active != null && active.CanBreak;

    private void Start()
    {
        Animator animator = character.Animator;
        ReactionMotionAdapter motion = new (character);

        ctx = new ReactionContext(
            character,
            animator,
            motion,
            //stateMachine,
            EnqueueHit
        );
    }

    //TODO: Shift to Enumerator based on event to reduce excessive calls
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


    public void HandleHit(DamageEvent ev)
    {
        TryStart(ev);
    }

    private void TryStart(DamageEvent ev)
    {
        if (ev == null)
            { return; }

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

    public bool TryCancel()
    {
        if (!IsCancellable) return false;

        if (active != null)
            active.Exit(ctx);

        HandelModuleExit(null);

        return true;
    }

    private void StartModule(ReactionModule module, DamageEvent ev)
    {
        isFinished = false;

        active = module;
        active.onExit += HandelModuleExit;
        module.Enter(ev, ctx);
    }

    private void HandelModuleExit(ActionData recoveryData)
    {
        active.onExit -= HandelModuleExit;

        if (queue.Count != 0) return;

        if (recoveryData != null)
        {
            if (recoveryRoutine != null)
            {
                StopCoroutine(recoveryRoutine);
                recoveryRoutine = null;
            }
            recoveryRoutine = StartCoroutine(RecoveryRoutine(recoveryData));
            //character.Recover(recoveryData);
        }
        else
        {
            isFinished = true;
        }
    }

    private IEnumerator RecoveryRoutine(ActionData recoveryData)
    {
        character.PlayAnim(recoveryData.animState, recoveryData.transitionTime);
        yield return new WaitForSeconds(recoveryData.duration);

        isFinished = true;
        recoveryRoutine = null;
    }

    private void EnqueueHit(DamageEvent ev)
    {
        queue.Enqueue(ev);
    }
}