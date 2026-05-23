using System;
using UnityEngine;

public sealed class Target : MonoBehaviour, IDamageable
{
    [Tooltip("Character Context of Character, if applicable")]
    [field: SerializeField] public CharacterContext Context { get; private set; }

    [field: SerializeField] public CharacterAttributes Attributes { get; private set; }

    [SerializeField] private ResourceDefinition healthResource;

    public bool IsAlive
    {
        get
        {
            if (Attributes == null || healthResource == null)
                return true;

            Resource health = Attributes.Resources.GetResource(healthResource);

            return health != null && !health.IsDepleted;
        }
    }

    public event Action<DamageEvent> OnHit;

    private DamageEvent recentHit;
    private float lastHitTime;

    [SerializeField] private float hitMemoryDuration = 0.5f;

    public void TakeDamage(DamageEvent damageEvent)
    {
        if (damageEvent == null)
            return;

        RegisterHit(damageEvent);
        OnHit?.Invoke(damageEvent);

        if (Context != null)
            Context.dataAggregator.MarkAsTargetted();
    }

    public void RegisterHit(DamageEvent damageEvent)
    {
        recentHit = damageEvent;
        lastHitTime = Time.time;
    }

    public DamageEvent GetRecentHit()
    {
        if (Time.time - lastHitTime <= hitMemoryDuration)
            return recentHit;

        return null;
    }
}