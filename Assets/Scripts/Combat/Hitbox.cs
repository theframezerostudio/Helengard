using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitData
{
    public IDamageable target;
    public DamageEvent damageEvent;

    public HitData(IDamageable target, DamageEvent damageEvent)
    {
        this.target = target;
        this.damageEvent = damageEvent;
    }
}

public class Hitbox : MonoBehaviour
{
    public AttackProfile profile;
    public Transform attackerRoot;

    public LayerMask hurtboxMask;
    public Vector3 boxHalfExtents = Vector3.one;
    public Vector3 boxOffset = Vector3.forward;

    public bool debugDraw = false;
    private HashSet<IDamageable> damageables = new();
    private Coroutine attackRoutine;

    public Action<HitData> OnHit;

    public void InitiateHit(AttackProfile attackProfile)
    {
        profile = attackProfile;

        TerminateHit();

        attackRoutine = StartCoroutine(AttackSequence());
    }

    public void TerminateHit()
    {
        damageables.Clear();
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    IEnumerator AttackSequence()
    {
        float safetyTime = 5f;

        while (safetyTime > 0)
        {
            FireHit();
            safetyTime -= Time.deltaTime;
            yield return null;
        }

        damageables.Clear();
    }

    public void FireHit()
    {
        Vector3 center = transform.position + transform.TransformDirection(boxOffset);
        Collider[] hits = Physics.OverlapBox(center, boxHalfExtents, transform.rotation, hurtboxMask);
        foreach (var col in hits)
        {
            var damageable = col.GetComponentInParent<IDamageable>();

            if (damageable == null) continue;
            if (damageables.Contains(damageable)) continue;

            damageables.Add(damageable);

            // compute hit point & normal
            Vector3 hitPoint = col.ClosestPoint(center);
            Vector3 hitNormal = (hitPoint - center).normalized;

            // compute direction/height relative to target
            Transform targetRoot = col.transform.root;
            HitDirection dir = HitUtilities.ComputeHitDirection(targetRoot, attackerRoot, hitPoint);
            HitHeight height = HitUtilities.ComputeHitHeight(targetRoot, hitPoint);

            try
            {
                DamageEvent damageEvent = new(profile.damage, profile.effect, hitPoint, hitNormal,
                    attackerRoot.TransformDirection(profile.hitForce), attackerRoot, col.transform, dir, height, profile.swingType,
                    profile.canChain, profile.stunDuration, profile.hitStop);

                OnHit?.Invoke(new HitData(damageable, damageEvent));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log((profile == null) + " " + col == null);
            }
            //damageable.TakeDamage(damageEvent);
        }

        if (debugDraw)
        {
            DebugDrawBox(center, boxHalfExtents, transform.rotation, Color.red, 1.0f);
        }
    }

    void OnDrawGizmos()
    {
        if (debugDraw) return;

        Gizmos.color = Color.blue;

        Vector3 center = transform.position + transform.TransformDirection(boxOffset);
        Quaternion rotation = transform.rotation;

        // Save matrix
        Matrix4x4 oldMatrix = Gizmos.matrix;

        // Draw rotated box
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);

        // Restore matrix
        Gizmos.matrix = oldMatrix;
    }

    void DebugDrawBox(Vector3 center, Vector3 halfExtents, Quaternion rotation, Color color, float duration)
    {
        Vector3[] corners = new Vector3[8];

        // Local corner positions
        Vector3 ext = halfExtents;
        corners[0] = new Vector3(-ext.x, -ext.y, -ext.z);
        corners[1] = new Vector3(ext.x, -ext.y, -ext.z);
        corners[2] = new Vector3(ext.x, -ext.y, ext.z);
        corners[3] = new Vector3(-ext.x, -ext.y, ext.z);

        corners[4] = new Vector3(-ext.x, ext.y, -ext.z);
        corners[5] = new Vector3(ext.x, ext.y, -ext.z);
        corners[6] = new Vector3(ext.x, ext.y, ext.z);
        corners[7] = new Vector3(-ext.x, ext.y, ext.z);

        // Transform corners to world space
        for (int i = 0; i < 8; i++)
        {
            corners[i] = center + rotation * corners[i];
        }

        // Bottom
        Debug.DrawLine(corners[0], corners[1], color, duration);
        Debug.DrawLine(corners[1], corners[2], color, duration);
        Debug.DrawLine(corners[2], corners[3], color, duration);
        Debug.DrawLine(corners[3], corners[0], color, duration);

        // Top
        Debug.DrawLine(corners[4], corners[5], color, duration);
        Debug.DrawLine(corners[5], corners[6], color, duration);
        Debug.DrawLine(corners[6], corners[7], color, duration);
        Debug.DrawLine(corners[7], corners[4], color, duration);

        // Sides
        Debug.DrawLine(corners[0], corners[4], color, duration);
        Debug.DrawLine(corners[1], corners[5], color, duration);
        Debug.DrawLine(corners[2], corners[6], color, duration);
        Debug.DrawLine(corners[3], corners[7], color, duration);
    }
}
