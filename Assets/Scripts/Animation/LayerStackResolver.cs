using System.Collections.Generic;
using UnityEngine;

public class LayerStackResolver
{
    private List<LayerState> layers;

    public LayerStackResolver(List<LayerState> layers)
    {
        this.layers = layers;
    }

    public void Resolve(float dt)
    {
        // Sort by priority (highest first)
        layers.Sort((a, b) => b.priority.CompareTo(a.priority));

        float remaining = 1f;

        for (int i = 0; i < layers.Count; i++)
        {
            LayerState layer = layers[i];

            float target = layer.intent * layer.influence;

            // Clamp to remaining space
            float applied = Mathf.Min(target, remaining);

            layer.UpdateWeight(applied, dt);

            remaining -= layer.weight;

            if (remaining <= 0f)
                break;
        }
    }
}