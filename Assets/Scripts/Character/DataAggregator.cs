using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct CombatSnapshot
{
    public int version;

    // TODO: Replace hp, stamona with with proper Stat refernce
    public float hp;
    public float stamina;

    public Vector3 position;
    public Vector3 forward;
    public Vector3 velocity;

    public bool isGettingTargeted;
    public bool isAttacking;
    public bool isLightAttacking;
    public bool isHeavyAttacking;
    public bool isDefending;
    public bool isInRecovery;

    public float lastTimeAttacked;
    public float timeInAttackState;
}

public class DataAggregator
{
    private const float GRACE_PERIOD = 1f; // Time in seconds to consider data "fresh" after last update

    private CombatSnapshot snapshot;

    [Header("Thresholds")]
    private readonly float positionThreshold = 0.01f;
    private readonly float velocityThreshold = 0.01f;

    private Vector3 lastPos;
    private Vector3 forward;
    private Vector3 lastVel;

    private float lastHp = -9999f;
    private float attackStartTime;

    public event Action<CombatSnapshot> OnSnapshotChanged;
    private Coroutine attackCoroutine;
    private Coroutine targetCoroutine;
    private WaitForSeconds graceWait = new WaitForSeconds(GRACE_PERIOD);

    public CombatSnapshot Snapshot
    {
        get
        {
            snapshot.timeInAttackState = snapshot.isAttacking ? Time.time - attackStartTime : 0f;
            return snapshot;
        }
    }

    public void SetHP(float hp)
    {
        if (Mathf.Abs(lastHp - hp) < 0.01f) return;
        lastHp = hp;
        snapshot.hp = hp;
        MarkDirty();
    }
    public void SetStamina(float stamina)
    {
        snapshot.stamina = stamina;
        MarkDirty();
    }

    public void SetPosition(Vector3 pos, Vector3 forward)
    {
        if ((pos - lastPos).sqrMagnitude < positionThreshold * positionThreshold) return;
        lastPos = pos;
        snapshot.position = pos;
        snapshot.forward = forward;
        MarkDirty();
    }

    public void SetVelocity(Vector3 vel)
    {
        if ((vel - lastVel).sqrMagnitude < velocityThreshold * velocityThreshold) return;
        lastVel = vel;
        snapshot.velocity = vel;
        MarkDirty();
    }

    public void SetInRecovery(bool inRecovery)
    {
        snapshot.isInRecovery = inRecovery;
        MarkDirty();
    }

    public void MarkAsTargetted()
    {
        if (targetCoroutine != null)
        {
            CoroutineManager.Stop(targetCoroutine);
            targetCoroutine = null;
        }

        snapshot.isGettingTargeted = true;
        targetCoroutine = CoroutineManager.Run(ResetTargetRoutine());
        MarkDirty();
    }

    public void SetAttacking(bool active, bool isLight = true)
    {
        if (active)
        {
            StartAttack(isLight);
        }
        else
        {
            ScheduleAttackReset();
        }
    }

    private void StartAttack(bool isLight)
    {
        if (attackCoroutine != null)
        {
            CoroutineManager.Stop(attackCoroutine);
            attackCoroutine = null;
        }

        if (snapshot.isAttacking)
            return;

        snapshot.isAttacking = true;

        snapshot.isLightAttacking = isLight;
        snapshot.isHeavyAttacking = !isLight;

        attackStartTime = Time.time;
        MarkDirty();
    }

    private void ScheduleAttackReset()
    {
        if (attackCoroutine != null)
            return;

        attackCoroutine = CoroutineManager.Run(ResetAttackSnapshot());
    }

    private IEnumerator ResetAttackSnapshot()
    {
        yield return graceWait;

        snapshot.lastTimeAttacked = Time.time;
        snapshot.isAttacking = false;

        snapshot.isLightAttacking = false;
        snapshot.isHeavyAttacking = false;

        attackStartTime = 0f;
        attackCoroutine = null;

        MarkDirty();
    }

    private IEnumerator ResetTargetRoutine()
    {
        yield return graceWait;
        snapshot.isGettingTargeted = false;
        targetCoroutine = null;
        MarkDirty();
    }

    public void SetDefending(bool active)
    {
        snapshot.isDefending = active;
        MarkDirty();
    }

    private void MarkDirty()
    {
        unchecked { snapshot.version++; }
        OnSnapshotChanged?.Invoke(snapshot);
    }
}