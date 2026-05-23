using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Weapon : MonoBehaviour
{
    [SerializeField] private CharacterAttributes owner;

    [Header("Combo")]
    [SerializeField] private ComboGraph comboGraph;

    [Header("Hitboxes")]
    [SerializeField] private Hitbox[] hitboxes;

    [Header("Feedback")]
    [SerializeField] private FeedbackPlayer feedbackPlayer;

    private readonly HashSet<IDamageable> targetsHitThisWindow = new();
    private AttackExecutor attackExecutor;

    public ComboGraph ComboGraph => comboGraph;

    public Action<DamageEvent> OnHit;

    public AttackResolver AttackResolver { get; private set; }

    private void Awake()
    {
        AttackResolver = new AttackResolver(comboGraph);
    }

    private void Start()
    {
        Initialize(owner);
    }

    private void OnDestroy()
    {
        UnsubscribeFromHitboxes();
    }

    public void Initialize(CharacterAttributes owner)
    {
        this.owner = owner;

        attackExecutor = new AttackExecutor(owner, gameObject, this);

        for (int i = 0; i < hitboxes.Length; i++)
        {
            Hitbox hitbox = hitboxes[i];

            if (hitbox == null)
                continue;

            hitbox.Initialize(owner);
            hitbox.OnHit -= HandleHit;
            hitbox.OnHit += HandleHit;
        }
    }

    public ComboNode InitiateAttack(CharacterContext context, AttackInput attackInput)
    {
        return AttackResolver.GetEntryNode(context, attackInput);
    }

    public ComboNode NextAttack(CharacterContext context, AttackInput attackInput, ComboNode node)
    {
        return AttackResolver.Resolve(context, attackInput, node);
    }

    public bool TryCommitAttack(ComboNode node, float powerMultiplier = 1f)
    {
        if (node == null || node.attackProfile == null || attackExecutor == null)
            return false;

        return attackExecutor.TryCommitAttack(node.attackProfile, powerMultiplier);
    }

    public void StartAttack(ComboNode node, float powerMultiplier = 1f)
    {
        if (node == null || node.attackProfile == null)
            return;

        targetsHitThisWindow.Clear();

        for (int i = 0; i < hitboxes.Length; i++)
        {
            Hitbox hitbox = hitboxes[i];

            if (hitbox != null)
                hitbox.InitiateHit(node.attackProfile, powerMultiplier);
        }
    }

    public void EndAttack()
    {
        for (int i = 0; i < hitboxes.Length; i++)
        {
            Hitbox hitbox = hitboxes[i];

            if (hitbox != null)
                hitbox.TerminateHit();
        }

        targetsHitThisWindow.Clear();
    }

    private void HandleHit(HitData hit)
    {
        if (hit.target == null || attackExecutor == null)
        {
            Debug.LogWarning(hit.target == null ? "Hit target is null." : "Attack executor is null.");
            return;
        }

        if (!targetsHitThisWindow.Add(hit.target))
            return;

        if (!attackExecutor.TryResolveHit(hit, out DamageEvent damageEvent))
            return;

        hit.target.TakeDamage(damageEvent);

        OnHit?.Invoke(damageEvent);

        HandleFeedback(damageEvent);
    }

    private void HandleFeedback(DamageEvent damageEvent)
    {
        // Use damageEvent.Result when FeedbackPlayer integration is added.
    }

    private void UnsubscribeFromHitboxes()
    {
        if (hitboxes == null)
            return;

        for (int i = 0; i < hitboxes.Length; i++)
        {
            Hitbox hitbox = hitboxes[i];

            if (hitbox != null)
                hitbox.OnHit -= HandleHit;
        }
    }
}