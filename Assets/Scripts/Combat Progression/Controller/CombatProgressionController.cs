using System;
using UnityEngine;

public sealed class CombatProgressionController : MonoBehaviour
{
    public event Action<CombatProgressionStateSnapshot> StateChanged;
    public event Action<CombatProgressionApplicationResult> ProgressionApplied;
    public event Action<CombatProgressionDecayResult> ProgressionDecayed;
    public event Action<CombatProgressionRankStabilityResult> RankChanged;

    [Header("References")]
    [SerializeField] private CombatEventHub eventHub;
    [SerializeField] private CombatProgressionProfile profile;

    [Header("Pipeline")]
    [SerializeField] private CombatProgressionEventAdapter adapter = new();
    [SerializeField] private CombatProgressionEvaluator evaluator = new();
    [SerializeField] private CombatProgressionScoreApplier scoreApplier = new();
    [SerializeField] private CombatProgressionStableRankCalculator rankCalculator = new();
    [SerializeField] private CombatProgressionDecayProcessor decayProcessor = new();

    [Header("Runtime")]
    [SerializeField] private CombatProgressionRuntime runtime = new();

    [Header("Behaviour")]
    [SerializeField] private bool initializeOnAwake = true;
    [SerializeField] private bool autoActivateCombatOnValidSignal = true;
    [SerializeField] private bool tickDecay = true;
    [SerializeField] private bool logPipeline;

    private bool initialized;
    private bool subscribed;

    public CombatProgressionProfile Profile => profile;
    public CombatProgressionRuntime Runtime => runtime;

    public float CurrentScore => runtime != null ? runtime.CurrentScore : 0f;
    public CombatRankGrade CurrentRank => runtime != null ? runtime.CurrentRank : CombatRankGrade.D;
    public float CurrentMultiplier => runtime != null ? runtime.CurrentMultiplier : 1f;
    public bool IsCombatActive => runtime != null && runtime.IsCombatActive;

    private void Awake()
    {
        ResolveReferences();

        if (initializeOnAwake)
            Initialize();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Update()
    {
        if (!initialized)
            return;

        if (!tickDecay)
            return;

        TickDecay(Time.deltaTime);
    }

    public void Initialize()
    {
        if (runtime == null)
            runtime = new CombatProgressionRuntime();

        runtime.Initialize(profile);
        initialized = true;

        NotifyStateChanged();
    }

    public void ResetProgression()
    {
        if (runtime == null)
            runtime = new CombatProgressionRuntime();

        runtime.Reset(profile);
        initialized = true;

        NotifyStateChanged();
    }

    public void BeginCombat()
    {
        EnsureInitialized();

        runtime.SetCombatActive(true);
        NotifyStateChanged();
    }

    public void EndCombat()
    {
        EnsureInitialized();

        runtime.SetCombatActive(false);
        NotifyStateChanged();
    }

    public void SetCombatActive(bool active)
    {
        EnsureInitialized();

        runtime.SetCombatActive(active);
        NotifyStateChanged();
    }

    public CombatProgressionStateSnapshot CreateSnapshot()
    {
        EnsureInitialized();

        return runtime.CreateSnapshot(profile);
    }

    public bool ProcessCombatEvent(CombatEventData eventData)
    {
        EnsureInitialized();

        if (profile == null)
            return false;

        if (!adapter.TryCreateSignal(eventData, profile, out CombatProgressionSignal signal))
            return false;

        if (autoActivateCombatOnValidSignal && !runtime.IsCombatActive)
            runtime.SetCombatActive(true);

        CombatMemory memory = eventHub != null ? eventHub.Memory : null;

        if (!evaluator.Evaluate(signal, runtime, profile, memory, 
            out CombatProgressionRuleEvaluation evaluation))
            return false;

        bool applied = scoreApplier.Apply(
            evaluation,
            runtime,
            profile,
            rankCalculator,
            out CombatProgressionApplicationResult applicationResult);

        if (!applied)
            return false;

        if (logPipeline)
            Debug.Log("Combat Progression Applied. Score: " + applicationResult.ScoreBefore + " -> " + applicationResult.ScoreAfter + ", Rank: " + applicationResult.RankBefore + " -> " + applicationResult.RankAfter);

        ProgressionApplied?.Invoke(applicationResult);

        if (applicationResult.RankChanged)
            NotifyRankChanged(applicationResult.RankBefore, applicationResult.RankAfter);

        NotifyStateChanged();

        return true;
    }

    public bool TickDecay(float deltaTime)
    {
        EnsureInitialized();

        bool decayed = decayProcessor.Tick(
            deltaTime,
            runtime,
            profile,
            rankCalculator,
            out CombatProgressionDecayResult decayResult);

        if (!decayed)
            return false;

        ProgressionDecayed?.Invoke(decayResult);

        if (decayResult.RankChanged)
            RankChanged?.Invoke(decayResult.RankResult);

        NotifyStateChanged();

        return true;
    }

    private void HandleCombatEventRaised(CombatEventData eventData)
    {
        ProcessCombatEvent(eventData);
    }

    private void ResolveReferences()
    {
        if (eventHub == null)
            eventHub = GetComponent<CombatEventHub>();

        if (eventHub == null)
            eventHub = GetComponentInParent<CombatEventHub>();

        runtime ??= new CombatProgressionRuntime();
    }

    private void Subscribe()
    {
        if (subscribed)
            return;

        ResolveReferences();

        if (eventHub == null)
            return;

        eventHub.EventRaised += HandleCombatEventRaised;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed)
            return;

        if (eventHub != null)
            eventHub.EventRaised -= HandleCombatEventRaised;

        subscribed = false;
    }

    private void EnsureInitialized()
    {
        if (initialized)
            return;

        Initialize();
    }

    private void NotifyStateChanged()
    {
        if (runtime == null)
            return;

        StateChanged?.Invoke(runtime.CreateSnapshot(profile));
    }

    private void NotifyRankChanged(CombatRankGrade previousRank, CombatRankGrade newRank)
    {
        CombatProgressionRankChangeType changeType = CombatProgressionRankChangeType.None;

        if (newRank > previousRank)
            changeType = CombatProgressionRankChangeType.Promoted;
        else if (newRank < previousRank)
            changeType = CombatProgressionRankChangeType.Demoted;

        CombatProgressionRankStabilityResult result = new CombatProgressionRankStabilityResult(
            previousRank,
            newRank,
            changeType);

        RankChanged?.Invoke(result);
    }

    private void Reset()
    {
        ResolveReferences();
    }
}