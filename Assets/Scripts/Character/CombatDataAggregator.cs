using UnityEngine;
using System;

[Serializable]
public struct CombatSnapshot
{
    public int version;

    // TODO: Replace hp, stamona with with proper Stat refernce
    public float hp;
    public float stamina;

    public Vector3 position;
    public Vector3 velocity;
    public bool isAttacking;
    public bool isDefending;
    public float timeInAttackState;
}
public class CombatDataAggregator
{
    private CombatSnapshot snapshot;

    [Header("Thresholds")]
    private readonly float positionThreshold = 0.01f;
    private readonly float velocityThreshold = 0.01f;

    private Vector3 lastPos;
    private Vector3 lastVel;
    private float lastHp = -9999f;

    public event Action<CombatSnapshot> OnSnapshotChanged;

    public CombatSnapshot Snapshot => snapshot;

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

    public void SetPosition(Vector3 pos)
    {
        if ((pos - lastPos).sqrMagnitude < positionThreshold * positionThreshold) return;
        lastPos = pos;
        snapshot.position = pos;
        MarkDirty();
    }

    public void SetVelocity(Vector3 vel)
    {
        if ((vel - lastVel).sqrMagnitude < velocityThreshold * velocityThreshold) return;
        lastVel = vel;
        snapshot.velocity = vel;
        MarkDirty();
    }

    public void SetAttacking(bool active, float time)
    {
        snapshot.isAttacking = active;
        snapshot.timeInAttackState = time;
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