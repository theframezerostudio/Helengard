public readonly struct HitAnimationRequest
{
    public readonly string StateName;
    public readonly HitDirection Direction;
    public readonly HitHeight Height;
    public readonly SwingType SwingType;

    public HitAnimationRequest(
        string stateName,
        HitDirection direction,
        HitHeight height,
        SwingType swingType)
    {
        StateName = stateName;
        Direction = direction;
        Height = height;
        SwingType = swingType;
    }
}