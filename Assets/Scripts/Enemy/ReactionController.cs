using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ReactionController : MonoBehaviour
{
    /// <summary>
    /// One possible module that may respond to a reaction key.
    /// Candidates are checked in inspector order.
    /// </summary>
    [Serializable]
    private sealed class ReactionCandidate
    {
        [Tooltip("Component that executes this reaction when CanHandle passes.")]
        public ReactionModule module;

        [Tooltip(
            "Used when this reaction attempts to replace an already active reaction. " +
            "Higher values interrupt lower values.")]
        public int interruptPriority = 100;

        [Tooltip(
            "Allow this candidate to interrupt a different active reaction " +
            "when both have the same interrupt priority.")]
        public bool canInterruptEqualPriority;
    }

    /// <summary>
    /// One attack-authored key can resolve into several possible reactions.
    /// The first candidate whose CanHandle passes is selected.
    /// </summary>
    [Serializable]
    private sealed class ReactionBinding
    {
        [Tooltip("Identity-only reaction key referenced by an AttackProfile.")]
        public ReactionKey reactionKey;

        [Tooltip(
            "Evaluated from top to bottom. " +
            "The first module whose CanHandle returns true executes.")]
        public List<ReactionCandidate> reactions = new();
    }

    [Header("References")]
    [SerializeField] private Character character;

    [Tooltip("Hit animator of this character.")]
    [SerializeField] private HitAnimationController hitAnimator;

    [Header("Reaction Bindings")]
    [Tooltip(
        "Each key contains an ordered list of possible reactions. " +
        "Specialised reactions should appear above fallback reactions.")]
    [SerializeField] private List<ReactionBinding> bindings = new();

    [Header("Runtime Debug")]
    [SerializeField, ReadOnly] private ReactionKey activeReactionKey;
    [SerializeField, ReadOnly] private ReactionModule active;
    [SerializeField, ReadOnly] private int activeInterruptPriority;

    private readonly Dictionary<ReactionKey, ReactionBinding> bindingByKey = new();
    private readonly Queue<DamageEvent> queuedHits = new();

    private ReactionCandidate activeCandidate;
    private ReactionContext context;
    private Coroutine recoveryRoutine;

    public bool IsReacting => active != null || recoveryRoutine != null;

    public bool IsCancellable => active != null && active.CanBreak;

    private void Start()
    {
        ReactionMotionAdapter motion = new ReactionMotionAdapter(character);

        context = new ReactionContext(
            character,
            hitAnimator,
            motion,
            EnqueueHit);

        BuildLookup();
    }

    private void Update()
    {
        TickActiveReaction();
        TryProcessQueuedHit();
    }

    /// <summary>
    /// Entry point used when this character receives a resolved damage event.
    /// </summary>
    public void HandleHit(DamageEvent hit)
    {
        TryStart(hit);
    }

    /// <summary>
    /// Attempts to manually cancel the active reaction when its module allows it.
    /// </summary>
    public bool TryCancel()
    {
        if (!IsCancellable)
            return false;

        InterruptActiveReaction();
        EndRecoveryRoutine();

        hitAnimator.SetIntent(0f);

        return true;
    }

    /// <summary>
    /// Builds a direct ReactionKey to ReactionBinding lookup.
    /// A key may appear only once because its modules are now stored under its list.
    /// </summary>
    private void BuildLookup()
    {
        bindingByKey.Clear();

        foreach (ReactionBinding binding in bindings)
        {
            if (binding == null)
                continue;

            if (binding.reactionKey == null)
            {
                Debug.LogWarning(
                    $"[{nameof(ReactionController)}] A reaction binding on '{name}' has no ReactionKey.",
                    this);

                continue;
            }

            if (binding.reactions == null || binding.reactions.Count == 0)
            {
                Debug.LogWarning(
                    $"[{nameof(ReactionController)}] Reaction key " +
                    $"'{binding.reactionKey.name}' on '{name}' has no candidate modules.",
                    this);

                continue;
            }

            if (bindingByKey.ContainsKey(binding.reactionKey))
            {
                Debug.LogError(
                    $"[{nameof(ReactionController)}] Duplicate binding group for reaction key " +
                    $"'{binding.reactionKey.name}' on '{name}'. " +
                    $"Place all candidates for the same key inside one reactions list.",
                    this);

                continue;
            }

            ValidateCandidates(binding);

            bindingByKey.Add(binding.reactionKey, binding);
        }
    }

    private void ValidateCandidates(ReactionBinding binding)
    {
        HashSet<ReactionModule> registeredModules = new();

        for (int i = 0; i < binding.reactions.Count; i++)
        {
            ReactionCandidate candidate = binding.reactions[i];

            if (candidate == null || candidate.module == null)
            {
                Debug.LogWarning(
                    $"[{nameof(ReactionController)}] Candidate index {i} under key " +
                    $"'{binding.reactionKey.name}' on '{name}' has no module.",
                    this);

                continue;
            }

            if (!registeredModules.Add(candidate.module))
            {
                Debug.LogWarning(
                    $"[{nameof(ReactionController)}] Module " +
                    $"'{candidate.module.GetType().Name}' is listed more than once under key " +
                    $"'{binding.reactionKey.name}' on '{name}'.",
                    this);
            }
        }
    }

    private void TickActiveReaction()
    {
        if (active == null)
            return;

        active.Tick(Time.deltaTime);

        if (active.IsFinished)
            CompleteActiveReaction();
    }

    private void TryProcessQueuedHit()
    {
        if (active != null || queuedHits.Count == 0)
            return;

        DamageEvent queuedHit = queuedHits.Dequeue();
        TryStart(queuedHit);
    }

    private void TryStart(DamageEvent hit)
    {
        if (hit == null || hit.ExpectedReaction == null)
            return;

        if (!TryResolveFirstValidCandidate(hit, out ReactionCandidate incomingCandidate))
            return;

        ReactionModule incomingModule = incomingCandidate.module;

        if (active == null)
        {
            StartModule(hit.ExpectedReaction, incomingCandidate, hit);
            return;
        }

        if (ReferenceEquals(incomingModule, active))
        {
            if (!active.AllowChaining || !hit.CanChain)
                return;

            InterruptActiveReaction();
            StartModule(hit.ExpectedReaction, incomingCandidate, hit);
            return;
        }

        if (!CanInterruptActive(incomingCandidate))
            return;

        InterruptActiveReaction();
        StartModule(hit.ExpectedReaction, incomingCandidate, hit);
    }

    /// <summary>
    /// Gets the binding for the hit's key, then evaluates its reaction list
    /// from top to bottom. The first module whose CanHandle passes wins.
    /// </summary>
    private bool TryResolveFirstValidCandidate(
        DamageEvent hit,
        out ReactionCandidate selectedCandidate)
    {
        selectedCandidate = null;

        if (!bindingByKey.TryGetValue(
                hit.ExpectedReaction,
                out ReactionBinding binding))
        {
            Debug.LogWarning(
                $"[{nameof(ReactionController)}] No binding exists for reaction key " +
                $"'{hit.ExpectedReaction.name}' on '{name}'.",
                this);

            return false;
        }

        foreach (ReactionCandidate candidate in binding.reactions)
        {
            if (candidate == null || candidate.module == null)
                continue;

            if (!candidate.module.CanHandle(hit, context))
                continue;

            selectedCandidate = candidate;
            return true;
        }

        return false;
    }

    private bool CanInterruptActive(ReactionCandidate incomingCandidate)
    {
        if (activeCandidate == null)
            return true;

        if (incomingCandidate.interruptPriority > activeCandidate.interruptPriority)
            return true;

        if (incomingCandidate.interruptPriority < activeCandidate.interruptPriority)
            return false;

        return incomingCandidate.canInterruptEqualPriority;
    }

    private void StartModule(ReactionKey reactionKey, ReactionCandidate candidate, DamageEvent hit)
    {
        EndRecoveryRoutine();

        activeReactionKey = reactionKey;
        activeCandidate = candidate;
        activeInterruptPriority = candidate.interruptPriority;

        active = candidate.module;

        active.onExit -= HandleModuleExit;
        active.onExit += HandleModuleExit;

        active.Enter(hit, context);
    }

    /// <summary>
    /// Called when the active module naturally completes.
    /// Normal completion is allowed to supply recovery ActionData.
    /// </summary>
    private void CompleteActiveReaction()
    {
        if (active == null)
            return;

        ReactionModule completedModule = active;

        completedModule.Exit(context);

        if (ReferenceEquals(active, completedModule))
            ClearActiveReaction();
    }

    /// <summary>
    /// Immediately removes the active module without beginning its normal recovery.
    /// Used when replaced by a stronger reaction or manually cancelled.
    /// </summary>
    private void InterruptActiveReaction()
    {
        if (active == null)
            return;

        ReactionModule interruptedModule = active;

        interruptedModule.onExit -= HandleModuleExit;
        interruptedModule.Exit(context);

        ClearActiveReaction();
    }

    private void ClearActiveReaction()
    {
        activeReactionKey = null;
        activeInterruptPriority = 0;

        activeCandidate = null;
        active = null;
    }

    private void HandleModuleExit(ActionData recoveryData)
    {
        if (active != null)
            active.onExit -= HandleModuleExit;

        if (queuedHits.Count > 0)
            return;

        if (recoveryData == null)
            return;

        EndRecoveryRoutine();

        recoveryRoutine = StartCoroutine(
            RecoveryRoutine(recoveryData));
    }

    private IEnumerator RecoveryRoutine(ActionData recoveryData)
    {
        hitAnimator.PlayAnim(
            recoveryData.animState,
            recoveryData.transitionTime);

        yield return new WaitForSeconds(recoveryData.duration);

        hitAnimator.SetIntent(0f);

        recoveryRoutine = null;
    }

    private void EndRecoveryRoutine()
    {
        if (recoveryRoutine == null)
            return;

        StopCoroutine(recoveryRoutine);
        recoveryRoutine = null;
    }

    private void EnqueueHit(DamageEvent hit)
    {
        if (hit != null)
            queuedHits.Enqueue(hit);
    }
}