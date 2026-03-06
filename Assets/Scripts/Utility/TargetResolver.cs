using UnityEngine;

public class TargetResolver
{
    private static Collider[] targetColliders = new Collider[10];
    private static Target[] targets = new Target[10];

    public static Transform ResolveTarget(Player player, Vector2 moveInput, float checkRadius, LayerMask layerMask)
    {
        // TODO: Get Possible Targets
        int colCount = Physics.OverlapSphereNonAlloc(player.transform.position,
                                      checkRadius,
                                      targetColliders,
                                      layerMask);

        int count = 0;

        DebugDrawSphere(player.transform.position, checkRadius);

        for (int i = 0; i < colCount; i++)
        {
            Collider col = targetColliders[i];
            Target target = col.GetComponentInParent<Target>();

            if (target != null)
            {
                targets[count++] = target;
            }
        }

        Debug.Log(layerMask.ToString() + " " + colCount + " " + count);

        Transform bestMoveTarget = null;
        Transform bestFrontTarget = null;
        Transform attackingTarget = null;
        Transform bestLeftTarget = null;
        Transform bestRightTarget = null;

        float bestMoveDot = 0.6f;
        float bestFrontDot = 0.75f;
        float bestLeftDot = -1f;
        float bestRightDot = -1f;

        for (int i = 0; i < count; i++)
        {
            Target target = targets[i];
            Vector3 toTarget = target.transform.position - player.transform.position;
            toTarget.y = 0;

            float distance = toTarget.magnitude;
            Vector3 dir = toTarget.normalized;

            float dotForward = Vector3.Dot(player.transform.forward, dir);

            // Priority 1 — Stick Direction
            if (moveInput.sqrMagnitude > 0.1f)
            {
                Vector3 inputDir = player.LocomotionMode.GetDirection(moveInput);
                float dotInput = Vector3.Dot(inputDir, dir);

                if (dotInput > bestMoveDot)
                {
                    bestMoveDot = dotInput;
                    bestMoveTarget = target.transform;
                }
            }

            // Priority 2 — In Front
            if (dotForward > bestFrontDot)
            {
                bestFrontDot = dotForward;
                bestFrontTarget = target.transform;
            }
            float side = Vector3.Cross(player.transform.forward, dir).y;

            // Priority 3 - In Left
            if (side < 0)
            {
                if (dotForward > bestLeftDot)
                {
                    bestLeftDot = dotForward;
                    bestLeftTarget = target.transform;
                }
            }

            // Priority 4 — In Right
            else
            {
                if (dotForward > bestRightDot)
                {
                    bestRightDot = dotForward;
                    bestRightTarget = target.transform;
                }
            }

            // Priority 5 — Attacking Player
            //if (Check if any target is attacking player)
            //{
            //    attackingTarget = target.transform;
            //}
        }

        //Debug.Log(bestMoveTarget + " " + bestFrontTarget + " " + attackingTarget + " " + ); 
        if (bestMoveTarget) return bestMoveTarget;
        if (bestFrontTarget) return bestFrontTarget;
        if (bestLeftTarget) return bestLeftTarget;
        if (bestRightTarget) return bestRightTarget;
        if (attackingTarget) return attackingTarget;

        return null;
    }

    static void DebugDrawSphere(Vector3 center, float radius)
    {
        int segments = 24;

        DrawCircle(center, Vector3.up, radius, segments);
        DrawCircle(center, Vector3.right, radius, segments);
        DrawCircle(center, Vector3.forward, radius, segments);
    }

    static void DrawCircle(Vector3 center, Vector3 normal, float radius, int segments)
    {
        Vector3 axisA = Vector3.Cross(normal, Vector3.up);

        if (axisA == Vector3.zero)
            axisA = Vector3.Cross(normal, Vector3.right);

        axisA.Normalize();
        Vector3 axisB = Vector3.Cross(normal, axisA);

        float angleStep = 360f / segments;

        Vector3 prevPoint = center + axisA * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 nextPoint =
                center +
                (axisA * Mathf.Cos(angle) + axisB * Mathf.Sin(angle)) * radius;

            Debug.DrawLine(prevPoint, nextPoint, Color.yellow, 1f);

            prevPoint = nextPoint;
        }
    }
}