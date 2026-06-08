using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Hitbox : MonoBehaviour
{
    [SerializeField] private Vector3 boxHalfExtents = Vector3.one;
    [SerializeField] private Vector3 boxOffset = Vector3.forward;
    [SerializeField] private bool debugDraw;

    private readonly HashSet<IDamageable> targetsHit = new();

    private CharacterAttributes owner;
    private Transform attackerRoot;
    private AttackProfile profile;
    private float powerMultiplier = 1f;
    private Coroutine attackRoutine;

    public event Action<HitData> OnHit;

    public void Initialize(CharacterAttributes owner)
    {
        this.owner = owner;
        attackerRoot = owner != null ? owner.transform.root : transform.root;
    }

    public void InitiateHit(AttackProfile attackProfile, float powerMultiplier = 1f)
    {
        if (attackProfile == null)
            return;

        TerminateHit();

        profile = attackProfile;
        this.powerMultiplier = powerMultiplier;

        attackRoutine = StartCoroutine(AttackSequence());
    }

    public void TerminateHit()
    {
        targetsHit.Clear();
        profile = null;
        powerMultiplier = 1f;

        if (attackRoutine == null)
            return;

        StopCoroutine(attackRoutine);
        attackRoutine = null;
    }

    private IEnumerator AttackSequence()
    {
        float safetyTime = 5f;

        while (safetyTime > 0f)
        {
            FireHit();
            safetyTime -= Time.deltaTime;
            yield return null;
        }

        TerminateHit();
    }

    public void FireHit()
    {
        if (profile == null || attackerRoot == null)
            return;

        Vector3 center = transform.position + transform.TransformDirection(boxOffset);

        Collider[] hits = Physics.OverlapBox(
            center,
            boxHalfExtents,
            transform.rotation,
            profile.hurtboxMask);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hitCollider = hits[i];
            IDamageable target = hitCollider.GetComponentInParent<IDamageable>();

            if (target == null || target.Attributes == null)
                continue;

            if (!target.IsAlive)
                continue;

            if (owner != null && target.Attributes == owner)
                continue;

            if (!targetsHit.Add(target))
                continue;

            Vector3 hitPoint = hitCollider.ClosestPoint(center);
            Vector3 hitNormal = (hitPoint - center).normalized;
            Vector3 hitForce = attackerRoot.TransformDirection(profile.hitForce);

            Transform targetRoot = hitCollider.transform.root;

            HitDirection direction = HitUtilities.ComputeHitDirection(targetRoot, attackerRoot, hitPoint);
            HitHeight height = HitUtilities.ComputeHitHeight(targetRoot, hitPoint);

            OnHit?.Invoke(new HitData(
                target,
                profile,
                owner.transform,
                hitCollider.transform,
                hitPoint,
                hitNormal,
                hitForce,
                direction,
                height,
                powerMultiplier));
        }

        if (debugDraw)
            DebugDrawBox(center, boxHalfExtents, transform.rotation, Color.red, 1f);
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw)
            return;

        Gizmos.color = Color.blue;

        Vector3 center = transform.position + transform.TransformDirection(boxOffset);
        Matrix4x4 previousMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);

        Gizmos.matrix = previousMatrix;
    }

    private void DebugDrawBox(Vector3 center, Vector3 halfExtents, Quaternion rotation, Color color, float duration)
    {
        Vector3[] corners = new Vector3[8];

        corners[0] = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
        corners[1] = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
        corners[2] = new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
        corners[3] = new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
        corners[4] = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
        corners[5] = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
        corners[6] = new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
        corners[7] = new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);

        for (int i = 0; i < corners.Length; i++)
            corners[i] = center + rotation * corners[i];

        Debug.DrawLine(corners[0], corners[1], color, duration);
        Debug.DrawLine(corners[1], corners[2], color, duration);
        Debug.DrawLine(corners[2], corners[3], color, duration);
        Debug.DrawLine(corners[3], corners[0], color, duration);

        Debug.DrawLine(corners[4], corners[5], color, duration);
        Debug.DrawLine(corners[5], corners[6], color, duration);
        Debug.DrawLine(corners[6], corners[7], color, duration);
        Debug.DrawLine(corners[7], corners[4], color, duration);

        Debug.DrawLine(corners[0], corners[4], color, duration);
        Debug.DrawLine(corners[1], corners[5], color, duration);
        Debug.DrawLine(corners[2], corners[6], color, duration);
        Debug.DrawLine(corners[3], corners[7], color, duration);
    }
}