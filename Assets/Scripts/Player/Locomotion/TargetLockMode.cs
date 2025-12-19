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

    public override void PerformDodge(Vector2 dir)
    {
    }

    public override void PlayAnimation(Vector3 input)
    {
    }

    public override void Rotate(Vector3 dir)
    {
    }
}
