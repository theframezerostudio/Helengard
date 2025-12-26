using UnityEngine;

public abstract class LocomotionMode
{
    protected Player player;

    public LocomotionMode(Player player)
    {
        this.player = player;
    }

    public abstract void Move(Vector3 dir, float movemwntSpeed);
    public abstract void Rotate(Vector2 dir);
    public abstract void PlayAnimation(Vector3 input);
    public abstract void StopAnimation();
    public abstract void PerformDash(Vector2 dir);
    public abstract Vector3 GetDirection(Vector2 input);
}
