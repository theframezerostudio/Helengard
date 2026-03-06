using System;
using UnityEngine;

public class Target : MonoBehaviour, IDamageable
{
    [Tooltip("Character Context of Character, if applicable")]
    [field: SerializeField] public CharacterContext Context { get; private set; }

    // TODO: Update isAlive logic to be based on health or other conditions as needed
    public bool IsAlive => true;

    public event Action<DamageEvent> onHit;

    private DamageEvent damageEvent = null;
    private float lastHitTime;
    private readonly float hitMemoryDuration = 0.5f;

    public void TakeDamage(DamageEvent damageEvent)
    {
        onHit?.Invoke(damageEvent);
        RegisterHit(damageEvent);
        Context.dataAggregator.MarkAsTargetted();
    }

    public void RegisterHit(DamageEvent ev)
    {
        damageEvent = ev;
        lastHitTime = Time.time;
    }

    public DamageEvent GetRecentHit()
    {
        if (Time.time - lastHitTime <= hitMemoryDuration)
            return damageEvent;

        return null;
    }
}
