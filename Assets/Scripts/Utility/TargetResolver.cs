using UnityEngine;

public sealed class TargetResolver
{
    private readonly Collider[] targetColliders;
    private readonly Target[] targets;

    public TargetResolver(int capacity)
    {
        targetColliders = new Collider[capacity];
        targets = new Target[capacity];
    }

    public Target ResolveSoftTarget(Player player, Vector2 moveInput, float checkRadius, LayerMask layerMask, bool drawDebug = false)
    {
        if (player == null)
            return null;

        int count = CollectTargets(player.transform.position, checkRadius, layerMask);

        if (drawDebug)
            DebugDrawSphere(player.transform.position, checkRadius, Color.yellow);

        Transform playerTransform = player.transform;

        Target bestMoveTarget = null;
        Target bestFrontTarget = null;
        Target bestSideTarget = null;

        float bestMoveScore = 0.55f;
        float bestFrontScore = 0.35f;
        float bestSideScore = -999f;

        bool hasMoveInput = moveInput.sqrMagnitude > 0.1f;
        Vector3 inputDir = Vector3.zero;

        if (hasMoveInput)
        {
            inputDir = player.LocomotionMode.GetDirection(moveInput);
            inputDir.y = 0f;

            if (inputDir.sqrMagnitude > 0.0001f)
                inputDir.Normalize();
            else
                hasMoveInput = false;
        }

        for (int i = 0; i < count; i++)
        {
            Target target = targets[i];

            if (!IsUsableTarget(target))
                continue;

            Vector3 toTarget = target.transform.position - playerTransform.position;
            toTarget.y = 0f;

            float sqrDistance = toTarget.sqrMagnitude;

            if (sqrDistance <= 0.0001f)
                continue;

            float distance = Mathf.Sqrt(sqrDistance);
            float distance01 = Mathf.Clamp01(distance / checkRadius);

            Vector3 dir = toTarget / distance;

            float dotForward = Vector3.Dot(playerTransform.forward, dir);

            if (hasMoveInput)
            {
                float dotInput = Vector3.Dot(inputDir, dir);
                float moveScore = dotInput - distance01 * 0.15f;

                if (dotInput > 0.5f && moveScore > bestMoveScore)
                {
                    bestMoveScore = moveScore;
                    bestMoveTarget = target;
                }
            }

            float frontScore = dotForward - distance01 * 0.25f;

            if (dotForward > 0.35f && frontScore > bestFrontScore)
            {
                bestFrontScore = frontScore;
                bestFrontTarget = target;
            }

            float sideAmount = Mathf.Abs(Vector3.Cross(playerTransform.forward, dir).y);
            float sideScore = sideAmount * 0.35f + dotForward * 0.25f - distance01 * 0.25f;

            if (sideScore > bestSideScore)
            {
                bestSideScore = sideScore;
                bestSideTarget = target;
            }
        }

        if (bestMoveTarget != null) return bestMoveTarget;
        if (bestFrontTarget != null) return bestFrontTarget;
        if (bestSideTarget != null) return bestSideTarget;

        return null;
    }

    public Target ResolveLockTarget(Player player, Vector2 moveInput, float lockRange, LayerMask layerMask, bool drawDebug = false)
    {
        if (player == null)
            return null;

        int count = CollectTargets(player.transform.position, lockRange, layerMask);

        if (drawDebug)
            DebugDrawSphere(player.transform.position, lockRange, Color.cyan);

        Transform playerTransform = player.transform;

        bool hasMoveInput = moveInput.sqrMagnitude > 0.1f;
        Vector3 inputDir = Vector3.zero;

        if (hasMoveInput)
        {
            inputDir = player.LocomotionMode.GetDirection(moveInput);
            inputDir.y = 0f;

            if (inputDir.sqrMagnitude > 0.0001f)
                inputDir.Normalize();
            else
                hasMoveInput = false;
        }

        Target bestTarget = null;
        float bestScore = -999f;

        for (int i = 0; i < count; i++)
        {
            Target target = targets[i];

            if (!IsUsableTarget(target))
                continue;

            Vector3 toTarget = target.transform.position - playerTransform.position;
            toTarget.y = 0f;

            float sqrDistance = toTarget.sqrMagnitude;

            if (sqrDistance <= 0.0001f)
                continue;

            float distance = Mathf.Sqrt(sqrDistance);
            float distance01 = Mathf.Clamp01(distance / lockRange);

            Vector3 dir = toTarget / distance;

            float dotForward = Vector3.Dot(playerTransform.forward, dir);
            float dotInput = hasMoveInput ? Vector3.Dot(inputDir, dir) : 0f;

            bool acceptable = dotForward > -0.15f || dotInput > 0.5f;

            if (!acceptable)
                continue;

            float score = dotForward * 1.35f;
            score += (1f - distance01) * 0.65f;

            if (hasMoveInput)
                score += dotInput * 0.75f;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = target;
            }
        }

