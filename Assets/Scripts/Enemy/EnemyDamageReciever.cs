using UnityEngine;
using System.Collections;

public class EnemyDamageReceiver : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private ReactionController reactionController;

    public bool IsAlive => health > 0;

    public void TakeDamage(DamageEvent ev)
    {
        if (!IsAlive) return;
        if (!CanBeHit(ev)) return;

        health -= ev.Damage;

        reactionController.OnDamageReceived(ev);
    }

    bool CanBeHit(DamageEvent ev)
    {
        return true;
    }
}
