using System;
using System.Collections.Generic;

public sealed class AilmentController
{
    private readonly Dictionary<AilmentDefinition, RuntimeAilment> ailments = new();
    private readonly Dictionary<AilmentDefinition, float> resistances = new();
    private readonly List<RuntimeAilment> activeAilments = new();

    private readonly GameplayEffectController effectController;

    public bool RequiresTick => activeAilments.Count > 0;

    public event Action<RuntimeAilment> AilmentTriggered;
    public event Action<RuntimeAilment> BuildupCleared;

    public AilmentController(GameplayEffectController effectController, IReadOnlyList<AilmentResistance> resistances)
    {
        this.effectController = effectController;

        if (resistances == null)
            return;

        for (int i = 0; i < resistances.Count; i++)
        {
            AilmentResistance resistance = resistances[i];

            if (resistance == null || resistance.Ailment == null)
                continue;

            this.resistances[resistance.Ailment] = resistance.Resistance;
        }
    }

    public void Tick(float deltaTime)
    {
        for (int i = activeAilments.Count - 1; i >= 0; i--)
            activeAilments[i].Tick(deltaTime);
    }

    public RuntimeAilment GetAilment(AilmentDefinition definition)
    {
        if (definition == null)
            return null;

        if (ailments.TryGetValue(definition, out RuntimeAilment ailment))
            return ailment;

        ailment = new RuntimeAilment(definition);
        ailment.Triggered += HandleAilmentTriggered;
        ailment.BuildupCleared += HandleBuildupCleared;

        ailments.Add(definition, ailment);

        return ailment;
    }

    public void ApplyAilment(AilmentApplication application)
    {
        if (application.Ailment == null || application.Buildup <= 0f)
            return;

        RuntimeAilment ailment = GetAilment(application.Ailment);

        if (ailment == null)
            return;

        float buildup = application.Buildup;

        if (resistances.TryGetValue(application.Ailment, out float resistance))
            buildup *= 1f - resistance;

        if (buildup <= 0f)
            return;

        if (!activeAilments.Contains(ailment))
            activeAilments.Add(ailment);

        ailment.AddBuildup(buildup);
    }

    private void HandleAilmentTriggered(RuntimeAilment ailment)
    {
        activeAilments.Remove(ailment);

        EffectDefinition effect = ailment.Definition.TriggerEffect;

        if (effect != null)
            effectController.ApplyEffect(effect);

        AilmentTriggered?.Invoke(ailment);

        ailment.Reset();
    }

    private void HandleBuildupCleared(RuntimeAilment ailment)
    {
        activeAilments.Remove(ailment);
        BuildupCleared?.Invoke(ailment);
    }
}