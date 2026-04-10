using System.Collections.Generic;
using UnityEngine;

public sealed class ActiveCameraModifier
{
    public CameraModifierDefinition Definition { get; }
    public object Source { get; }
    public bool IsActive { get; private set; }

    private readonly Dictionary<CameraChannel, ChannelBlendState> channelStates = new();

    public ActiveCameraModifier(CameraModifierDefinition definition, object source, float initialWeight = 1f)
    {
        Definition = definition;
        Source = source;
        IsActive = true;

        InitializeChannels(Mathf.Clamp01(initialWeight));
    }

    public void SetTargetWeight(float weight)
    {
        if (Definition == null)
            return;

        float clamped = Mathf.Clamp01(weight);

        foreach (var kvp in channelStates)
        {
            CameraChannel channel = kvp.Key;
            var state = kvp.Value;

            state.BeginBlendIn(
                clamped,
                Definition.BlendProfile.GetBlendInTime(channel),
                Definition.BlendProfile.GetBlendInCurve(channel)
            );
        }

        IsActive = true;
    }

    public void BeginRemoval()
    {
        if (Definition == null)
            return;

        foreach (var kvp in channelStates)
        {
            CameraChannel channel = kvp.Key;
            var state = kvp.Value;

            state.BeginBlendOut(
                Definition.BlendProfile.GetBlendOutTime(channel),
                Definition.BlendProfile.GetBlendOutCurve(channel)
            );
        }
    }

    public void Tick(float deltaTime)
    {
        if (Definition == null)
            return;

        bool anyAlive = false;

        foreach (var kvp in channelStates)
        {
            kvp.Value.Tick(deltaTime);

            if (kvp.Value.IsAlive)
                anyAlive = true;
        }

        IsActive = anyAlive;
    }

    public float GetWeight(CameraChannel channel)
    {
        return channelStates.TryGetValue(channel, out var state)
            ? state.CurrentWeight
            : 0f;
    }

    private void InitializeChannels(float initialWeight)
    {
        foreach (CameraChannel channel in System.Enum.GetValues(typeof(CameraChannel)))
        {
            if (channel == CameraChannel.None)
                continue;

            if (!Definition.Affects(channel))
                continue;

            var state = new ChannelBlendState();
            state.BeginBlendIn(
                initialWeight,
                Definition.BlendProfile.GetBlendInTime(channel),
                Definition.BlendProfile.GetBlendInCurve(channel)
            );

            channelStates[channel] = state;
        }
    }

    private sealed class ChannelBlendState
    {
        public float CurrentWeight { get; private set; }
        public bool IsAlive { get; private set; }

        private float startWeight;
        private float targetWeight;
        private float timer;
        private float duration;
        private bool blendingIn;
        private AnimationCurve curve;

        public void BeginBlendIn(float target, float blendTime, AnimationCurve blendCurve)
        {
            blendingIn = true;
            IsAlive = true;

            startWeight = CurrentWeight;
            targetWeight = Mathf.Clamp01(target);
            timer = 0f;
            duration = Mathf.Max(0f, blendTime);
            curve = blendCurve;
        }

        public void BeginBlendOut(float blendTime, AnimationCurve blendCurve)
        {
            blendingIn = false;
            IsAlive = true;

            startWeight = CurrentWeight;
            targetWeight = 0f;
            timer = 0f;
            duration = Mathf.Max(0f, blendTime);
            curve = blendCurve;
        }

        public void Tick(float deltaTime)
        {
            if (!IsAlive)
                return;

            if (duration <= 0f)
            {
                CurrentWeight = targetWeight;
                IsAlive = blendingIn || CurrentWeight > 0f;
                return;
            }

            timer += deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float easedT = curve != null ? curve.Evaluate(t) : t;

            CurrentWeight = Mathf.Lerp(startWeight, targetWeight, easedT);

            if (t >= 1f)
            {
                CurrentWeight = targetWeight;
                if (!blendingIn && Mathf.Approximately(CurrentWeight, 0f))
                    IsAlive = false;
            }
        }
    }
}