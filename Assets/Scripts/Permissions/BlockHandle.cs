public struct BlockHandle
{
    private readonly PermissionManager system;
    private readonly AbilityTag tag;
    private bool released;

    public BlockHandle(PermissionManager system, AbilityTag tag)
    {
        this.system = system;
        this.tag = tag;
        this.released = false;
    }

    public void Release()
    {
        if (released) return;

        system.Release(tag);
        released = true;
    }
}