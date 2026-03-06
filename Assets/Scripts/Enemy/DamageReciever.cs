using UnityEngine;
using System;

[Obsolete("Damage Reciever is deprecated. Use Target and ReactionController instead.")]
public class DamageReciever : MonoBehaviour
{
    //[SerializeField] private float health = 100f;
    //[SerializeField] private ReactionController reactionController;
    
    //public Action<DamageEvent> onDamageRecieved;

    //public bool IsAlive => health > 0;

    //public void TakeDamage(DamageEvent ev)
    //{
    //    if (!IsAlive) return;
    //    if (!CanBeHit(ev)) return;

    //    health -= ev.Damage;
    //    onDamageRecieved?.Invoke(ev);

    //    //if (reactionController)
    //    //    reactionController.OnDamageReceived(ev);
    //}

    //bool CanBeHit(DamageEvent ev)
    //{
    //    return true;
    //}
}
