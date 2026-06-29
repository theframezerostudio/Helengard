using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Spell", menuName = "Create Conjurations/Projectile Conjurations")]
public class ProjectileCastingProperties : CastingProperties
{
    [field: SerializeField] public float projectileSpeed { get; private set; }

    [field: SerializeField] public float effectRadius { get; private set; }

    [field: SerializeField] public LayerMask groundMask { get; private set; }
}
