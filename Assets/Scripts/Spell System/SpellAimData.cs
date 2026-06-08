using UnityEngine;

public struct SpellAimData
{
    public Target Target { get; private set; }
    public Vector3 Origin { get; private set; }
    public Vector3 AimPoint { get; private set; }
    public Vector3 Direction { get; private set; }
    public readonly bool HasTarget => Target != null;
    public bool IsManual { get; private set; }
    public bool IsValid { get; private set; }

    public SpellAimData(Target target, Vector3 origin, Vector3 aimPoint, Vector3 direction, bool isManual, bool isValid)
    {
        Target = target;
        Origin = origin;
        AimPoint = aimPoint;
        Direction = direction;
        IsManual = isManual;
        IsValid = isValid;
    }
}