        return bestTarget;
    }

    public Target ResolveCycleTarget(Player player, Target currentTarget, int direction, float lockRange, LayerMask layerMask, bool drawDebug = false)
    {
        if (player == null)
            return null;

        int count = CollectTargets(player.transform.position, lockRange, layerMask);

        if (drawDebug)
            DebugDrawSphere(player.transform.position, lockRange, Color.cyan);

        if (direction == 0)
            direction = 1;

        direction = direction > 0 ? 1 : -1;

        Vector3 origin = player.transform.position;

        Vector3 baseDir;

        if (currentTarget != null)
        {
            baseDir = currentTarget.transform.position - origin;
            baseDir.y = 0f;
        }
        else
        {
            baseDir = player.transform.forward;
        }

        if (baseDir.sqrMagnitude <= 0.0001f)
            baseDir = player.transform.forward;

        baseDir.Normalize();

        Target bestSideTarget = null;
        Target fallbackTarget = null;

        float bestSideAngle = 999f;
        float bestFallbackDistance = 999f;

        for (int i = 0; i < count; i++)
        {
            Target target = targets[i];

            if (!IsUsableTarget(target))
                continue;

            if (target == currentTarget)
                continue;

            Vector3 toTarget = target.transform.position - origin;
            toTarget.y = 0f;

            float sqrDistance = toTarget.sqrMagnitude;

            if (sqrDistance <= 0.0001f)
                continue;

            float distance = Mathf.Sqrt(sqrDistance);
            Vector3 dir = toTarget / distance;

            float signedAngle = Vector3.SignedAngle(baseDir, dir, Vector3.up);

            if (direction > 0 && signedAngle > 5f)
            {
                if (signedAngle < bestSideAngle)
                {
                    bestSideAngle = signedAngle;
                    bestSideTarget = target;
                }
            }
            else if (direction < 0 && signedAngle < -5f)
            {
                float absAngle = Mathf.Abs(signedAngle);

                if (absAngle < bestSideAngle)
                {
                    bestSideAngle = absAngle;
                    bestSideTarget = target;
                }
            }

            if (distance < bestFallbackDistance)
            {
                bestFallbackDistance = distance;
                fallbackTarget = target;
            }
        }

        if (bestSideTarget != null)
            return bestSideTarget;

        return fallbackTarget;
    }

    private int CollectTargets(Vector3 origin, float radius, LayerMask layerMask)
    {
        int colCount = Physics.OverlapSphereNonAlloc(origin, radius, targetColliders, layerMask);

        int count = 0;

        for (int i = 0; i < colCount; i++)
        {
            Collider col = targetColliders[i];

            if (col == null)
                continue;

            Target target = col.GetComponentInParent<Target>();

            if (target == null)
                continue;

            if (ContainsTarget(target, count))
                continue;

            if (count >= targets.Length)
                break;

            targets[count] = target;
            count++;
        }

        return count;
    }

    private bool ContainsTarget(Target target, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (targets[i] == target)
                return true;
        }

        return false;
    }

    private static bool IsUsableTarget(Target target)
    {
        if (target == null)
            return false;

        if (!target.gameObject.activeInHierarchy)
            return false;

        return true;
    }

    private static void DebugDrawSphere(Vector3 center, float radius, Color color)
    {
        int segments = 24;

        DrawCircle(center, Vector3.up, radius, segments, color);
        DrawCircle(center, Vector3.right, radius, segments, color);
        DrawCircle(center, Vector3.forward, radius, segments, color);
    }

    private static void DrawCircle(Vector3 center, Vector3 normal, float radius, int segments, Color color)
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

            Debug.DrawLine(prevPoint, nextPoint, color, 0f);

            prevPoint = nextPoint;
        }
    }
}