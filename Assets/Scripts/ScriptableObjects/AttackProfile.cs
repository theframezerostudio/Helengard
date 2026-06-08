using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Attack Profile")]
public sealed class AttackProfile : ScriptableObject
{
    [Header("Interactions")]
    public InteractionDefinition activationInteraction;
    public InteractionDefinition impactInteraction;

    [Header("Hit Detection")]
    public LayerMask hurtboxMask;

    [Header("Reaction Routing")]
    [Tooltip("Identity-only reaction asset used by the target ReactionController.")]
    public ReactionKey expectedReaction;

    [Header("Hit Response")]
    public HitImpactType hitImpact = HitImpactType.Light;
    public SwingType swingType = SwingType.Stab;

    [Tooltip("Authored attack force before runtime hit resolution.")]
    public Vector3 hitForce;

    [Tooltip("Base gameplay stun duration. Stagger reactions may apply small contextual adjustments.")]
    [Min(0f)]
    public float stunDuration = 0.25f;

    [Tooltip("Brief impact freeze, separate from gameplay stun.")]
    [Min(0f)]
    public float hitStop = 0.06f;

    [Tooltip("Whether another compatible reaction can refresh or replace this reaction.")]
    public bool canChain = true;

    [Tooltip("Posture or stagger pressure applied by this attack.")]
    [Min(0f)]
    public float staggerValue = 1f;
}