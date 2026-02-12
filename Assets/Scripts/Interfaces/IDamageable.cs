public interface IDamageable
{
    void TakeDamage(DamageEvent damageEvent);
    bool IsAlive { get; }
}
