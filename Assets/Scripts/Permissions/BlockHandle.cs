public struct BlockHandle
{
    private readonly PermissionManager system;
    private readonly AbilityTag category;
    private bool released;

    public BlockHandle(PermissionManager system, AbilityTag category)
    {
        this.system = system;
        this.category = category;
        this.released = false;
    }

    public void Release()
    {
        if (released) return;

        system.Release(category);
        released = true;
    }
}