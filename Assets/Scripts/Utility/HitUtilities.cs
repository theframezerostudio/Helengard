using UnityEngine;

public static class HitUtilities
{
    public static HitDirection ComputeHitDirection(Transform target, Transform attacker, Vector3 hitPoint)
    {
        Vector3 worldDir = attacker.position - target.position;

        Vector3 localDir = target.InverseTransformDirection(worldDir.normalized);

        float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        if (Mathf.Abs(angle) < 45f) return HitDirection.Front;
        if (Mathf.Abs(angle) > 135f) return HitDirection.Back;
        return angle > 0 ? HitDirection.Right : HitDirection.Left;
    }


    public static HitHeight ComputeHitHeight(Transform target, Vector3 hitPoint)
    {
        var localHit = target.InverseTransformPoint(hitPoint);
        float normalizedY = localHit.y / target.localScale.y;
        if (normalizedY > 0.6f) return HitHeight.High;
        if (normalizedY < 0.3f) return HitHeight.Low;
        return HitHeight.Mid;
    }
}
