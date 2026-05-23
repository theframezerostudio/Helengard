public interface IDamageable
{
    CharacterAttributes Attributes { get; }
    bool IsAlive { get; }

    void TakeDamage(DamageEvent damageEvent);
}