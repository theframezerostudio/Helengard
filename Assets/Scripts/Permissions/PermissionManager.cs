using System.Collections.Generic;

public class PermissionManager
{
    private readonly Dictionary<AbilityTag, int> blockers = new();

    public event System.Action<AbilityTag, bool> OnPermissionChanged;

    public bool IsAllowed(AbilityTag tag)
    {
        return !blockers.TryGetValue(tag, out int count) || count <= 0;
    }

    public BlockHandle Block(AbilityTag tag)
    {
        bool wasAllowed = IsAllowed(tag);

        if (!blockers.ContainsKey(tag))
            blockers[tag] = 0;

        blockers[tag]++;

        if (wasAllowed)
            OnPermissionChanged?.Invoke(tag, false);

        return new BlockHandle(this, tag);
    }

    public void BlockAll()
    {
        foreach (AbilityTag tag in System.Enum.GetValues(typeof(AbilityTag)))
        {
            Block(tag);
        }
    }

    public void Release(AbilityTag tag)
    {
        if (!blockers.ContainsKey(tag)) return;

        blockers[tag]--;

        if (blockers[tag] <= 0)
        {
            blockers.Remove(tag);
            OnPermissionChanged?.Invoke(tag, true); 
        }
    }

    public void ReleaseAll()
    {
        foreach (AbilityTag tag in System.Enum.GetValues(typeof(AbilityTag)))
        {
            while (!IsAllowed(tag))
            {
                Release(tag);
            }
        }
    }

}