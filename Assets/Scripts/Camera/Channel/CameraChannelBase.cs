using UnityEngine;

public abstract class CameraChannelBase<T> : ICameraChannel
{
    public abstract CameraChannel Id { get; }

    protected abstract T GetBase(CameraModeDefinition mode);
    protected abstract T GetModifierValue(CameraModifierDefinition modifier);
    protected abstract T Combine(T baseValue, T modifierValue, float weight);

    protected abstract void Apply(ref CameraResolvedState state, T value);

    public void Resolve(CameraModeDefinition mode, CameraModifierRegistry registry, ref CameraResolvedState state)
    {
        var best = GetBestModifier(mode, registry);
        if (best == null)
            return;

        float modeWeight = mode.GetWeightForModifier(best.Definition);
        float intensity = best.GetWeight(Id);
        float finalWeight = Mathf.Clamp01(modeWeight * intensity);

        T baseValue = GetBase(mode);
        T modValue = GetModifierValue(best.Definition);

        T result = Combine(baseValue, modValue, finalWeight);
        Apply(ref state, result);
    }

    private ActiveCameraModifier GetBestModifier(CameraModeDefinition mode, CameraModifierRegistry registry)
    {
        ActiveCameraModifier best = null;
        int bestPriority = int.MinValue;

        foreach (var modifier in registry.ActiveModifiers)
        {
            if (modifier == null || !modifier.IsActive || modifier.Definition == null)
                continue;

            if (!modifier.Definition.Affects(Id))
                continue;

            float weight = mode.GetWeightForModifier(modifier.Definition);
            if (weight <= 0f)
                continue;

            int priority = modifier.Definition.Priority;
            if (priority > bestPriority)
            {
                bestPriority = priority;
                best = modifier;
            }
        }

        return best;
    }
}