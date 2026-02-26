using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public ComboGraph comboGraph;
    public Hitbox[] hitboxes;

    public Action<DamageEvent> OnHit;
    public FeedbackPlayer feedbackPlayer;

    public AttackResolver AttackResolver {  get; private set; }

    private void Awake()
    {
        AttackResolver = new AttackResolver(comboGraph);
    }

    private void Start()
    {
        foreach (var hitbox in hitboxes)
        {
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

    public void StartAttack(ComboNode node)
    {
        foreach (var hitbox in hitboxes)
        {
            hitbox.InitiateHit(node.attackProfile);
        }
    }

    public void EndAttack()
    {
        foreach (var hitbox in hitboxes)
        {
            hitbox.TerminateHit();
        }
    }

    private void HandleHit(HitData data)
    {
        data.target.TakeDamage(data.damageEvent);
        OnHit?.Invoke(data.damageEvent);

        if (feedbackPlayer != null)
            feedbackPlayer.Play();
    }

    private void OnDestroy()
    {
        foreach (var hitbox in hitboxes)
        {
            hitbox.OnHit -= HandleHit;
        }
    }
}