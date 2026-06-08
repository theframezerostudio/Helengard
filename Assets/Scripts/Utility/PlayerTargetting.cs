using System;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private LayerMask targetMask;

    [Header("Soft Target")]
    [SerializeField] private float softTargetRange = 6f;
    [SerializeField] private bool updateSoftTargetWhileLocked = true;

    [Header("Lock Target")]
    [SerializeField] private float lockRange = 14f;
    [SerializeField] private float lockBreakRange = 17f;

    [Header("Debug")]
    [SerializeField] private bool drawDebug;

    public event Action<Target, Target> SoftTargetChanged;
    public event Action<Target, Target> LockedTargetChanged;
    public event Action<Target, Target> ActiveTargetChanged;

    [field: SerializeField, ReadOnly] public Target SoftTarget { get; private set; }
    [field: SerializeField, ReadOnly] public Target LockedTarget { get; private set; }

    [SerializeField] private int targetBufferSize = 32;

    private TargetResolver resolver;

    public Target ActiveTarget
    {
        get
        {
            if (LockedTarget != null)
                return LockedTarget;

            return SoftTarget;
        }
    }

    public Transform ActiveTransform
    {
        get
        {
            Target activeTarget = ActiveTarget;

            if (activeTarget == null)
                return null;

            return activeTarget.transform;
        }
    }

    public bool HasLock => LockedTarget != null;

    private void Awake()
    {
        resolver = new TargetResolver(targetBufferSize);

        if (player == null)
            player = GetComponent<Player>();
    }

    private void Start()
    {
        InputManager.Instance.onMove += Tick;
    }

    public void Tick(Vector2 moveInput)
    {
        if (player == null)
            return;

        Target previousActiveTarget = ActiveTarget;

        if (LockedTarget != null && !IsTargetValidForRange(LockedTarget, lockBreakRange))
            SetLockedTargetInternal(null);

        if (LockedTarget == null || updateSoftTargetWhileLocked)
        {
            Target nextSoftTarget = resolver.ResolveSoftTarget(
                player,
                moveInput,
                softTargetRange,
                targetMask,
                drawDebug
            );

            SetSoftTargetInternal(nextSoftTarget);
        }

        RaiseActiveTargetChangedIfNeeded(previousActiveTarget);
    }

    public bool TryLockOn(Vector2 moveInput)
    {
        if (player == null)
            return false;

        Target previousActiveTarget = ActiveTarget;

        Target nextLockTarget = SoftTarget;

        if (!IsTargetValidForRange(nextLockTarget, lockRange))
        {
            nextLockTarget = resolver.ResolveLockTarget(
                player,
                moveInput,
                lockRange,
                targetMask,
                drawDebug
            );
        }

        SetLockedTargetInternal(nextLockTarget);
        RaiseActiveTargetChangedIfNeeded(previousActiveTarget);

        return nextLockTarget != null;
    }

    public void ClearLock()
    {
        Target previousActiveTarget = ActiveTarget;

        SetLockedTargetInternal(null);
        RaiseActiveTargetChangedIfNeeded(previousActiveTarget);
    }

    public void ToggleLock(Vector2 moveInput)
    {
        if (LockedTarget != null)
        {
            ClearLock();
            return;
        }

        TryLockOn(moveInput);
    }

    public void CycleLockRight()
    {
        CycleLock(1);
    }

    public void CycleLockLeft()
    {
        CycleLock(-1);
    }

    public void CycleLock(int direction)
    {
        if (player == null)
            return;

        if (LockedTarget == null)
        {
            TryLockOn(Vector2.zero);
            return;
        }

        Target previousActiveTarget = ActiveTarget;

        Target nextTarget = resolver.ResolveCycleTarget(
            player,
            LockedTarget,
            direction,
            lockRange,
            targetMask,
            drawDebug
        );

        if (nextTarget != null)
            SetLockedTargetInternal(nextTarget);

        RaiseActiveTargetChangedIfNeeded(previousActiveTarget);
    }

    public Target GetAttackTarget(ComboNode node, Vector2 moveInput)
    {
        if (node == null)
            return ActiveTarget;

        if (LockedTarget != null && IsTargetValidForRange(LockedTarget, lockBreakRange))
            return LockedTarget;

        float attackAssistRange = Mathf.Max(node.attackRange, softTargetRange);

        return resolver.ResolveSoftTarget(
            player,
            moveInput,
            attackAssistRange,
            targetMask,
            drawDebug
        );
    }

    public void SetSoftTargetRange(float value)
    {
        softTargetRange = Mathf.Max(0f, value);
    }

    public void SetLockRange(float value)
    {
        lockRange = Mathf.Max(0f, value);

        if (lockBreakRange < lockRange)
            lockBreakRange = lockRange;
    }

    public void SetLockBreakRange(float value)
    {
        lockBreakRange = Mathf.Max(lockRange, value);
    }

    private void SetSoftTargetInternal(Target nextTarget)
    {
        if (SoftTarget == nextTarget)
            return;

        Target previousTarget = SoftTarget;
        SoftTarget = nextTarget;

        SoftTargetChanged?.Invoke(previousTarget, nextTarget);
    }

    private void SetLockedTargetInternal(Target nextTarget)
    {
        if (LockedTarget == nextTarget)
            return;

        Target previousTarget = LockedTarget;
        LockedTarget = nextTarget;

        LockedTargetChanged?.Invoke(previousTarget, nextTarget);
    }

    private void RaiseActiveTargetChangedIfNeeded(Target previousActiveTarget)
    {
        Target currentActiveTarget = ActiveTarget;

        if (previousActiveTarget == currentActiveTarget)
            return;

        ActiveTargetChanged?.Invoke(previousActiveTarget, currentActiveTarget);
    }

    private bool IsTargetValidForRange(Target target, float range)
    {
        if (target == null)
            return false;

        if (!target.gameObject.activeInHierarchy)
            return false;

        Vector3 toTarget = target.transform.position - transform.position;
        toTarget.y = 0f;

        return toTarget.sqrMagnitude <= range * range;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.onMove -= Tick;
    }
}