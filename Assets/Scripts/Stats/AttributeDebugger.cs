using System;
using UnityEngine;

public class AttributeDebugger : MonoBehaviour
{
    [SerializeField] private CharacterAttributes attributes;

    [SerializeField] private StatDefinition[] statDefinitions;
    [SerializeField] private ResourceDefinition[] resourceDefinitions;
    [SerializeField] private AilmentDefinition[] ailmentDefinitions;

    private void Start()
    {
        SubscribeResources();
        SubscribeAilments();
    }

    private void SubscribeResources()
    {
        foreach (ResourceDefinition definition in resourceDefinitions)
        {
            if (attributes.Resources.HasResource(definition))
            {
                attributes.Resources.GetResource(definition).ValueChanged += HandleResourceChange;
            }
            else
            {
                Debug.LogWarning($"Stat '{definition.DisplayName}' not found.");
            }
        }
    }

    private void SubscribeAilments()
    {
        foreach (AilmentDefinition definition in ailmentDefinitions)
        {
            attributes.Ailments.AilmentTriggered += HandleAilmentChange;
            //attributes.Ailments.BuildupCleared += HandleAilmentChange;
        }
    }

    private void HandleAilmentChange(RuntimeAilment ailment)
    {
        Debug.Log($"<color=cyan>Ailment Triggered:</color> {ailment.Definition.DisplayName} " +
            $"is now <color=red>{ailment.State}</color>");
    }

    private void HandleResourceChange(Resource resource, float oldvalue, float newValue)
    {
        Debug.Log($"<color=cyan>Resource Changed:</color> {resource.Definition.DisplayName} " +
            $"changed from <color=red>{oldvalue}</color> " +
            $"to <color=green>{newValue}</color>");
    }

    private void HandleStatChange(RuntimeStat stat, float oldValue, float newValue)
    {
        Debug.Log($"<color=cyan>Stat Changed:</color> {stat.Definition.DisplayName} " +
            $"changed from <color=red>{oldValue}</color> " +
            $"to <color=green>{newValue}</color>");
    }

    [ContextMenu("Debug Attributes")]
    private void DebugAttributes()
    {
        foreach (ResourceDefinition definition in resourceDefinitions)
        {
            if (attributes.Resources.HasResource(definition))
            {
                Resource resource = attributes.Resources.GetResource(definition);
                Debug.Log($"<color=yellow>Resource:</color> {resource.Definition.DisplayName} " +
                    $"has current value <color=green>{resource.CurrentValue}</color> " +
                    $"and max value <color=blue>{resource.MaxValue}</color>");
            }
            else
            {
                Debug.LogWarning($"Stat '{definition.DisplayName}' not found.");
            }
        }
    }

    public static void Log(string message)
    {
        Debug.Log($"<color=magenta>[AttributeDebugger]</color> {message}");
    }

    private void OnDestroy()
    {
        UnsubscribeResources();
        UnsubscribeAilments();
    }

    private void UnsubscribeAilments()
    {
        foreach (AilmentDefinition definition in ailmentDefinitions)
        {
            attributes.Ailments.AilmentTriggered -= HandleAilmentChange;
            //attributes.Ailments.BuildupCleared -= HandleAilmentChange;
        }
    }

    private void UnsubscribeResources()
    {
        foreach (ResourceDefinition definition in resourceDefinitions)
        {
            if (attributes.Resources.HasResource(definition))
            {
                attributes.Resources.GetResource(definition).ValueChanged -= HandleResourceChange;
            }
        }
    }
} 