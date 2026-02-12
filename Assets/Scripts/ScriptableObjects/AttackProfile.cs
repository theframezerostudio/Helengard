using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AttackProfile")]
public class AttackProfile : ScriptableObject
{
    public float damage = 10f;
    public LayerMask hurtboxMask;

    public HitEffectType effect = HitEffectType.Light;
    public HitSwing swingType = HitSwing.Stab;

    public Vector3 hitForce;
    public float stunDuration = 0.25f;
    public float hitStop = 0.06f;
    public bool canChain = true;
    public float staggerValue = 1f;
}