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
        layers.Sort((a, b) => b.priority.CompareTo(a.priority));

        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].targetWeight = 0f;
        }
        float remaining = 1f;

        // PASS 1 — Solve target weights (instant, no smoothing)
        for (int i = 0; i < layers.Count; i++)
        {
            LayerState layer = layers[i];

            float target = Mathf.Clamp01(layer.intent * layer.influence);
            float allowed = Mathf.Min(target, remaining);

            layer.targetWeight = allowed;   // store target
            remaining -= allowed;

            if (remaining <= 0f)
                break;
        }

        // Remaining layers target = 0
        for (int i = 0; i < layers.Count; i++)
        {
            if (layers[i].targetWeight <= 0f)
                layers[i].targetWeight = 0f;
        }

        // PASS 2 — Smooth towards target
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].UpdateWeight(layers[i].targetWeight, dt);
        }
    }
}