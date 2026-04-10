using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraModifierRegistry : MonoBehaviour
{
    private readonly List<ActiveCameraModifier> activeModifiers = new();

    public event Action ModifiersChanged;

    public IReadOnlyList<ActiveCameraModifier> ActiveModifiers => activeModifiers;

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = activeModifiers.Count - 1; i >= 0; i--)
        {
            var modifier = activeModifiers[i];
            if (modifier == null)
                continue;

            modifier.Tick(dt);

            if (!modifier.IsActive)
                activeModifiers.RemoveAt(i);
        }
    }

    public ActiveCameraModifier AddModifier(CameraModifierDefinition definition, object source, float weight = 1f)
    {
        if (definition == null)
            return null;

        var existing = activeModifiers.FirstOrDefault(m =>
            m != null &&
            m.IsActive &&
            m.Definition == definition &&
            Equals(m.Source, source));

        if (existing != null)
        {
            existing.SetTargetWeight(weight);
            ModifiersChanged?.Invoke();
            return existing;
        }

        var modifier = new ActiveCameraModifier(definition, source, weight);
        activeModifiers.Add(modifier);
        ModifiersChanged?.Invoke();
        return modifier;
    }

    public void RemoveModifier(CameraModifierDefinition definition, object source)
    {
        if (definition == null)
            return;

        var modifier = activeModifiers.FirstOrDefault(m =>
            m != null &&
            m.IsActive &&
            m.Definition == definition &&
            Equals(m.Source, source));

        if (modifier == null)
            return;

        modifier.BeginRemoval();
        ModifiersChanged?.Invoke();
    }

    public void RemoveAllFromSource(object source)
    {
        bool changed = false;

        foreach (var modifier in activeModifiers)
        {
            if (modifier != null && modifier.IsActive && Equals(modifier.Source, source))
            {
                modifier.BeginRemoval();
                changed = true;
            }
        }

        if (changed)
            ModifiersChanged?.Invoke();
    }

    public void ClearAll()
    {
        foreach (var modifier in activeModifiers)
            modifier?.BeginRemoval();

        activeModifiers.Clear();
        ModifiersChanged?.Invoke();
    }
}