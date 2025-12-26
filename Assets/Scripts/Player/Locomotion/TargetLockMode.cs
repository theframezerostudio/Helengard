using UnityEngine;

public class TargetLockMode : LocomotionMode
{
    private Target target;

    public TargetLockMode(Player player, Target target) : base(player)
    {
        this.target = target;
    }

    public override void Move(Vector3 dir, float speed)
    {
    }

    public override void PerformDash(Vector2 dir)
    {
    }

    public override void PlayAnimation(Vector3 input)
    {
    }

    public override void Rotate(Vector2 dir)
    {
    }

    public override void StopAnimation()
    {
    }

    public override Vector3 GetDirection(Vector2 input)
    {
        return Vector3.zero;
    }
}
