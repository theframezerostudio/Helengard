using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    public sealed class AilmentController
    {
        private readonly Dictionary<AilmentDefinition, RuntimeAilment> ailments = new();
        private readonly Dictionary<AilmentDefinition, float> resistances = new();
        private readonly GameplayEffectController effectController;

        public event Action<RuntimeAilment> AilmentTriggered;
        public event Action<RuntimeAilment> AilmentRecovered;

        public AilmentController(
            GameplayEffectController effectController,
            IReadOnlyList<AilmentResistance> resistances)
        {
            this.effectController = effectController;

            for (int i = 0; i < resistances.Count; i++)
            {
                AilmentResistance resistance = resistances[i];
                this.resistances[resistance.Ailment] = resistance.Resistance;
            }
        }

        public void Tick(float deltaTime)
        {
            if (ailments.Count == 0)
                return;
            
            foreach (RuntimeAilment ailment in ailments.Values)
            {
                AilmentState previous = ailment.State;

                ailment.Tick(deltaTime);

                if (previous != AilmentState.Triggered &&
                    ailment.State == AilmentState.Triggered)
                {
                    TriggerAilment(ailment);
                }

                if (previous != AilmentState.Inactive &&
                    ailment.State == AilmentState.Inactive)
                {
                    AilmentRecovered?.Invoke(ailment);
                }
            }
        }

        public RuntimeAilment GetAilment(AilmentDefinition definition)
        {
            if (ailments.TryGetValue(definition, out RuntimeAilment ailment))
            {
                return ailment;
            }

            ailment = new RuntimeAilment(definition);
            ailments.Add(definition, ailment);

            return ailment;
        }

        public void ApplyAilment(AilmentApplication application)
        {
            RuntimeAilment ailment = GetAilment(application.Ailment);
            float buildup = application.Buildup;

            if (resistances.TryGetValue(application.Ailment, out float resistance))
            {
                buildup *= 1f - resistance;
            }

            ailment.AddBuildup(buildup);
        }

        private void TriggerAilment(RuntimeAilment ailment)
        {
            EffectDefinition effect = ailment.Definition.TriggerEffect;

            if (effect != null)
            {
                effectController.ApplyEffect(effect);
            }

            AilmentTriggered?.Invoke(ailment);
            ailment.Reset();
        }
    }
}