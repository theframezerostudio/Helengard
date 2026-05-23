using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Attack Profile")]
public sealed class AttackProfile : ScriptableObject
{
    [Header("Interactions")]
    public InteractionDefinition activationInteraction;
    public InteractionDefinition impactInteraction;

    [Header("Hit Detection")]
    public LayerMask hurtboxMask;

    [Header("Hit Response")]
    public HitImpactType hitImpact = HitImpactType.Light;
    public SwingType swingType = SwingType.Stab;

    public Vector3 hitForce;
    public float stunDuration = 0.25f;
    public float hitStop = 0.06f;
    public bool canChain = true;
    public float staggerValue = 1f;
}