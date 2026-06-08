using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DirectionStunAdjustment
{
    public HitDirection direction;

    [Tooltip("Seconds added to the attack's base stun duration.")]
    public float additiveSeconds;
}

[Serializable]
public struct HeightStunAdjustment
{
    public HitHeight height;

    [Tooltip("Seconds added to the attack's base stun duration.")]
    public float additiveSeconds;
}

[Serializable]
public struct StaggerContextOverride
{
    public HitDirection direction;
    public HitHeight height;

    [Header("Optional Swing Filter")]
    public bool filterSwing;
    public SwingType swingType;

    [Header("Timing")]
    [Tooltip("Additional seconds added after direction and height adjustments.")]
    public float additionalSeconds;

    [Header("Impulse Override")]
    public bool overrideImpulseMultiplier;

    [Min(0f)]
    public float impulseMultiplier;

    [Header("Cancel Window Override")]
    public bool overrideCancelNormalizedTime;

    [Range(0f, 1f)]
    public float cancelNormalizedTime;
}

public readonly struct ResolvedStaggerReaction
{
    public readonly float Duration;
    public readonly float ImpulseMultiplier;
    public readonly float CancelNormalizedTime;
    public readonly string AnimationState;

    public ResolvedStaggerReaction(
        float duration,
        float impulseMultiplier,
        float cancelNormalizedTime,
        string animationState)
    {
        Duration = duration;
        ImpulseMultiplier = impulseMultiplier;
        CancelNormalizedTime = cancelNormalizedTime;
        AnimationState = animationState;
    }
}

[CreateAssetMenu(
    fileName = "Stagger Reaction Profile",
    menuName = "Combat/Reactions/Stagger Reaction Profile")]
public sealed class StaggerReactionProfile : ScriptableObject
{
    [Header("Timing")]
    [SerializeField, Min(0f)] private float fallbackBaseStun = 0.2f;
    [SerializeField, Min(0f)] private float minimumStun = 0.08f;
    [SerializeField, Min(0f)] private float maximumStun = 0.45f;

    [Header("Animation")]
    [SerializeField] private string staggerAnimationState = "Hit_Blend";

    [Header("Default Motion")]
    [SerializeField, Min(0f)] private float defaultImpulseMultiplier = 1f;

    [Header("Cancel Window")]
    [SerializeField, Range(0f, 1f)] private float defaultCancelNormalizedTime = 0.7f;

    [Header("Context Timing")]
    [SerializeField] private List<DirectionStunAdjustment> directionAdjustments = new();
    [SerializeField] private List<HeightStunAdjustment> heightAdjustments = new();

    [Header("Specific Overrides")]
    [SerializeField] private List<StaggerContextOverride> overrides = new();

    public ResolvedStaggerReaction Resolve(DamageEvent hit)
    {
        float baseDuration = hit.BaseStunDuration > 0f
            ? hit.BaseStunDuration
            : fallbackBaseStun;

        float duration = baseDuration;
        duration += GetDirectionAdjustment(hit.Direction);
        duration += GetHeightAdjustment(hit.Height);

        float impulseMultiplier = defaultImpulseMultiplier;
        float cancelNormalizedTime = defaultCancelNormalizedTime;

        if (TryFindOverride(hit, out StaggerContextOverride contextOverride))
        {
            duration += contextOverride.additionalSeconds;

            if (contextOverride.overrideImpulseMultiplier)
                impulseMultiplier = contextOverride.impulseMultiplier;

            if (contextOverride.overrideCancelNormalizedTime)
                cancelNormalizedTime = contextOverride.cancelNormalizedTime;
        }

        duration = Mathf.Clamp(duration, minimumStun, maximumStun);
        cancelNormalizedTime = Mathf.Clamp01(cancelNormalizedTime);

        return new ResolvedStaggerReaction(
            duration,
            impulseMultiplier,
            cancelNormalizedTime,
            staggerAnimationState);
    }

    private float GetDirectionAdjustment(HitDirection direction)
    {
        foreach (DirectionStunAdjustment adjustment in directionAdjustments)
        {
            if (adjustment.direction == direction)
                return adjustment.additiveSeconds;
        }

        return 0f;
    }

    private float GetHeightAdjustment(HitHeight height)
    {
        foreach (HeightStunAdjustment adjustment in heightAdjustments)
        {
            if (adjustment.height == height)
                return adjustment.additiveSeconds;
        }

        return 0f;
    }

    private bool TryFindOverride(DamageEvent hit, out StaggerContextOverride resolvedOverride)
    {
        foreach (StaggerContextOverride contextOverride in overrides)
        {
            if (contextOverride.direction != hit.Direction)
                continue;

            if (contextOverride.height != hit.Height)
                continue;

            if (contextOverride.filterSwing &&
                contextOverride.swingType != hit.SwingType)
            {
                continue;
            }

            resolvedOverride = contextOverride;
            return true;
        }

        resolvedOverride = default;
        return false;
    }

    private void OnValidate()
    {
        if (maximumStun < minimumStun)
            maximumStun = minimumStun;
    }
}