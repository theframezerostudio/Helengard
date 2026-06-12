using System;
using UnityEngine;

public sealed class CombatProgressionFacade : MonoBehaviour, ICombatProgressionReader
{
    public event Action<CombatProgressionStateSnapshot> StateChanged;
    public event Action<CombatProgressionRankStabilityResult> RankChanged;
    public event Action<CombatProgressionApplicationResult> ProgressionApplied;
    public event Action<CombatProgressionDecayResult> ProgressionDecayed;

    [SerializeField] private CombatProgressionController controller;

    private bool subscribed;

    public float CurrentScore
    {
        get
        {
            if (controller == null)
                return 0f;

            return controller.CurrentScore;
        }
    }

    public CombatRankGrade CurrentRank
    {
        get
        {
            if (controller == null)
                return CombatRankGrade.D;

            return controller.CurrentRank;
        }
    }

    public CombatRankGrade PreviousRank
    {
        get
        {
            CombatProgressionStateSnapshot snapshot = CreateSnapshot();
            return snapshot.PreviousRank;
        }
    }

    public float CurrentMultiplier
    {
        get
        {
            if (controller == null)
                return 1f;

            return controller.CurrentMultiplier;
        }
    }

    public float RankProgress01
    {
        get
        {
            CombatProgressionStateSnapshot snapshot = CreateSnapshot();
            return snapshot.RankProgress01;
        }
    }

    public bool IsCombatActive
    {
        get
        {
            if (controller == null)
                return false;

            return controller.IsCombatActive;
        }
    }

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Initialize()
    {
        if (controller == null)
            ResolveReferences();

        if (controller == null)
            return;

        controller.Initialize();
    }

    public void ResetProgression()
    {
        if (controller == null)
            ResolveReferences();

        if (controller == null)
            return;

        controller.ResetProgression();
    }

    public void BeginCombat()
    {
        if (controller == null)
            ResolveReferences();

        if (controller == null)
            return;

        controller.BeginCombat();
    }

    public void EndCombat()
    {
        if (controller == null)
            ResolveReferences();

        if (controller == null)
            return;

        controller.EndCombat();
    }

    public void SetCombatActive(bool active)
    {
        if (controller == null)
            ResolveReferences();

        if (controller == null)
            return;

        controller.SetCombatActive(active);
    }

    public CombatProgressionStateSnapshot CreateSnapshot()
    {
        if (controller == null)
            ResolveReferences();

        if (controller == null)
        {
            return new CombatProgressionStateSnapshot(
                0f,
                CombatRankGrade.D,
                CombatRankGrade.D,
                1f,
                0f,
                false,
                0f,
                0f,
                0f);
        }

        return controller.CreateSnapshot();
    }

    public bool IsAtRank(CombatRankGrade rank)
    {
        return CurrentRank == rank;
    }

    public bool IsAtLeastRank(CombatRankGrade rank)
    {
        return CurrentRank >= rank;
    }

    public bool TryGetLatestScoreResult(out CombatProgressionScoreResult result)
    {
        result = default;

        if (controller == null)
            ResolveReferences();

        if (controller == null || controller.Runtime == null)
            return false;

        return controller.Runtime.TryGetLatestResult(out result);
    }

    public bool TryGetRecentScoreResult(int indexFromLatest, out CombatProgressionScoreResult result)
    {
        result = default;

        if (controller == null)
            ResolveReferences();

        if (controller == null || controller.Runtime == null)
            return false;

        return controller.Runtime.TryGetRecentResult(indexFromLatest, out result);
    }

    private void ResolveReferences()
    {
        if (controller == null)
            controller = GetComponent<CombatProgressionController>();

        if (controller == null)
            controller = GetComponentInParent<CombatProgressionController>();
    }

    private void Subscribe()
    {
        if (subscribed)
            return;

        ResolveReferences();

        if (controller == null)
            return;

        controller.StateChanged += HandleStateChanged;
        controller.RankChanged += HandleRankChanged;
        controller.ProgressionApplied += HandleProgressionApplied;
        controller.ProgressionDecayed += HandleProgressionDecayed;

        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed)
            return;

        if (controller != null)
        {
            controller.StateChanged -= HandleStateChanged;
            controller.RankChanged -= HandleRankChanged;
            controller.ProgressionApplied -= HandleProgressionApplied;
            controller.ProgressionDecayed -= HandleProgressionDecayed;
        }

        subscribed = false;
    }

    private void HandleStateChanged(CombatProgressionStateSnapshot snapshot)
    {
        StateChanged?.Invoke(snapshot);
    }

    private void HandleRankChanged(CombatProgressionRankStabilityResult result)
    {
        RankChanged?.Invoke(result);
    }

    private void HandleProgressionApplied(CombatProgressionApplicationResult result)
    {
        ProgressionApplied?.Invoke(result);
    }

    private void HandleProgressionDecayed(CombatProgressionDecayResult result)
    {
        ProgressionDecayed?.Invoke(result);
    }

    private void Reset()
    {
        ResolveReferences();
    }
}