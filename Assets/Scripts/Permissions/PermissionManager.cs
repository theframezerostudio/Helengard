using System.Collections.Generic;

public class PermissionManager
{
    private readonly Dictionary<AbilityTag, int> blockers = new();

    public event System.Action<AbilityTag, bool> OnPermissionChanged;

    public bool IsAllowed(AbilityTag category)
    {
        return !blockers.TryGetValue(category, out int count) || count <= 0;
    }

    public BlockHandle Block(AbilityTag category)
    {
        bool wasAllowed = IsAllowed(category);

        if (!blockers.ContainsKey(category))
            blockers[category] = 0;

        blockers[category]++;

        if (wasAllowed)
            OnPermissionChanged?.Invoke(category, false);

        return new BlockHandle(this, category);
    }

    public void BlockAll()
    {
        foreach (AbilityTag category in System.Enum.GetValues(typeof(AbilityTag)))
        {
            Block(category);
        }
    }

    public void Release(AbilityTag category)
    {
        if (!blockers.ContainsKey(category)) return;

        blockers[category]--;

        if (blockers[category] <= 0)
        {
            blockers.Remove(category);
            OnPermissionChanged?.Invoke(category, true); 
        }
    }

    public void ReleaseAll()
    {
        foreach (AbilityTag category in System.Enum.GetValues(typeof(AbilityTag)))
        {
            while (!IsAllowed(category))
            {
                Release(category);
            }
        }
    } 
}