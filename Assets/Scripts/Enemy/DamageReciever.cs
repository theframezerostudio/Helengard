using UnityEngine;
using System;

public class DamageReciever : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f;
    [SerializeField] private ReactionController reactionController;
    
    public Action onDamageRecieved;

    public bool IsAlive => health > 0;

    public void TakeDamage(DamageEvent ev)
    {
        if (!IsAlive) return;
        if (!CanBeHit(ev)) return;

        health -= ev.Damage;
        onDamageRecieved?.Invoke();

        if (reactionController)
            reactionController.OnDamageReceived(ev);
    }

    bool CanBeHit(DamageEvent ev)
    {
        return true;
    }
}
