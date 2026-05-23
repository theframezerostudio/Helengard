using System.Collections.Generic;

public sealed class ResourceContainer
{
    private readonly Dictionary<ResourceDefinition, Resource> resources = new();

    public ResourceContainer(IReadOnlyList<ResourceDefinition> definitions, StatContainer stats)
    {
        for (int i = 0; i < definitions.Count; i++)
        {
            CreateResource(definitions[i], stats);
        }
    }

    public bool HasResource(ResourceDefinition definition)
    {
        return resources.ContainsKey(definition);
    }

    public Resource GetResource(ResourceDefinition definition)
    {
        resources.TryGetValue(definition, out Resource resource);

        return resource;
    }

    public float GetValue(ResourceDefinition definition, float fallback = 0f)
    {
        Resource resource = GetResource(definition);

        return resource?.CurrentValue ?? fallback;
    }

    public bool TryConsume(ResourceDefinition definition, float amount)
    {
        Resource resource = GetResource(definition);

        if (resource == null)
            return false;

        if (resource.CurrentValue < amount)
            return false;

        resource.Consume(amount);

        return true;
    }

    public void Restore(ResourceDefinition definition, float amount)
    {
        Resource resource = GetResource(definition);

        if (resource == null)
            return;

        resource.Restore(amount);
    }

    private void CreateResource(ResourceDefinition definition, StatContainer stats)
    {
        if (definition == null)
            return;

        if (resources.ContainsKey(definition))
            return;

        RuntimeStat maxStat = stats.GetStat(definition.MaxValueStat);

        if (maxStat == null)
            return;

        Resource resource = new Resource(definition, maxStat);

        resources.Add(definition, resource);
    }
}