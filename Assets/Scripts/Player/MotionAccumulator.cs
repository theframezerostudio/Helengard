using UnityEngine;

public class MotionAccumulator
{
    Vector3 rootDelta;
    Vector3 extraDelta;
    Quaternion rootRotation = Quaternion.identity;

    public void AddRootDelta(Vector3 delta)
    {
        rootDelta += delta;
    }

    public void AddRootRotation(Quaternion delta)
    {
        rootRotation = delta * rootRotation;
    }

    public void AddExtraDelta(Vector3 delta)
    {
        extraDelta += delta;
    }

    public void Consume(MovementMotionPolicy motionPolicy, RotationMotionPolicy rotationPolicy, Transform characterTransform, 
                        out Vector3 position, out Quaternion rotation)
    {
        position = FilterRootMotion(rootDelta, motionPolicy, characterTransform) + extraDelta;
        rotation = FilterRootRotation(rootRotation, rotationPolicy);
        Reset();
    }

    void Reset()
    {
        rootDelta = Vector3.zero;
        extraDelta = Vector3.zero;
        rootRotation = Quaternion.identity;
    }

    Vector3 FilterRootMotion(Vector3 delta, MovementMotionPolicy policy, Transform t)
    {
        switch (policy)
        {
            case MovementMotionPolicy.FullRootMotion:
                return delta;

            case MovementMotionPolicy.Root_XZ_MotionOnly:
                delta.y = 0;
                return delta;

            case MovementMotionPolicy.RootForwardOnly:
                return Vector3.Project(delta, t.forward);

            case MovementMotionPolicy.NoRootMotion:
                return Vector3.zero;
        }

        return delta;
    }

    Quaternion FilterRootRotation(Quaternion delta, RotationMotionPolicy policy)
    {
        switch (policy)
        {
            case RotationMotionPolicy.FullRootRotation:
                return delta;

            case RotationMotionPolicy.YawOnly:
                Vector3 euler = delta.eulerAngles;
                return Quaternion.Euler(0, euler.y, 0);

            case RotationMotionPolicy.NoRotation:
                return Quaternion.identity;
        }

        return delta;
    }
}
