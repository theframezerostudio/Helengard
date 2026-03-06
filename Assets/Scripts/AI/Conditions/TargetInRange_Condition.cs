using UnityEngine;

public class TargetInRange_Condition : Condition
{
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float detectionRange;

    /// <summary>
    /// If target was already in range,
    /// it must exceed detectionRange + threshold to become false
    /// </summary>
    [Tooltip("If target was already in range, it must exceed detectionRange + threshold to return false")]
    [SerializeField] private float threshold;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private bool visualizeThreshold;

    private Collider[] colliders;
    private bool wasInRange = false;
    private Target currentTarget;

    private void Start()
    {
        colliders = new Collider[5];
    }

    public override void Initialize(Character owner, AICombatData combatData)
    {
        base.Initialize(owner, combatData);

        wasInRange = false;
        currentTarget = null;
    }

    public override bool Evaluate()
    {
        Vector3 center = transform.position + offset;

        if (!wasInRange)
        {
            int count = Physics.OverlapSphereNonAlloc(
                center,
                detectionRange,
                colliders,
                detectionLayer
            );

            if (count > 0)
            {
                foreach (Collider collider in colliders)
                {
                    if (collider == null)
                        continue;

                    if (collider.TryGetComponent<Target>(out var target))
                    {
                        currentTarget = target;
                        break;
                    }
                }

                if (currentTarget == null)
                    return false;

                CombatData.Target = currentTarget;
                wasInRange = true;
                return true;
            }

            return false;
        }

        if (currentTarget == null)
        {
            wasInRange = false;
            return false;
        }

        float distance = Vector3.Distance(center, currentTarget.transform.position);

        if (distance > detectionRange + threshold)
        {
            wasInRange = false;
            currentTarget = null;
            return false;
        }

        CombatData.Target = currentTarget;
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position + offset, detectionRange);

        if (!visualizeThreshold)
            return;

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position + offset, detectionRange + threshold);
    }
}