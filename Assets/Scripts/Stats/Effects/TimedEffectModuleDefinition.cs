using UnityEngine;

public abstract class TimedEffectModuleDefinition : EffectModuleDefinition
{
    [SerializeField]
    private float interval = 1f;

    public float Interval => interval;
}