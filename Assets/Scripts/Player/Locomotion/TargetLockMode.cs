using UnityEngine;

public class TargetLockMode : LocomotionMode
{
    private Target target;

    public TargetLockMode(Player player, MotionAccumulator motion, Target target) : base(player, motion)
    {
        this.target = target;
    }

    public override void Move(Vector3 dir, float speed)
    {
    }

    public override void Move(Vector2 input, float movementSpeed)
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

    public override void AddImpulse(Vector2 input, float distance)
    {
    }

    public override void AddImpulse(Vector3 dir, float distance)
    {
    }

    public override void ResetVelocity()
    {
    }
}
