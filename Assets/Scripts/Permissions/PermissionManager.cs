using System.Collections.Generic;
public enum AbilityTag
{
    Move,
    Jump,
    Attack,
    Cast,
    Guard,
    Aim
}

public class PermissionManager
{
    private readonly Dictionary<AbilityTag, int> blockers = new();

    public bool IsAllowed(AbilityTag tag)
    {
        return !blockers.TryGetValue(tag, out int count) || count <= 0;
    }

    public BlockHandle Block(AbilityTag tag)
    {
        if (!blockers.ContainsKey(tag))
            blockers[tag] = 0;

        blockers[tag]++;
        return new BlockHandle(this, tag);
    }

    internal void Release(AbilityTag tag)
    {
        if (!blockers.ContainsKey(tag)) return;

        blockers[tag]--;

        if (blockers[tag] <= 0)
            blockers.Remove(tag);
    }
}