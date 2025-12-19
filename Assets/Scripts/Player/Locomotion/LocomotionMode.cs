using UnityEngine;

public abstract class LocomotionMode
{
    protected Player player;

    public LocomotionMode(Player player)
    {
        this.player = player;
    }

    public abstract void Move(Vector3 dir, float movemwntSpeed);
    public abstract void Rotate(Vector3 dir);
    public abstract void PlayAnimation(Vector3 input);
    public abstract void PerformDodge(Vector2 dir);
}
